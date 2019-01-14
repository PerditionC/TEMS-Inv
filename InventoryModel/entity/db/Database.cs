// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using NLog;
using StatePrinting;
using SQLite;
using SQLiteNetSessionModule.Sync;

using TEMS.InventoryModel.util.attribute;
using System.Runtime.InteropServices;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// used to determine if when closing out a transaction/savepoint if should commit or rollback changes
    /// </summary>
    public enum EndTransactionAction
    {
        COMMIT,
        ROLLBACK
    }

    /// <summary>
    /// Interface to backend database - direct manipulation of database handled via an instance of this class.
    /// </summary>
    public sealed class Database : IDisposable
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Stateprinter statePrinter = new Stateprinter();

        // force load and validate SQLite DLL only on first instance
        private static bool InitDll = true;

        // maintains our connection to the SQLite database
        private SQLiteConnection dbConnection { get; set; }

        // what version of database schema we support/expect
        public static readonly int DB_SCHEMA_VERSION = 2;

        // file path including name of database file
        public string dbPath { get; private set; } = null;

        // SQLite error callback
        // stored in variable to ensure delegate passed is kept from GC as long as Database object is alive, delegate does not require pinning
        private readonly SQLite3.ErrorLogCallback SQLiteErrorLogCallbackDelegate = (IntPtr pArg, int iErrCode, string zMsg) =>
        {
            logger.Error($"SQLite Error: error ({iErrCode}) - {zMsg}");
        };

        // must specify database file on creation
        // will throw an error if database not found or invalid format
        public Database(string dbPath)
        {
            logger.Trace(nameof(Database));
            this.dbPath = dbPath;

            // check if the database exists
            ValidateDbFileExists();

            if (InitDll)
            {
                InitDll = false;

                // add support for loading correct native DLLs under both Win32 (x86) and Win64 (amd64)
                try
                {
                    SQLiteLoader.SetNativeDllDirectory();

                    // force loading of DLL (should throw an error if failed to load DLL) and log version
                    var sqliteVersion = SQLite3.LibVersionNumber();
                    logger.Info($"SQLite3 DLL version is {sqliteVersion}");
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Critical error initializing dynamic libraries!  ABORTING! - {e.Message}");
                    throw;
                }

                // setup logging SQLite errors, must be done before Initialize()
                if (SQLite3.Config(SQLite3.ConfigOption.Log, SQLiteErrorLogCallbackDelegate, IntPtr.Zero) != SQLite3.Result.OK) logger.Error("Failed to register SQlite error log callback!");

                // ensure initialized (future versions and some embedded versions may not automatically call this)
                // [may be automatically invoked by Open() call when creating the SQLiteConnection]
                if (SQLite3.Initialize() != SQLite3.Result.OK) logger.Warn("Failed to Initialize() SQLite!");
            }

            // attempt to open our connection to db
            dbConnection = new SQLiteConnection(dbPath, storeDateTimeAsTicks: true);

#if DEBUG
            // enable debug output of SQL statements executed
            dbConnection.Trace = true;
#endif

            // confirm it is of expected format
            ValidateSchema();
        }

        #region Dispose()

        public void Dispose()
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
            dbConnection = null;
            SQLite3.Shutdown();
        }

        #endregion Dispose()

        #region database validation

        /// <summary>
        /// indicates if database file opened has expected schema
        /// </summary>
        private void ValidateSchema()
        {
            logger.Trace(nameof(ValidateSchema));
            int currentVersion = -1;
            try
            {
                currentVersion = dbConnection.ExecuteScalar<int>("SELECT [value] FROM [META] WHERE [key]='dbVersion';");
            }
            catch (Exception e)
            {
                /* ignore any errors, but log in case not just a simple version mismatch */
                logger.Warn(e, "DB:validateSchema() - Unable to retrieve existing database schema version.");
            }

            if (currentVersion != DB_SCHEMA_VERSION)
                throw new DatabaseFormatException($"The database does not have the expected schema!  Please update the database - {dbPath}.");
        }

        /// <summary>
        /// Validates database file exists prior to attempting to open.
        /// Throws exception if not found.
        /// Note: when connecting may still throw an exception during SQLite.Open()
        /// call if there are other issues with the database file, e.g. corrupt or file permissions.
        /// </summary>
        private void ValidateDbFileExists()
        {
            logger.Trace(nameof(ValidateDbFileExists));
            // verify a valid path argument provided
            if (dbPath == null) throw new ArgumentNullException("dbPath", "Unable to connect to database; Property value not set for database file name and path!");

            // see if file actually exists
            if (!System.IO.File.Exists(dbPath))
            {
                throw new System.IO.FileNotFoundException($"Database not found; verify database path and filename is properly configured! [{dbPath}]", dbPath);
            }
        }

        #endregion database validation

        #region transactions

        public void BeginTransaction()
        {
            logger.Trace(nameof(BeginTransaction));
            try
            {
                dbConnection.BeginTransaction();
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unable to BeginTransaction() - {e.Message} {e.GetSqliteExtendedError()}");
                throw;
            }
        }

        public void EndTransaction(EndTransactionAction action = EndTransactionAction.COMMIT)
        {
            logger.Trace(nameof(EndTransaction));
            if (action == EndTransactionAction.COMMIT)
            {
                try
                {
                    dbConnection.Commit();
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Error committing transaction - {e.Message} {e.GetSqliteExtendedError()}");
                    throw;
                }
            }
            else
            {
                try
                {
                    dbConnection.Rollback();
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Error rolling back transaction - {e.Message} {e.GetSqliteExtendedError()}");
                    throw;
                }
            }
        }

        public string SaveTransactionPoint()
        {
            logger.Trace(nameof(SaveTransactionPoint));
            try
            {
                return dbConnection.SaveTransactionPoint();
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unable to SaveTransactionPoint() - {e.Message} {e.GetSqliteExtendedError()}");
                throw;
            }
        }

        public void ReleaseTransactionPoint(string savepoint, EndTransactionAction action = EndTransactionAction.COMMIT)
        {
            logger.Trace(nameof(EndTransaction));
            if (action == EndTransactionAction.COMMIT)
            {
                try
                {
                    dbConnection.Release(savepoint);
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Error committing savepoint - {e.Message} {e.GetSqliteExtendedError()}");
                    throw;
                }
            }
            else
            {
                try
                {
                    dbConnection.RollbackTo(savepoint);
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Error rolling back to savepoint - {e.Message} {e.GetSqliteExtendedError()}");
                    throw;
                }
            }
        }

        #endregion transactions

        #region save and delete entity

        /// <summary>
        /// Removes row from table corresponding to entity; does Not cascade deletes so
        /// any one-to-one mappings should be deleted 1st manually.
        /// Note: Entity reference is added to a Tombstone shadow table for replication purposes prior to actual delete.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete<T>(T entity) where T : class
        {
            int rowsUpdated = 0;
            string savePointName = null;

            try
            {
                // use SavePoint instead of Transaction as they can nest, only 1 Transaction allowed
                savePointName = SaveTransactionPoint();

                // we must delete many-to-many mappings to avoid FOREIGN CONSTRAINT violation on deletion
                // we are not cascading deletes to objects related to, only the mapping tables
                entity.IterateOverManyToManyRelatedEntities(((mappingType, entityPkProp, foreignEntityType, foreignPkProp, entityPk, value) =>
                {
                    // warning, mappings in current object are ignored, only existing stored mappings are deleted
                    // load existing mappings so can remove stale ones and use existing primary keys on unchanged ones
                    var currentMappings = this.InvokeLoadRows(mappingType, $"WHERE {entityPkProp.Name}=?;", new object[] { entityPk.ToString() });

                    // delete stale mappings
                    var mappingPkProp = mappingType.GetPrimaryKey();
                    foreach (var mapping in currentMappings) // collection of mappings of related objects
                    {
                        dbConnection.Execute($"DELETE FROM `{mappingType.Name}` WHERE {mappingPkProp.Name}=?;", mappingPkProp.GetValue(mapping, null).ToString());
                    }
                }));

                // Note: triggers will add entry in shadow table
                rowsUpdated = dbConnection.Delete(entity);

                ReleaseTransactionPoint(savePointName, EndTransactionAction.COMMIT);
            }
            catch (SQLite.SQLiteException e) // e.g. SQLite.NotNullConstraintViolationException
            {
                try
                {
                    ReleaseTransactionPoint(savePointName, EndTransactionAction.ROLLBACK);
                }
                catch (Exception e2)
                {
                    logger.Error(e2, $"Error rolling back to previous state from SavePoint {savePointName}. {e.GetSqliteExtendedError()}");
                }
                logger.Error(e, "Failed to delete! Error:" + e.Message + $" {e.GetSqliteExtendedError()}", "Error Deleting:");
                throw;
            }

            return rowsUpdated > 0;
        }

        /// <summary>
        /// Removes all rows from table corresponding to sql query; does Not cascade deletes so
        /// any one-to-one mappings should be deleted 1st manually.
        /// Note: deleted items are added to a Tombstone shadow table for replication purposes prior to actual delete.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public int DeleteAll(string table, string condition, params object[] args)
        {
            int rowsUpdated = 0;
            string savePointName = null;

            try
            {
                // use SavePoint instead of Transaction as they can nest, only 1 Transaction allowed
                savePointName = SaveTransactionPoint();

                // Note: triggers will add entry in shadow table
                rowsUpdated = dbConnection.Execute($"DELETE FROM [{table}] WHERE {condition};", args);

                ReleaseTransactionPoint(savePointName, EndTransactionAction.COMMIT);
            }
            catch (SQLite.SQLiteException e) // e.g. SQLite.NotNullConstraintViolationException
            {
                try
                {
                    ReleaseTransactionPoint(savePointName, EndTransactionAction.ROLLBACK);
                }
                catch (Exception e2)
                {
                    logger.Error(e2, $"Error rolling back to previous state from SavePoint {savePointName}. {e.GetSqliteExtendedError()}");
                }
                logger.Error(e, "Failed to delete! Error:" + e.Message + $" {e.GetSqliteExtendedError()}", "Error Deleting:");
                throw;
            }

            return rowsUpdated;
        }

        /// <summary>
        /// Updates or Inserts a record associated with obj based on the object's Type and Primary Key
        /// will throw an Exception if error saving
        /// </summary>
        /// <param name="entity">object with public properties to be added (inserted) into database</param>
        /// <param name="beginNewTransaction">when true performs db action within a newly started transaction,
        /// if false (default) assumes no transaction required or already in an existing transaction
        /// (such as recursive calls).</param>
        public void Save<T>(T entity, bool beginNewTransaction = true) where T : class
        {
            string savePointName = null;

            try
            {
                // because user could invoke Save within another transaction we use savepoints instead
                // as they allow nesting within a transaction and otherwise work as a transaction if no outer one
                if (beginNewTransaction) savePointName = SaveTransactionPoint();

                // ensure any foreign keys exist (related entities written first)
                entity.IterateOverRelatedEntities((x => this.Save(entity: x, beginNewTransaction: false)));

                // update self in db but skip if can determine no changes
                if (entity is IChangeTracking item)
                {
                    if (item.IsChanged)
                    {
                        // dbConnection.Upsert(entity);  // Note: can't upsert if multiple uniqueness requirements
                        var success = dbConnection.InsertOrIgnore(entity);
                        if (success < 1) dbConnection.Update(entity);
                        item.AcceptChanges();
                    }
                }
                else
                {
                    // if doesn't support change tracking then always update db
                    // dbConnection.Upsert(entity);  // Note: can't upsert if multiple uniqueness requirements
                    var success = dbConnection.InsertOrIgnore(entity);
                    if (success < 1) dbConnection.Update(entity);
                }

                // now all foreign keys and entity's primary key exists, update any many-to-many mapping tables
                // Note: we need to remove any stale mappings as well, so as to avoid unnecessary updates to
                // db replication we also need to keep mapping's primary key unchanged for existing mappings.
                entity.IterateOverManyToManyRelatedEntities(((mappingType, entityPkProp, foreignEntityType, foreignPkProp, entityPk, value) =>
                {
                    // load existing mappings so can remove stale ones and use existing primary keys on unchanged ones
                    var currentMappings = this.InvokeLoadRows(mappingType, $"WHERE {entityPkProp.Name}=?;", new object[] { entityPk.ToString() });

                    // create new mappings in a dictionary based on foreign key of related object
                    Dictionary<object, object> newMappings = new Dictionary<object, object>();
                    foreach (var element in value) // collection of related objects
                    {
                        var mapping = Activator.CreateInstance(mappingType);
                        entityPkProp.SetValue(mapping, entityPk, null);
                        var foreignPk = foreignEntityType/*element.GetType()*/.GetPrimaryKey().GetValue(element, null);
                        foreignPkProp.SetValue(mapping, foreignPk, null);

                        newMappings.Add(foreignPk, mapping);
                    }

                    // delete stale mappings and ensure new mapping pk matches existing mappings' pk
                    var mappingPkProp = mappingType.GetPrimaryKey();
                    foreach (var mapping in currentMappings) // collection of mappings of related objects
                    {
                        var foreignPk = foreignPkProp.GetValue(mapping, null);
                        // if mapping in current and new then update pk to match, else (not in new but in current) then need to delete it
                        if (newMappings.ContainsKey(foreignPk))
                        {
                            // set the mapping primary key in new mapping (stored in newMappings dictionary) to
                            // the value of the primary key in the current mapping using reflection to get & set values
                            mappingPkProp.SetValue(newMappings[foreignPk], mappingPkProp.GetValue(mapping, null), null);
                            if (newMappings[foreignPk] is IChangeTracking changeTracking)
                            {
                                changeTracking.AcceptChanges();
                            }
                        }
                        else
                        {
                            dbConnection.Execute($"DELETE FROM `{mappingType.Name}` WHERE {mappingPkProp.Name}=?;", mappingPkProp.GetValue(mapping, null).ToString());
                        }
                    }

                    // actually update (save) the mappings
                    foreach (var mapping in newMappings)
                    {
                        this.Save(mapping.Value, beginNewTransaction: false);
                    }
                }));

                if (beginNewTransaction) ReleaseTransactionPoint(savePointName, EndTransactionAction.COMMIT);
            }
            catch (SQLite.SQLiteException e) // e.g. SQLite.NotNullConstraintViolationException
            {
                // we allow exceptions to propagate backwards if multiple nested Save calls
                // to first call to begin transaction, then we rollback the changes up to
                // prior to that Save call and only then log the error (to avoid repetitive
                // error messages in log, one for each Save nesting level.
                if (beginNewTransaction)
                {
                    ReleaseTransactionPoint(savePointName, EndTransactionAction.ROLLBACK);
                    logger.Error(e, "Failed to save! Error:" + e.Message + $" {e.GetSqliteExtendedError()}", "Error Saving:");
                }
                throw;
            }
        }

        #endregion save and delete entity

        #region load entity

        /// <summary>
        /// generic query DB
        /// Used when a subset of fields are needed and not whole items, e.g. reports or searching
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task<List<T>> QueryAsync<T>(string sql, params object[] args)
            where T : new()
        {
            logger.Trace(nameof(QueryAsync));
            return Task<List<T>>.Factory.StartNew(() =>
            {
                var conn = dbConnection;
                //using (conn.Lock()) - TODO proper locking, note we use SQLite in default serialized mode and this is only for read queries
                {
                    return conn.Query<T>(sql, args);
                }
            });
        }

        /// <summary>
        /// generic query DB for a single result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, params object[] args)
        {
            logger.Trace(nameof(ExecuteScalar));
            return dbConnection.ExecuteScalar<T>(sql, args);
        }

        /// <summary>
        /// non generic version of Load, must provide TableName where
        /// TableName is the same as the entity's Type
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public object Load(Object pk, string TableName)
        {
            if (TableName == null) throw new ArgumentNullException("TableName", "Missing required TableName==entity Type");
            if (pk == null) throw new ArgumentNullException("pk", "Missing required value, primary key is null");

            var method = this.GetType().GetMethod("Load", Type.GetTypeArray(new object[] { pk }));
            var objType = Type.GetType("TEMS.InventoryModel.entity.db." + TableName);
            var item = method.MakeGenericMethod(new Type[] { objType }).Invoke(this, new object[] { pk });
            return item;
        }

        /// <summary>
        /// Queries back-end for entity corresponding to primary key for type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pk"></param>
        /// <returns>the entity and nested entities corresponding to PK for T, throws exception if PK is not found in DB</returns>
        public T Load<T>(Object pk) where T : class, new()
        {
            logger.Trace(nameof(Load));
            T item = default;

            try
            {
                // return cached item if exists first
                item = DataRepository.GetDataRepository.ReferenceData[typeof(T).Name, pk] as T;
                if (item != null) return item;

                // get the item including related items, throws exception if primary key not found
                item = dbConnection.Get<T>(pk);

                // load in nested entities and mark as not changed
                LoadChildren(item.GetType(), new List<T>() { item });
            }
            catch (SQLite.SQLiteException e)
            {
                logger.Error(e, "Failed to load item. Error:" + e.Message, "Error Loading:");
                throw;
            }

            return item;
        }

        /// <summary>
        /// Dynamically invokes LoadRows(null) for Type as specified by objType when
        /// objType is not known/available at compile time.
        /// Note: this implementation must be in same object as method invoked
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        public IEnumerable InvokeLoadRows(Type objType, string query = null, params object[] args)
        {
            logger.Trace(nameof(InvokeLoadRows));
            try
            {
                var method = this.GetType().GetMethod("LoadRows"); //, new Type[] { typeof(System.String, System.ParamArray) });
                var items = method.MakeGenericMethod(new Type[] { objType }).Invoke(this, new object[] { query, args });

                // and convert to collection type used in our view model
                //return new ObservableCollection<ItemBase>(((IEnumerable)items).Cast<ItemBase>());
                return (IEnumerable)items;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error in InvokeLoadRows() - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Returns items based on query; assumes table name is same as type T
        /// Similar to Load but with explicit query instead of using primary key,
        /// as such may return multiple items.
        /// Note: query string must include the 'WHERE' literal if WHERE clause is used,
        /// it may optionally also include additional join conditions, defaults to a
        /// SELECT all T from table for type T with no conditions or joins.
        /// Recursively loads nested objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IList<T> LoadRows<T>(string query = null, params object[] args) where T : class, new()
        {
            logger.Trace(nameof(LoadRows));
            IList<T> items = null;

            try
            {
                // load items of type T that correspond with query
                var tableName = typeof(T).GetTableName();
                items = dbConnection.Query<T>($"SELECT {tableName}.* FROM {tableName} " + (query ?? string.Empty) + ";", args).NotChanged();

                // load additional items for many-to-one and many-to-many mappings
                LoadChildren(typeof(T), items);
            }
            catch (SQLite.SQLiteException e)
            {
                logger.Error(e, "Failed to load [all] items. Error:" + e.Message, "Error Loading:");
                throw;
            }

            return items;
        }

        /// <summary>
        /// will recursively load inner or child items of all T in items list
        /// and mark all items supporting IChangeTracking as not changed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="itemType"></param>
        /// <param name="items"></param>
        private void LoadChildren<T>(Type itemType, IList<T> items) where T : class, new()
        {
            logger.Trace(nameof(LoadChildren));
            if ((items == null) || (!items.Any())) return; // exit early if no parent elements

            try
            {
                // load in nested entities
                // get a list of all PropertyInfo for each member with a [ForeignKey] attribute
                var fkList = itemType.GetForeignKeyProperties();
                foreach (var fk in fkList)
                {
                    // get the attribute so we can extract which member property to store value in
                    var fkAttribute = fk.GetAttribute<ForeignKeyAttribute>();
                    if (fkAttribute.EntityPropertyName != null)
                    {
                        // get the property of the member property we are storing related value in
                        var fk_entityProp = itemType.GetPropertyInfo(fkAttribute.EntityPropertyName);

                        foreach (var item in items)
                        {
                            // get the foreign key value (primary key for the related item)
                            var fk_pk = fk.GetValue(item, null);
                            // may not be any value stored, e.g. parentId may be null if no parent
                            if (fk_pk != null)
                            {
                                // load the related item from DB (will recursively load any of its items as well)
                                // WARNING: we do not handle loops, if item A hold item B and B holds A will get stuck!
                                var relatedItem = Load(fk_pk, fk_entityProp.PropertyType.Name);

                                // and store in our item
                                fk_entityProp.SetValue(item, relatedItem, null);
                            }
                        }
                    }
                }

                // load Many to Many collection

                // get PropertyInfo for each ManyToOne or ManyToMany property
                foreach (var relationshipProperty in itemType.GetRelationshipProperties())
                {
                    // we only care about ManyToMany, so ignore others
                    var relationshipAttribute = relationshipProperty.GetAttribute<ManyToManyAttribute>();
                    if (relationshipAttribute == null) continue;

                    // get Type used for mapping
                    Type mappingType = relationshipAttribute.IntermediateType;
                    PropertyInfo entityPkProp = null;
                    PropertyInfo foreignPkProp = null;
                    Type foreignEntityType = null;

                    // get foreign key props (should only be two PropertyInfo objects to enumerate through)
                    foreach (var fkProp in mappingType.GetForeignKeyProperties())
                    {
                        var fkAttribute = fkProp.GetAttribute<ForeignKeyAttribute>();
                        if (fkAttribute.ForeignTableType == itemType)
                        {
                            entityPkProp = fkProp;
                        }
                        else //if (fkAttribute.ForeignTableType == relationshipProperty.PropertyType) -- collection of T, need T
                        {
                            foreignEntityType = fkAttribute.ForeignTableType;
                            foreignPkProp = fkProp;
                        }
                    }

                    foreach (var item in items)
                    {
                        // instead of creating a new collection
                        //IList relatedCollection = (IList)Activator.CreateInstance(relationshipProperty.PropertyType);
                        // simply get existing one (assumes default initialized to empty collection
                        IList relatedCollection = (IList)relationshipProperty.GetValue(item, null);

                        var mappings = InvokeLoadRows(mappingType, $"WHERE {entityPkProp.Name}=?; ", itemType.GetPrimaryKey().GetValue(item, null));

                        foreach (var mapping in mappings)
                        {
                            var foreignPk = foreignPkProp.GetValue(mapping, null);
                            var value = Load(foreignPk, foreignEntityType.Name);
                            relatedCollection.Add(value);
                        }
                    }
                }

                // mark item as unchanged if supports change tracking [as we just loaded from DB]
                // Note: nested elements will already be marked unchanged when they were loaded.
                foreach (var item in items)
                {
                    if (item is IChangeTracking entity /* && entity.IsChanged */) entity.AcceptChanges();
                }
            }
            catch (SQLite.SQLiteException e)
            {
                logger.Error(e, "Failed to load item's inner items. Error:" + e.Message, "Error Loading:");
                throw;
            }
        }

        #endregion load entity

        #region replication

        // Only need to add tables that are to be synchronized [replicated] here, all others ignored
        // Note: order here matters! this is the order items are sync'd so should be done in order to
        // allow foreign keys and constraints to be handled properly
        private static readonly string[] tablesToSync = {
            "SiteLocation",
            "SiteLocationEquipmentUnitTypeMapping",
            "UserDetail",
            "UserActivity",
            "UserSiteMapping",
            "Image",
            "Document",
            "UnitOfMeasure",
            "BatteryType",
            "VehicleLocation",
            "ItemStatus",
            "ItemCategory",
            "ServiceCategory",
            "VendorDetail",
            "EquipmentUnitType",
            "ItemType",
            "Item",
            "ItemInstance",
            "ItemService",
            "ItemServiceHistory",
            "DeployEvent",
            "DamageMissingEvent",
        };

        public void SyncFromDb(string replicaDB)
        {
            var dbSync = new DBSync(dbConnection);
            dbSync.SyncOneWay(replicaDB, tablesToSync);
        }

        public void SyncToDb(string replicaDB)
        {
            var replicaDBConnection = new SQLiteConnection(replicaDB, storeDateTimeAsTicks: true);
            var dbSync = new DBSync(replicaDBConnection);
            dbSync.SyncOneWay(dbPath, tablesToSync);
        }

        public void SyncWithDb(string replicaDB)
        {
            SyncFromDb(replicaDB);
            SyncToDb(replicaDB);
        }

        #endregion replication
    }
}