// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SQLite;
using static SQLite.SQLite3;
using Sqlite3DatabaseHandle = System.IntPtr; // match SQLite.Net usage

namespace SQLiteNetSessionModule
{
    /// <summary>
    /// typesafe wrapper for iterator used to iterate through a change set
    /// See https://blogs.msdn.microsoft.com/shawnfa/2004/08/12/safehandle/
    /// </summary>
    public class Sqlite3ChangesetIterator : SafeHandle
    {
        protected Sqlite3ChangesetIterator() : base(IntPtr.Zero, true) { }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3changeset_finalize", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result ChangeSetFinalize(IntPtr iter);
        protected override bool ReleaseHandle()
        {
            var result = ChangeSetFinalize(handle);
            handle = IntPtr.Zero;
            // note: sqlite3changeset_finalize should not be called on Sqlite3ChangesetIterator
            // passed to conflict handler; however calling it is a no-op returning misuse
            // so we consider OK or Misuse to be a success
            return (result == Result.OK) || (result == Result.Misuse);
        }
    }

    /// <summary>
    /// typesafe wrapper for iterator used to iterate through a change set during conflict resolution
    /// </summary>
    public sealed class Sqlite3ConflictChangesetIterator : Sqlite3ChangesetIterator
    {
        private Sqlite3ConflictChangesetIterator() : base() { }
        public Sqlite3ConflictChangesetIterator(IntPtr handle) : this()
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            // sqlite3changeset_finalize should not be called on Sqlite3ChangesetIterator
            // passed to conflict handler, so we override ReleaseHandle to not call it
            handle = IntPtr.Zero;
            return true;
        }
    }

    /// <summary>
    /// typesafe wrapper for returned change set buffer
    /// must be free'd using sqlite3_free to prevent memory leaks
    /// </summary>
    public sealed class Sqlite3ChangesetBuffer : SafeHandle
    {
        private Sqlite3ChangesetBuffer() : base(IntPtr.Zero, true) { }
        public Sqlite3ChangesetBuffer(IntPtr buffer) : this()
        {
            SetHandle(buffer);
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        [DllImport("sqlite3", EntryPoint = "sqlite3_free", CallingConvention = CallingConvention.Cdecl)]
        private static extern void SQLiteFree(IntPtr buffer);
        protected override bool ReleaseHandle()
        {
            SQLiteFree(handle);
            handle = IntPtr.Zero;
            return true;
        }
    }

    /// <summary>
    /// represent a SQLite change set opaque data block
    /// </summary>
    public sealed class SQLiteChangeSet : IDisposable
    {
        public int size { get; private set; } = 0;
        public Sqlite3ChangesetBuffer buffer { get; private set; }

        /// <summary>
        /// Constructor used via SQLite Session module when initially creating change set buffer
        /// </summary>
        /// <param name="size">count of bytes of buffer</param>
        /// <param name="buffer">data used by SQLite Session module to reconstruct tables based on generated change set</param>
        public SQLiteChangeSet(int size, Sqlite3ChangesetBuffer buffer)
        {
            this.size = size;
            this.buffer = buffer;
        }

        /// <summary>
        /// Constructor when recreating change set from previously generated and persisted change set buffer
        /// </summary>
        /// <param name="persistedBuffer">persisted data returned from previous change set generation buffer</param>
        public SQLiteChangeSet(byte[] persistedBuffer)
        {
            this.size = 0;
            this.buffer = new Sqlite3ChangesetBuffer(IntPtr.Zero);

            if (persistedBuffer.Length > 0)
            {
                IntPtr nativeBuffer = SQLiteMalloc(persistedBuffer.Length);
                // if malloc succeeded (not NULL) then copy managed data to sqlite's unmanaged memory buffer
                if (nativeBuffer != IntPtr.Zero)
                {
                    this.size = persistedBuffer.Length;
                    Marshal.Copy(persistedBuffer, 0, nativeBuffer, this.size);
                    this.buffer = new Sqlite3ChangesetBuffer(nativeBuffer);
                }
            }
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3_malloc", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SQLiteMalloc(int size);

        /// <summary>
        /// make a copy of data as managed byte array
        /// </summary>
        /// <returns>managed byte[] copy of sqlite's change set buffer</returns>
        public byte[] ToByteArray()
        {
            var result = new byte[size];
            if (size > 0)
            {
                Marshal.Copy(buffer.DangerousGetHandle(), result, 0, size);
            }
            return result;
        }

        /// <summary>
        /// ensure unmanaged (SQLite malloc'd) memory is released
        /// </summary>
        public void Dispose()
        {
            size = 0;
            buffer.Dispose();
        }
    }

    /// <summary>
    /// wraps the generic sqlite3_value used to return data from db
    /// 
    /// Can either immediately extract and cache value from a sqlite3_value in object
    /// or be used to directly extract value from sqlite3_value 
    /// Note: when used directly should pay attention to which other function invalidate sqlite3_value, etc.
    /// </summary>
    public class SQLiteValue
    {
        private object value = null;
        public SQLiteValue(IntPtr? sqlite3_value)
        {
            SetValue(sqlite3_value);
        }

        public void SetValue(IntPtr? sqlite3_value)
        {
            value = GetValue(sqlite3_value);
        }

        public bool HasValue()
        {
            return value != null;
        }

        public object GetValue()
        {
            return value;
        }



        public static object GetValue(IntPtr? sqlite3_value)
        {
            if (sqlite3_value == null) return null;
            if (sqlite3_value == IntPtr.Zero) return null;
            var value = (IntPtr)sqlite3_value;

            switch (ValueType(value))
            {
                case ColType.Integer:
                    return ValueAsInt64(value);
                case ColType.Float:
                    return ValueAsDouble(value);
                case ColType.Text:
                    return ValueAsString(value);
                case ColType.Blob:
                    return ValueAsByteArray(value);
                case ColType.Null:
                    return null;
                default:
                    throw new ArgumentException("sqlite3_value does not refer to valid value!");
            }
        }


        [DllImport("sqlite3", EntryPoint = "sqlite3_value_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern ColType ValueType(IntPtr sqlite3_value);

        [DllImport("sqlite3", EntryPoint = "sqlite3_value_numeric_type", CallingConvention = CallingConvention.Cdecl)]
        public static extern ColType ValueNumericType(IntPtr sqlite3_value);


        [DllImport("sqlite3", EntryPoint = "sqlite3_value_bytes", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ValueAsByteArraySize(IntPtr sqlite3_value);

        public static byte[] ValueAsByteArray(IntPtr sqlite3_value)
        {
            int length = ValueAsByteArraySize(sqlite3_value);
            var result = new byte[length];
            if (length > 0)
                Marshal.Copy(ValueAsBlob(sqlite3_value), result, 0, length);
            return result;
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3_value_blob", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ValueAsBlob(IntPtr sqlite3_value);

        public static string ValueAsString(IntPtr sqlite3_value)
        {
            return Marshal.PtrToStringUni(ValueAsText16(sqlite3_value));
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3_value_text16", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr ValueAsText16(IntPtr sqlite3_value);

        [DllImport("sqlite3", EntryPoint = "sqlite3_value_double", CallingConvention = CallingConvention.Cdecl)]
        public static extern double ValueAsDouble(IntPtr sqlite3_value);
        [DllImport("sqlite3", EntryPoint = "sqlite3_value_int", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 ValueAsInt32(IntPtr sqlite3_value);
        [DllImport("sqlite3", EntryPoint = "sqlite3_value_int64", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int64 ValueAsInt64(IntPtr sqlite3_value);
    }


    /// <summary>
    /// wraps SQLite Session module
    /// </summary>
    public static class SQLiteSession
    {
        public enum EnableState : int
        {
            QuerySessionIsEnabled = -1,  // any negative value is no-op and may be used to query enabled state
            DisableSession = 0,
            EnableSession = 1 // any positive value enables session, returns 1 if enabled on query/update of enabled state
        }

        public enum ActionCode : int
        {
            SQLITE_UNDEFINED = 0, /* Note: undefined is not a valid value in SQLite source */
            SQLITE_CREATE_INDEX = 1,
            SQLITE_CREATE_TABLE = 2,
            SQLITE_CREATE_TEMP_INDEX = 3,
            SQLITE_CREATE_TEMP_TABLE = 4,
            SQLITE_CREATE_TEMP_TRIGGER = 5,
            SQLITE_CREATE_TEMP_VIEW = 6,
            SQLITE_CREATE_TRIGGER = 7,
            SQLITE_CREATE_VIEW = 8,
            SQLITE_DELETE = 9,
            SQLITE_DROP_INDEX = 10,
            SQLITE_DROP_TABLE = 11,
            SQLITE_DROP_TEMP_INDEX = 12,
            SQLITE_DROP_TEMP_TABLE = 13,
            SQLITE_DROP_TEMP_TRIGGER = 14,
            SQLITE_DROP_TEMP_VIEW = 15,
            SQLITE_DROP_TRIGGER = 16,
            SQLITE_DROP_VIEW = 17,
            SQLITE_INSERT = 18,
            SQLITE_PRAGMA = 19,
            SQLITE_READ = 20,
            SQLITE_SELECT = 21,
            SQLITE_TRANSACTION = 22,
            SQLITE_UPDATE = 23,
            SQLITE_ATTACH = 24,
            SQLITE_DETACH = 25,
            SQLITE_ALTER_TABLE = 26,
            SQLITE_REINDEX = 27,
            SQLITE_ANALYZE = 28,
            SQLITE_CREATE_VTABLE = 29,
            SQLITE_DROP_VTABLE = 30,
            SQLITE_FUNCTION = 31,
            SQLITE_SAVEPOINT = 32,
            //SQLITE_COPY = 0,   /* No longer used */
            SQLITE_RECURSIVE = 33
        }

        public enum ConflictReason : int
        {
            SQLITE_CHANGESET_DATA = 1,
            SQLITE_CHANGESET_NOTFOUND = 2,
            SQLITE_CHANGESET_CONFLICT = 3,
            SQLITE_CHANGESET_CONSTRAINT = 4,
            SQLITE_CHANGESET_FOREIGN_KEY = 5
        }

        public enum ConflictResolution : int
        {
            SQLITE_CHANGESET_OMIT = 0,
            SQLITE_CHANGESET_REPLACE = 1,
            SQLITE_CHANGESET_ABORT = 2
        }

        //IntPtr sqlite3_session;
        //IntPtr sqlite3_changegroup;
        //IntPtr sqlite3_changeset_iter;

        #region session management

        /// <summary>
        /// Attach a secondary database to current db as "name"
        /// primary database is always attached as "main"
        /// See detachDB()
        /// </summary>
        /// <param name="db"></param>
        /// <param name="dbPath"></param>
        /// <param name="name"></param>
        public static void AttachDB(this SQLite.SQLiteConnection db, string dbPath, string name)
        {
            db.Execute($"ATTACH DATABASE ?1 AS ?2;", dbPath, name);
        }

        /// <summary>
        /// Removes secondary database "name" from current db
        /// See attachDB()
        /// </summary>
        /// <param name="db"></param>
        /// <param name="name"></param>
        public static void DetachDB(this SQLite.SQLiteConnection db, string name)
        {
            db.Execute($"DETACH DATABASE ?1;", name);
        }

        /// <summary>
        /// Convenience attached method to create a new session for current database connection
        /// See SQLiteSession.Create()
        /// </summary>
        /// <param name="db">open SQLite database handle</param>
        /// <param name="zDb">which db to attach session to, e.g. main or temp</param>
        /// <param name="session">returns session handle on successful call</param>
        /// <returns>results of session creation; Result.OK if successful, otherwise corresponding error code</returns>
        public static Result CreateSession(this SQLite.SQLiteConnection db, string zDb, out IntPtr session)
        {
            return Create(db, zDb, out session);
        }

        /// <summary>
        /// Convenience attached method to create a new session for current database connection,
        /// attach specified table or all tables,
        /// and optionally enable watching for changes
        /// </summary>
        /// <param name="db">open SQLite database handle</param>
        /// <param name="zDb">which db to attach session to, e.g. main or temp</param>
        /// <param name="session">returns session handle on successful call</param>
        /// <param name="zTab">which table to watch, use null for all; defaults to null</param>
        /// <param name="bEnable">default is EnableState.EnableSession to begin watching for changes</param>
        /// <returns>Result.OK if successful, otherwise corresponding error code</returns>
        public static Result CreateSessionAndAttachTable(this SQLite.SQLiteConnection db, string zDb, out IntPtr session, string zTab = null, EnableState bEnable = EnableState.EnableSession)
        {
            // create a session for "zDb" database
            var result = Create(db, zDb, out session);
            if (result != Result.OK) return result;

            // attach table
            // if null then attach to all tables [within db specified when session created]
            result = AttachToTable(session, zTab);
            if (result != Result.OK) return result;

            // enable watching for changes with this session
            if (Enable(session, bEnable) != bEnable) return Result.Error;

            return result;
        }


        /// <summary>
        /// create a new session
        /// Note: multiple sessions may be opened on the same db; sessions should be deleted prior to closing db
        /// See https://sqlite.org/session/sqlite3session_create.html
        /// </summary>
        /// <param name="db">open SQLite database handle</param>
        /// <param name="zDb">which db to attach session to, e.g. main or temp</param>
        /// <param name="session">returns session handle on successful call</param>
        /// <returns>results of session creation; Result.OK if successful, otherwise corresponding error code</returns>
        public static Result Create(SQLite.SQLiteConnection db, string zDb, out IntPtr session)
        {
            Sqlite3DatabaseHandle dbHandle = db?.Handle ?? IntPtr.Zero;
            if (string.IsNullOrWhiteSpace(zDb) || (dbHandle == IntPtr.Zero))
            {
                session = IntPtr.Zero;
                return Result.Misuse;
            }

            return Create(dbHandle, zDb, out session);
        }
        public static Result Create(SQLite.SQLiteConnection db, out IntPtr session)
        {
            return Create(db, "main", out session);
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3session_create", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result Create(Sqlite3DatabaseHandle dbHandle, [MarshalAs(UnmanagedType.LPStr)] string zDb, out IntPtr session);


        /// <summary>
        /// 
        /// See https://sqlite.org/session/sqlite3session_delete.html
        /// </summary>
        /// <param name="session"></param>
        [DllImport("sqlite3", EntryPoint = "sqlite3session_delete", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Delete(IntPtr session);


        /// <summary>
        /// Query, enable, or disable session change tracking
        /// See https://sqlite.org/session/sqlite3session_enable.html
        /// </summary>
        /// <param name="session">the session to query or update enabled state</param>
        /// <param name="bEnable">action to do, query, disable, or enable session change tracking</param>
        /// <returns>current enabled state, use query to check without changing current state</returns>
        [DllImport("sqlite3", EntryPoint = "sqlite3session_enable", CallingConvention = CallingConvention.Cdecl)]
        public static extern EnableState Enable(IntPtr session, EnableState bEnable);


        /// <summary>
        /// Attaches session to table to begin tracking changes for the table.
        /// Note: if zTab is null then changes to all current and newly added tables with a primary key are tracked
        /// See https://sqlite.org/session/sqlite3session_attach.html
        /// </summary>
        /// <param name="session">previously opened session handle</param>
        /// <param name="zTab">name of Table to track changes on or null for all tables</param>
        /// <returns></returns>
        [DllImport("sqlite3", EntryPoint = "sqlite3session_attach", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result AttachToTable(IntPtr session, [MarshalAs(UnmanagedType.LPStr)] string zTab);


        /// <summary>
        /// Returns if session has recorded no changes
        /// Note: may return false and still be no changes if changes are done that revert themselves.
        /// See https://sqlite.org/session/sqlite3session_isempty.html
        /// </summary>
        /// <param name="session">previously opened session handle</param>
        /// <returns>true if session is definitely empty, false if session is likely not empty</returns>
        [DllImport("sqlite3", EntryPoint = "sqlite3session_isempty", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsEmptySession(IntPtr session);


        /// <summary>
        /// Update session with changes such that if the changes are applied then
        /// table tbl in fromDb will be the same as table tbl in db session created with (toDb)
        /// i.e. if session created with CreateSession("main", out session); AttachToTable(session, null);
        /// calling AddTableDiffToSession(session, "replica", "sometable", ...)
        /// then session will have changes (inserts/updates/deletes) so that if applied to
        /// replica.sometable then it would have same data as main.sometable
        /// Note: swap CreateSession with "replica" and AddTableDiffToSession "main" for inverse,
        /// i.e. changes so main.sometable will have same data as replica.sometable
        /// (Session may be created for db other than "main" so can be diff'd to "main")
        /// See https://sqlite.org/session/sqlite3session_diff.html
        /// </summary>
        /// <param name="session">previously opened session handle</param>
        /// <param name="fromDb">attached secondary database to diff with</param>
        /// <param name="tbl">which table in two databases to compare for diff</param>
        /// <param name="errMsg">set to English error message, if possible, on any error; otherwise is null</param>
        /// <returns></returns>
        public static Result AddTableDiffToSession(IntPtr session, string fromDb, string tbl, out string errMsg)
        {
            var result = AddTableDiffToSession(session, fromDb, tbl, out Sqlite3DatabaseHandle pzErrMsg);
            if ((result != Result.OK) && (pzErrMsg != IntPtr.Zero))
            {
                errMsg = Marshal.PtrToStringAnsi(pzErrMsg);
            }
            else
            {
                errMsg = null;
            }
            return result;
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3session_diff", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result AddTableDiffToSession(IntPtr session,
                                                          [MarshalAs(UnmanagedType.LPStr)] string fromDb,
                                                          [MarshalAs(UnmanagedType.LPStr)] string tbl,
                                                          out IntPtr errMsg);

        #endregion // session management

        #region change set

        /// <summary>
        /// returns a handle used to iterate over all changes to a session since tracking began
        /// See https://sqlite.org/session/sqlite3session_changeset.html
        /// </summary>
        /// <param name="session">open session handle currently tracking changes</param>
        /// <param name="changeSet">buffer struct holding the returned change set</param>
        /// <returns></returns>
        public static Result GenerateChangeSet(IntPtr session, out SQLiteChangeSet changeSet)
        {
            // default to no ChangeSet (returned if any errors)
            changeSet = new SQLiteChangeSet(0, new Sqlite3ChangesetBuffer(IntPtr.Zero));

            if (session == IntPtr.Zero)
            {
                return Result.Misuse;
            }

            try
            {
                var result = GenerateChangeSet(session, out int size, out Sqlite3ChangesetBuffer buffer);
                if (result == Result.OK) changeSet = new SQLiteChangeSet(size, buffer);
                return result;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return Result.Internal;
            }
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3session_changeset", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result GenerateChangeSet(IntPtr session, out int changeSetBufferSize, out Sqlite3ChangesetBuffer changeSetBuffer);


        /// <summary>
        /// Obtain a handle to allow iterating through changes in a change set
        /// See https://sqlite.org/session/sqlite3changeset_start.html
        /// </summary>
        /// <param name="iter">handle returned for iteratoring changes</param>
        /// <param name="changeSet">the change set to iterate through</param>
        /// <returns></returns>
        public static Result ChangeSetStart(out Sqlite3ChangesetIterator iter, SQLiteChangeSet changeSet)
        {
            return ChangeSetStart(out iter, changeSet.size, changeSet.buffer);
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3changeset_start", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result ChangeSetStart(out Sqlite3ChangesetIterator iter, int changeSetBufferSize, Sqlite3ChangesetBuffer changeSetBuffer);

        /// <summary>
        /// 
        /// See https://sqlite.org/session/sqlite3changeset_next.html
        /// </summary>
        /// <param name="iter"></param>
        /// <returns></returns>
        [DllImport("sqlite3", EntryPoint = "sqlite3changeset_next", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result ChangeSetNext(Sqlite3ChangesetIterator iter);

#if false // called automatically by Sqlite3ChangesetIterator Dispose
        /// <summary>
        /// 
        /// See https://sqlite.org/session/sqlite3changeset_finalize.html
        /// </summary>
        /// <param name="iter"></param>
        /// <returns></returns>
        [DllImport("sqlite3", EntryPoint = "sqlite3changeset_finalize", CallingConvention = CallingConvention.Cdecl)]
        public static extern Result ChangeSetFinalize(Sqlite3ChangesetIterator iter);
#endif

        /// <summary>
        /// Given a changeset, prepares the inverse such that applying In then Out change sets
        /// will give the equivalent of no changes to the database.
        /// See https://sqlite.org/session/sqlite3changeset_invert.html
        /// </summary>
        /// <param name="changeSetIn">existing change set</param>
        /// <param name="changeSetOut">inverse of In change set</param>
        /// <returns>result status of inversion; Result.OK if successful, otherwise corresponding error code</returns>
        public static Result InvertChangeSet(SQLiteChangeSet changeSetIn, out SQLiteChangeSet changeSetOut)
        {
            if (changeSetIn.buffer.IsInvalid)
            {
                changeSetOut = new SQLiteChangeSet(0, new Sqlite3ChangesetBuffer(IntPtr.Zero));
                return Result.Misuse;
            }

            var result = InvertChangeSet(changeSetIn.size, changeSetIn.buffer, out int size, out Sqlite3ChangesetBuffer buffer);
            changeSetOut = new SQLiteChangeSet(size, buffer);
            return result;
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3changeset_invert", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result InvertChangeSet(int changeSetBufferSizeIn, Sqlite3ChangesetBuffer changeSetBufferIn,
                                                     out int changeSetBufferSizeOut, out Sqlite3ChangesetBuffer changeSetBufferOut);


        /// <summary>
        /// Applies a given change set to provided db
        /// with possibility to limit which tables are changed via xFilter delegate,
        /// and custom conflict resolution via xConflict delegate
        /// See https://sqlite.org/session/sqlite3changeset_apply.html
        /// </summary>
        /// <param name="db">which db ("main" only) to apply changes to</param>
        /// <param name="changeSet">change set to apply to db</param>
        /// <param name="xFilter">use null to not filter, else delegate to limit tables changes applied to</param>
        /// <param name="xConflict">use null to ignore conflicts, else delegate to handle conflicts</param>
        /// <param name="ctx">context passed as first argument to xFilter and xConflict delegates</param>
        /// <returns></returns>
        public static Result ChangeSetApply(SQLite.SQLiteConnection db, SQLiteChangeSet changeSet, FilterCallback xFilter, ConflictCallback xConflict, object ctx)
        {
            Sqlite3DatabaseHandle dbHandle = db?.Handle ?? IntPtr.Zero;
            if (dbHandle == IntPtr.Zero)
            {
                return Result.Misuse;
            }

            if (xConflict == null)
            {
                xConflict = new ConflictCallback(CallbackIgnoreConflicts);
            }

            // pinning not needed since just passing back thru; see https://blogs.msdn.microsoft.com/jmstall/2006/10/09/gchandle-tointptr-vs-gchandle-addrofpinnedobject/
            // Warning: if conflict handler is in unmanaged code then ctx should be pinned in Alloc and use AddrOfPinnedObject instead of ToIntPtr
            GCHandle gch = GCHandle.Alloc(ctx); // ok if ctx is null; pinning is unneeded since not kept past ChangeSetApply call
            IntPtr pCtx = (ctx == null) ? IntPtr.Zero : GCHandle.ToIntPtr(gch); // we don't pass GCHandle wrapper if null, instead pass NULL
            var result = ChangeSetApply(dbHandle, changeSet.size, changeSet.buffer, xFilter, xConflict, pCtx);
            gch.Free();

            return result;
        }
        public delegate bool FilterCallback(IntPtr pCtx, [MarshalAs(UnmanagedType.LPStr)] string tableName); // false means do not apply changes
        public delegate ConflictResolution ConflictCallback(IntPtr pCtx, ConflictReason eConflict, IntPtr iter);
        [DllImport("sqlite3", EntryPoint = "sqlite3changeset_apply", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result ChangeSetApply(Sqlite3DatabaseHandle db, int changeSetBufferSize, Sqlite3ChangesetBuffer changeSetBuffer,
                                                    [MarshalAs(UnmanagedType.FunctionPtr)]FilterCallback xFilter,
                                                    [MarshalAs(UnmanagedType.FunctionPtr)]ConflictCallback xConflict,
                                                    IntPtr pCtx);
        public static Result ApplySessionChangeSet(this SQLite.SQLiteConnection db, SQLiteChangeSet changeSet, FilterCallback xFilter, ConflictCallback xConflict, object ctx)
        {
            return ChangeSetApply(db, changeSet, xFilter, xConflict, ctx);
        }

        /// <summary>
        /// Default conflict handler if null is specified, ignores all conflicts and makes no changes
        /// </summary>
        /// <param name="ctx">unused</param>
        /// <param name="eConflict">reason for callback</param>
        /// <param name="iter">conflict change set, may iterate through values</param>
        /// <returns>always returns SQLITE_CHANGESET_OMIT</returns>
        public static ConflictResolution CallbackIgnoreConflicts(IntPtr pCtx, ConflictReason eConflict, IntPtr iter)
        {
            return ConflictResolution.SQLITE_CHANGESET_OMIT;
        }

#pragma warning disable RCS1163 // Unused parameter.
        /// <summary>
        /// Alternate common conflict handler, always overwrites changes with new value on conflict.
        /// Returns no change if conflict is not due to conflict or data issue.
        /// </summary>
        /// <param name="ctx">unused</param>
        /// <param name="eConflict">reason for callback</param>
        /// <param name="pIter">conflict change set, may iterate through values</param>
        /// <returns>SQLITE_CHANGESET_REPLACE indicating overwrite with conflicting value</returns>
        public static ConflictResolution CallbackReplaceOnConflicts(IntPtr pCtx, ConflictReason eConflict, IntPtr pIter)
#pragma warning restore RCS1163 // Unused parameter.
        {
#if false // example code only            
            object ctx = null;
            if (pCtx != IntPtr.Zero)
            {
                GCHandle gch = GCHandle.FromIntPtr(pCtx);
                ctx = gch.Target;
            }
            var myCtx = ctx as string;

            // Note: pIter should be converted to a Sqlite3ChangesetIterator to allow using ChangeSetOp,ChangeSet*Value methods
            // however must actually create Sqlite3ConflictChangesetIterator to avoid unnecessary finalize of iterator
            var iter = new Sqlite3ConflictChangesetIterator(pIter);

            // get table, column, and other information about this change
            string table;
            int columnCount;
            ActionCode op;
            bool indirect;
            ChangeSetOp(iter, out table, out columnCount, out op, out indirect);

            SQLiteValue value;
            for (int i = 0; i < columnCount; i++)
            {
                // on conflict with op=SQLITE_INSERT newvalue is the value to use on REPLACE, conflictvalue is current value (unchanged if omit)
                // oldvalue is unused with op=SQLITE_INSERT
                if (ChangeSetNewValue(iter, i, out value) == SQLite3.Result.OK)
                {
                    object o = value.GetValue();
                }
                if (ChangeSetOldValue(iter, i, out value) == SQLite3.Result.OK)
                {
                    object o = value.GetValue();
                }
                if (ChangeSetConflictValue(iter, i, out value) == SQLite3.Result.OK)
                {
                    object o = value.GetValue();
                }
            }
#endif

            // replace only valid on conflict or data
            if ((eConflict == ConflictReason.SQLITE_CHANGESET_CONFLICT) || (eConflict == ConflictReason.SQLITE_CHANGESET_DATA))
            {
                return ConflictResolution.SQLITE_CHANGESET_REPLACE;
            }
            else
            {
                return ConflictResolution.SQLITE_CHANGESET_OMIT;
            }
        }



        /// <summary>
        /// 
        /// See https://sqlite.org/session/sqlite3changeset_op.html
        /// </summary>
        /// <param name="iter"></param>
        /// <param name="zTab"></param>
        /// <param name="nCol"></param>
        /// <param name="op"></param>
        /// <param name="indirect"></param>
        /// <returns></returns>
        public static Result ChangeSetOp(Sqlite3ChangesetIterator iter, out string tab, out int nCol, out ActionCode op, out bool indirect)
        {
            tab = null;
            var result = ChangeSetOp(iter, out Sqlite3DatabaseHandle pzTab, out nCol, out op, out int _indirect);
            if (result == Result.OK)
            {
                indirect = _indirect != 0; // 0=false, 1=true (other values are undefined but assumed true)
                tab = Marshal.PtrToStringAnsi(pzTab);  // Note: pzTab is reference (pointer) to char * buffer of _UTF8_ table name
            }
            else  // on error these values are undefined, so set to known values
            {
                indirect = false;
                op = ActionCode.SQLITE_UNDEFINED;
                tab = null;
            }
            return result;
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3changeset_op", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result ChangeSetOp(Sqlite3ChangesetIterator iter, out IntPtr tab, out int nCol, out ActionCode op, out int indirect);


        /// <summary>
        /// 
        /// See https://sqlite.org/session/sqlite3changeset_pk.html
        /// </summary>
        /// <param name="iter"></param>
        /// <param name="pk"></param>
        /// <returns></returns>
        public static Result ChangeSetPrimaryKey(Sqlite3ChangesetIterator iter, out bool[] pk)
        {
            var result = ChangeSetPrimaryKey(iter, out Sqlite3DatabaseHandle pbPK, out int nCol);

            var bPk = new byte[nCol];
            Marshal.Copy(pbPK, bPk, 0, nCol);

            pk = new bool[nCol];
            for (int i = 0; i < nCol; i++)
                pk[i] = (bPk[i] != 0);

            return result;
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3changeset_pk", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result ChangeSetPrimaryKey(Sqlite3ChangesetIterator iter, out IntPtr pbPK, out int nCol);


        /// <summary>
        /// Convenience method that determines which columns in the current iter row are
        /// primary key values and returns a list of them (order should match CREATE order)
        /// </summary>
        /// <param name="iter"></param>
        /// <returns>a List of Tuple with sql column value and column index for each primary key column; 
        /// empty List if no primary keys or error</returns>
        public static List<Tuple<SQLiteValue, int>> ChangeSetPrimaryKeyValues(Sqlite3ChangesetIterator iter)
        {
            if (ChangeSetPrimaryKey(iter, out bool[] pk) != Result.OK)
            {
                System.Diagnostics.Debug.WriteLine("Failed to obtain primary key bool arrary.");
                return new List<Tuple<SQLiteValue, int>>(0);
            }

            return ChangeSetPrimaryKeyValues(iter, pk);
        }

        /// <summary>
        /// Convenience method that determines which columns in the current iter row are
        /// primary key values and returns a list of them (order should match CREATE order)
        /// </summary>
        /// <param name="iter"></param>
        /// <param name="pk">an array of bool indicating if column at matching index is a primary key value</param>
        /// <returns>a List of Tuple with sql column value and column index for each primary key column; 
        /// empty List if no primary keys or error</returns>
        public static List<Tuple<SQLiteValue, int>> ChangeSetPrimaryKeyValues(Sqlite3ChangesetIterator iter, bool[] pk)
        {
            /* Note: pk.Length should be same as number of columns (may be less if schema updated to add more columns)
             * We assume [because SQLite appears to work this way] that there is a 1-to-1 mapping between
             * each element in pk and columns in table schema, with order matching order of CREATE statement
             */

            var colValues = new List<Tuple<SQLiteValue, int>>(1);  // may be up to pk.Length size, but usually only a single pk column

            // cycle through pk array and obtain value if column is a primary key, add it to our List
            for (int i = 0; i < pk.Length; i++)
            {
                if (pk[i])
                {
                    // instead of passing in op we try in succession to obtain primary key value
                    // value if UPDATE op, Note: errors are ignored an instead try to get from next source
                    if (ChangeSetConflictValue(iter, i, out SQLiteValue value) != Result.OK)
                    {
                        // value if INSERT op
                        if (ChangeSetNewValue(iter, i, out value) != Result.OK)
                        {
                            // value if DELETE op
                            if (ChangeSetOldValue(iter, i, out value) != Result.OK) // valid on DELETE
                            {
                                System.Diagnostics.Debug.WriteLine($"Failed to obtain primary key value for column {i}.");
                                return new List<Tuple<SQLiteValue, int>>(0);
                            }
                        }
                    }
                    // value is primary key's value, i is column of this key component
                    colValues.Add(new Tuple<SQLiteValue, int>(value, i));
                }
            }

            return colValues;
        }


        /// <summary>
        /// 
        /// See https://sqlite.org/session/sqlite3changeset_new.html
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iter"></param>
        /// <param name="iVal"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Result ChangeSetNewValue(Sqlite3ChangesetIterator iter, int iVal, out SQLiteValue value)
        {
            var result = ChangeSetNewValue(iter, iVal, out Sqlite3DatabaseHandle pValue);
            value = new SQLiteValue(pValue);
            return result;
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3changeset_new", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result ChangeSetNewValue(Sqlite3ChangesetIterator iter, int iVal, out IntPtr pValue);


        /// <summary>
        /// 
        /// See https://sqlite.org/session/sqlite3changeset_old.html
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iter"></param>
        /// <param name="iVal"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Result ChangeSetOldValue(Sqlite3ChangesetIterator iter, int iVal, out SQLiteValue value)
        {
            var result = ChangeSetOldValue(iter, iVal, out Sqlite3DatabaseHandle pValue);
            value = new SQLiteValue(pValue);
            return result;
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3changeset_old", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result ChangeSetOldValue(Sqlite3ChangesetIterator iter, int iVal, out IntPtr pValue);


        /// <summary>
        /// 
        /// See https://sqlite.org/session/sqlite3changeset_conflict.html
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="iter"></param>
        /// <param name="iVal"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Result ChangeSetConflictValue(Sqlite3ChangesetIterator iter, int iVal, out SQLiteValue value)
        {
            var result = ChangeSetConflictValue(iter, iVal, out Sqlite3DatabaseHandle pValue);
            value = new SQLiteValue(pValue);
            return result;
        }
        [DllImport("sqlite3", EntryPoint = "sqlite3changeset_conflict", CallingConvention = CallingConvention.Cdecl)]
        private static extern Result ChangeSetConflictValue(Sqlite3ChangesetIterator iter, int iVal, out IntPtr pValue);


        /// <summary>
        /// Convenience method to obtain the current (conflict), old, and new value for specified column 
        /// in current conflicting row of iter.
        /// Warning! undefined if called when ConflictReason anything other than SQLITE_CHANGESET_CONFLICT || SQLITE_CHANGESET_DATA
        /// </summary>
        /// <param name="iter">the conflict Iterator, only queried not advanced</param>
        /// <param name="op">is the conflict a delete, insert, or update</param>
        /// <param name="column">which column to get values from, index >=0 and less than count returned by ChangeSetOp</param>
        /// <param name="currentValue">existing value (conflicting value)</param>
        /// <param name="oldValue">null on insert, otherwise ???</param>
        /// <param name="newValue">null on delete, otherwise ???</param>
        /// <returns>current, old, & new value of column for current row if available, null otherwise</returns>
        public static SQLite3.Result GetConflictValues(ref Sqlite3ConflictChangesetIterator iter, ActionCode op, int column,
                                                       out object currentValue, out object oldValue, out object newValue)
        {
            currentValue = null;
            oldValue = null;
            newValue = null;

            // on conflict with op=SQLITE_INSERT 
            //      newvalue is the value to use on REPLACE, 
            //      conflictvalue is current value (unchanged if omit)
            //      oldvalue is unused with op=SQLITE_INSERT
            // on conflict with op=SQLITE_DELETE
            //      newvalue is unused, 
            //      conflictvalue is current value (unchanged if omit)
            //      oldvalue is the value of deleted row (neither is used on REPLACE, ie delete succeeds)

            // Note: ConflictValue is only valid if conflict changeset, new/old are valid for other changesets
            // can be called for non-conflict changeset, but currentValue will always be null as call will always fail
            if (ChangeSetConflictValue(iter, column, out SQLiteValue value) == SQLite3.Result.OK)
            {
                if (value.HasValue()) currentValue = value.GetValue();
            }
            if (op == ActionCode.SQLITE_INSERT || op == ActionCode.SQLITE_UPDATE)
            {
                if (ChangeSetNewValue(iter, column, out value) == SQLite3.Result.OK)
                {
                    if (value.HasValue()) newValue = value.GetValue();
                }
            }
            if (op == ActionCode.SQLITE_DELETE || op == ActionCode.SQLITE_UPDATE)
            {
                if (ChangeSetOldValue(iter, column, out value) == SQLite3.Result.OK)
                {
                    if (value.HasValue()) oldValue = value.GetValue();
                }
            }

            return SQLite3.Result.OK;
        }

        #endregion // change set

        #region change group

        #endregion // change group
    }
}
