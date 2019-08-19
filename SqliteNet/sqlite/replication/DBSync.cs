// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using SQLite;
using SQLiteNetSessionModule;
using StatePrinting;
using static SQLiteNetSessionModule.SQLiteSession;

namespace SQLiteNetSessionModule.Sync
{
    /// <summary>
    /// Uses SQLite Session to synchronize (replicate) two databases.
    /// Currently does simple last update wins scenerio.
    /// Assumptions:
    /// primary keys are globally unique, i.e. either GUIDs or natural keys such that
    /// if items are created at two distinct locations then identical keys are unlikely or
    /// indicate that rows refer to same thing.  (Failure of this assumption will likely
    /// result in loss of information when the conflicting pk rows are sync'd.)
    /// Requirements:
    /// Each table should have a column called "timestamp" that contains last modification
    /// timestamp; recommended set via trigger and otherwise ignored.  Initially this should
    /// be the creation time.  This is what allows latest changed row to be selected.
    /// Each table should have a shadow table, called same name with a _ suffix, e.g. 
    /// shadow table for "MyTable" is "MyTable_"
    /// This is used to store deleted rows.  Only required columns are same primary key(s)
    /// and a column named "del_timestamp" which contains timestamp of when deleted.
    /// Note: this row may contain full contents of row prior to deletion allowing easier
    /// restoration [unrelated to sync] and enhanced sync'ing protocols other than latest wins.
    /// Optional:
    /// If columns have a based_on GUID and a current_version GUID then enhanced sync'ing 
    /// protocols may be devised. TODO - ???
    /// </summary>
    public class DBSync
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Stateprinter statePrinter = new Stateprinter();

        // database we update during replication
        private SQLiteConnection db { get; }

        public DBSync(SQLiteConnection db)
        {
            this.db = db;
        }


        /// <summary>
        /// Internal convenience method to throw exception on replication error
        /// and log exception at the same time.
        /// </summary>
        /// <param name="dbResult">result from an SQLite function, expected to
        /// return OK if successful or error code otherwise</param>
        /// <param name="message">information about action being performed (that failed)</param>
        private void IfNotOKThenThrowReplicationException(SQLite3.Result dbResult, string message)
        {
            if (dbResult != SQLite3.Result.OK)
            {
                var e = new ReplicationException(message, dbResult);
                logger.Error(e, "Sync failed - " + message);
                throw e;
            }
        }


        /*
         *  ///  have deletes trigger an add to a shadow table so we can get last modified (ie deleted timestamp)
         *  for insert see if deleted in replica, if so then replace, else omit
         *  for delete see if deleted in current, if so then omit, else replace
         *         
         * for update need row last modified timestamp
         *         use either always the latest change, ask ..., or always keep/replace
         */

        /// <summary>
        /// returns Dictionary keyed on table name of information about table's columns
        /// Does not query for information until first call, subsequent calls return cached information.
        /// Note: index of ColumnInfo List[i] corresponds with index of primary key array from ChangeSetPrimaryKey
        /// </summary>
        private Dictionary<string, List<SQLiteConnection.ColumnInfo>> _tableInformation = null;
        public Dictionary<string, List<SQLiteConnection.ColumnInfo>> tableInformation
        {
            get
            {
                if (_tableInformation == null)
                {
                    _tableInformation = new Dictionary<string, List<SQLiteConnection.ColumnInfo>>(_tablesToSync.Length);

                    // also cache information about table columns for conflict resolution use
                    foreach (var tableName in _tablesToSync)
                    {
                        _tableInformation.Add(tableName, db.GetTableInfo(tableName));
                    }
                }

                return _tableInformation;
            }
        }
        private string[] _tablesToSync = null;

        /// <summary>
        /// Perform one way synchronization where all changes in from db are replicated
        /// to current db with conflict resolution.  Once complete the current db will
        /// contain all changes made to syncFromDb (minus conflicting changes) and all
        /// changes to current db since last sync (minus conflicting changes).
        /// Note: dbToSyncFrom is not modified, i.e. changes within current db are not
        /// replicated back (hence the one way).
        /// On conflicting change only one or the other is used (obviously) - each
        /// table has its own conflict handler and that determines whether lastest
        /// change wins, current db wins, or user is asked.
        /// Uses active db connection for database to sync (db to update)
        /// </summary>
        /// <param name="dbToSyncFromFilename">filename (including path) to synchronize with</param>
        public void SyncOneWay(string dbToSyncFromFilename, string[] tablesToSync)
        {
            // todo fix properly
            _tablesToSync = tablesToSync;

            // session used during replication, Zero if no session currently open
            var session = IntPtr.Zero;

            try
            {
                logger.Info($"SyncOneWay with {dbToSyncFromFilename}");

                // open both database files in same db instance as 'main' and 'replica'
                db.AttachDB(dbToSyncFromFilename, "replica");

                // create a session, used to obtain differences
                // Note: we use reverse of desired to force each change to conflict, if we created session on "replica"
                // and diff to "main" below, then no conflicts will occur during applying of changset and when complete
                // main db will be identical to replica db - but we want a chance to evaluate each change before
                // applying, so we create the inverse changeset which forces a conflict for each change
                IfNotOKThenThrowReplicationException(db.CreateSession("main", out session), "Failed to create session");

                // do for each table that should be synchronized

                // attach all tables [from "main"] to it
                IfNotOKThenThrowReplicationException(SQLiteSession.AttachToTable(session, null), "Failed to attach tables");

                // Note: we have not enabled watching for changes

                // validate no changes have occurred
                if (!SQLiteSession.IsEmptySession(session)) throw new ReplicationException("Session is not empty!");

                // see what it would take to make main.table like replica.table
                foreach (var tableName in tablesToSync)
                {
                    IfNotOKThenThrowReplicationException(SQLiteSession.AddTableDiffToSession(session, "replica", tableName, out string errMsg), $"Unable to determine differences for table {tableName}: {errMsg}");
                }


                // if there are any changes then session will not be empty, it should contain the difference
                if (SQLiteSession.IsEmptySession(session))
                {
                    logger.Debug("Replication - no changes, session is empty after differencing.");
                }

                // create change set
                IfNotOKThenThrowReplicationException(SQLiteSession.GenerateChangeSet(session, out SQLiteChangeSet changeSet), "Failed to generate change set from session");
                using (changeSet) // ensure changeSet.Dispose() is called to release change set buffer
                {
                    // on conflicts our conflict handler callback uses our context to handle conflicts accordingly
                    IfNotOKThenThrowReplicationException(db.ApplySessionChangeSet(changeSet, null, CallbackConflictHandler, ctx: this), "Failed to apply replicated changes");
                }
            }
            finally
            {
                // ensure session is closed since we are done with our session, 
                // Ensure attached databases and sessions are cleaned up.
                // May be called if error occurs or as part of normal cleanup
                // Note: we must ensure replica is still available for conflict handler so don't do earlier
                if (session != IntPtr.Zero) SQLiteSession.Delete(session);
                session = IntPtr.Zero;

                // remove secondary db
                db?.DetachDB("replica");
            }
        }



        /// <summary>
        /// Returns index of column columnName in table based on column information
        /// </summary>
        /// <param name="tableInfo">the column information for table to find column in</param>
        /// <param name="columnName">the name of the column index to return</param>
        /// <returns>returns nonnegative index if columnName found, otherwise negative value to indicate not found</returns>
        private static int FindColumn(List<SQLiteConnection.ColumnInfo> tableInfo, string columnName)
        {
            int columnIndex = -1;
            for (int i = 0; i < tableInfo.Count; i++)
            {
                if (String.Equals(columnName, tableInfo[i].Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    columnIndex = i;
                    break; // found our column so stop looking
                }
            }
            return columnIndex;
        }

        /// <summary>
        /// holds all primary key column names and their value for a given row
        /// </summary>
        private struct PrimaryKeyData
        {
            public string name;
            public object value;
        }


        /// <summary>
        /// Conflict handler, allows multiple methods of conflict resolution.
        /// </summary>
        /// <param name="ctx">information about how to handler conflicts per table</param>
        /// <param name="eConflict">reason for callback</param>
        /// <param name="pIter">conflict change set, may iterate through values</param>
        /// <returns>how to respond to conflicting value, leave unchanged, replace, or abort</returns>
        public static ConflictResolution CallbackConflictHandler(IntPtr pCtx, ConflictReason eConflict, IntPtr pIter)
        {
            try
            {
                // get our context
                object ctx = null;
                if (pCtx != IntPtr.Zero)
                {
                    System.Runtime.InteropServices.GCHandle gch = System.Runtime.InteropServices.GCHandle.FromIntPtr(pCtx);
                    ctx = gch.Target;
                }
                var dbSync = ctx as DBSync;
                var dbConnection = dbSync.db;

                // Note: pIter should be converted to a Sqlite3ChangesetIterator to allow using ChangeSetOp,ChangeSet*Value methods
                // however must actually create Sqlite3ConflictChangesetIterator to avoid unnecessary finalize of iterator
                var iter = new Sqlite3ConflictChangesetIterator(pIter);

                // get table, column, and other information about this change
                ChangeSetOp(iter, out string tableName, out int columnCount, out ActionCode op, out bool indirect);

                // get information about columns in current table
                var tableInfo = dbSync.tableInformation[tableName];
                object currentValue, oldValue, newValue;

                // get primary key value for current row - NOTE: we support a single or multiple column PK
                var pkValues = SQLiteSession.ChangeSetPrimaryKeyValues(iter);
                if (pkValues.Count < 1)
                {
                    // we require a primary key and failed to get it!
                    return ConflictResolution.SQLITE_CHANGESET_ABORT;
                }
                PrimaryKeyData[] pk = new PrimaryKeyData[pkValues.Count];
                for (var i = 0; i < pkValues.Count; i++)
                {
                    // get primary key column's value
                    pk[i].value = pkValues[i].Item1.GetValue();
                    // get primary key column's name from index for table
                    pk[i].name = tableInfo[pkValues[i].Item2].Name;
                }

                // log some debug information about conflict (data to sync)
                {
                    var sb = new StringBuilder($"Conflict ({eConflict}) in table '{tableName}' for pk='{pk}', op={op}, Row values:"); sb.AppendLine();
                    for (int i = 0; i < tableInfo.Count; i++)
                    {
                        GetConflictValues(ref iter, op, i, out currentValue, out oldValue, out newValue);
                        sb.AppendLine($"[{i}] current={currentValue}, old={oldValue}, new={newValue}");
                    }
                    logger.Debug(sb.ToString());
                }

                // TODO handle constraint violations better
                if ((eConflict == ConflictReason.SQLITE_CHANGESET_CONSTRAINT) || (eConflict == ConflictReason.SQLITE_CHANGESET_FOREIGN_KEY))
                {
                    return ConflictResolution.SQLITE_CHANGESET_ABORT;
                }

                // if op==insert then in current db and not in replica
                // so is this a new record (keep) or a deleted record in replica (do we want to replicate delete?)
                // newValue (and currentValue) has current's value including timestamp if available
                // oldValue is null, need to query replica's shadow table to see if deleted (and timestamp) or if 
                // not found then assume a new row (so keep)
                //
                // if op==delete then in replica db and not in current
                // so is this a new record (insert) or a deleted record in current (do we want keep delete?)
                // oldValue has the replica's value including timestamp if available
                // newValue (and currentValue) are null, need to query current's shadow table to see if deleted (and timestamp) or if
                // not found then assume a new row (so insert)
                //
                // if op==update then which do we keep, current or replica => we can do this per column or per row
                // oldValue and newValue are null then column has no conflict (currentValue has column's value)
                // oldValue is replica's value, newValue (and currentValue) is current's value
                //
                // TODO - how to handle constraint and foreign key violations, structuring order of replicating tables
                // should avoid these in most cases, for now we punt and abort!


                // get timestamps from current and replica row, if have a timestamp (-1 for column if not one)
                int timestampColumn = FindColumn(tableInfo, "timestamp");
                DateTime? currentTimestamp = null, replicaTimestamp = null;
                if (timestampColumn >= 0)
                {
                    GetConflictValues(ref iter, op, timestampColumn, out currentValue, out oldValue, out newValue);

                    if (op == ActionCode.SQLITE_UPDATE) // both are in conflict values
                    {
                        currentTimestamp = currentValue as DateTime?;
                        replicaTimestamp = (oldValue as DateTime?) ?? currentTimestamp; // no oldValue indicates identical values
                    }
                    else // must query a shadow table
                    {
                        DateTime? shadowTimestamp = null;
                        var query = new StringBuilder($"SELECT [del_timestamp] FROM ");
                        query.Append((op == ActionCode.SQLITE_INSERT) ? "replica" : "main");
                        query.Append(".");
                        query.Append(tableName);
                        query.Append("_ WHERE ");  // shadow table has _ suffix, e.g. [MyTable] shadow table is [MyTable_]
                        var keys = new List<object>(pk.Length);
                        for (var i = 0; i < pk.Length; i++)
                        {
                            if (i != 0) query.Append(" AND ");
                            query.Append(pk[i].name);
                            query.Append("=?");

                            keys.Add(pk[i].value);
                        }
                        query.Append(";");
                        try
                        {
                            // if no shadow table or not a deleted row then this will throw since doesn't exist
                            shadowTimestamp = dbConnection.ExecuteScalar<DateTime>(query.ToString(), keys.ToArray());
                        }
                        catch (Exception) { /* swallow error if no shadow record */ }

                        if (op == ActionCode.SQLITE_INSERT)
                        {
                            if (currentValue != null) currentTimestamp = new DateTime((long)currentValue);
                            replicaTimestamp = shadowTimestamp;
                        }
                        else
                        {
                            currentTimestamp = shadowTimestamp;
                            if (oldValue != null) replicaTimestamp = new DateTime((long)oldValue);
                        }
                    }
                }


                // Note: the way we are using Session, we explicitly make the changes
                // in all cases except on Update where we currently keep or replace whole row
                // TODO allow selection of individual fields? which would require us to manually
                // do the update - hence also returning SQLITE_CHANGESET_OMIT
                var action = ConflictResolution.SQLITE_CHANGESET_OMIT;
                if (op == ActionCode.SQLITE_INSERT)
                {
                    // keep current value unless replicaTimestamp is newer
                    if ((currentTimestamp != null) && (replicaTimestamp != null) && (currentTimestamp < replicaTimestamp))
                    {
                        // TODO - we need to actually issue a DELETE!
                        // TODO - then update shadow table so del_timestamp matches replica
                    }
                }
                else if (op == ActionCode.SQLITE_DELETE)
                {
                    // indicate nothing to do, as we have to manually insert if desired to add
                    action = ConflictResolution.SQLITE_CHANGESET_OMIT;

                    // are we a new row only in replica? or was modified in replica after we deleted so re-add modified value?
                    if ((currentTimestamp == null) ||
                        ((replicaTimestamp != null) && (currentTimestamp < replicaTimestamp)))
                    {
                        // then insert it
                        {
                            var sb = new StringBuilder("INSERT INTO ");
                            sb.Append(tableName);
                            sb.Append(" (");
                            for (var i = 0; i < tableInfo.Count; i++)
                            {
                                if (i != 0) sb.Append(", ");
                                sb.Append("["); // double quoted "column names" are standard, but [name] works and easier to read
                                sb.Append(tableInfo[i].Name); // if any chance of user driven names then use " and escape internal "s
                                sb.Append("]");
                            }
                            sb.Append(") VALUES (");
                            var values = new object[tableInfo.Count];
                            for (var i = 0; i < tableInfo.Count; i++)
                            {
                                if (i != 0) sb.Append(", ");
                                sb.Append("?");

                                oldValue = null;
                                if (ChangeSetOldValue(iter, i, out SQLiteNetSessionModule.SQLiteValue value) == SQLite3.Result.OK)
                                {
                                    if (value.HasValue()) oldValue = value.GetValue();
                                }
                                values[i] = oldValue;
                            }
                            sb.Append(");");
                            logger.Debug("Inserting: '{0}' with values={1}", sb.ToString(), statePrinter.PrintObject(values));
                            dbConnection.Execute(sb.ToString(), values);
                        }
                    }
                    // else assume we deleted on purpose so leave that way
                }
                else if (op == ActionCode.SQLITE_UPDATE)
                {
                    // TODO allow more fine grained selection - for now simply keep latest change
                    if ((currentTimestamp == null) || (replicaTimestamp == null) || (currentTimestamp >= replicaTimestamp))
                        action = ConflictResolution.SQLITE_CHANGESET_OMIT;
                    else
                        action = ConflictResolution.SQLITE_CHANGESET_REPLACE;
                }
                else
                {
                    logger.Error($"Unknown or unexpected op {op} - aborting!");
                }


                // replace only valid on conflict or data
                //if ((eConflict == ConflictReason.SQLITE_CHANGESET_CONFLICT) || (eConflict == ConflictReason.SQLITE_CHANGESET_DATA))
                logger.Debug($"returning action={action}\r\n");
                return action;
            }
            catch (Exception e)
            {
                logger.Error(e, "Unexpected error during conflict handler callback.");
                return ConflictResolution.SQLITE_CHANGESET_ABORT;
            }
        }
    }
}
