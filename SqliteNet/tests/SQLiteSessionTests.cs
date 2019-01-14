// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace Tems_Inventory.Tests
{
    using NUnit.Framework;
    using SQLite;
    using SQLiteNetSessionModule;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;

    [TestFixture]
    public class SQLiteSessionsTest
    {
        private const string dbPrimaryPath = @"DBSessionsTestPrimary.db";
        private const string dbReplicaPath = @"DBSessionsTestReplica.db";

        // maintains our connection to the SQLite database
        private SQLiteConnection dbPrimary = null;
        private SQLiteConnection dbReplica = null;

        // session used during test
        private IntPtr session = IntPtr.Zero;

        class TestTable1
        {
            [PrimaryKey, AutoIncrement]
            public int pk { get; set; }
            [NotNull]
            public string myString { get; set; }
            [NotNull]
            public int myInt { get; set; }
            [NotNull]
            public DateTime myDate { get; set; }
            [NotNull] //[ForeignKey(typeof(TestTable2))]
            public Guid myTable2ObjectId { get; set; }
            [SQLite.Ignore] //[ManyToOne(foreignKey: "myTable2ObjectId", CascadeOperations = CascadeOperation.CascadeRead | CascadeOperation.CascadeInsert)]
            public TestTable2 myTable2Object
            {
                get { return _myTable2Object; }
                set { _myTable2Object = value; myTable2ObjectId = value.pk; }
            }
            private TestTable2 _myTable2Object;
        }
        class TestTable2
        {
            [PrimaryKey]
            public Guid pk { get; set; }
            public string anotherString { get; set; }
            public int anotherInt { get; set; }
            public DateTime anotherDate { get; set; }
        }

        TestTable1 sampleData = new TestTable1
        {
            //pk = 1,
            myString = "Sample row #1.",
            myInt = 1,
            myDate = DateTime.Now,
            myTable2Object = new TestTable2
            {
                pk = Guid.NewGuid(),
                anotherString = "Related row #1.",
                anotherInt = 11,
                anotherDate = DateTime.Now.AddYears(-5)
            }
        };
        TestTable1 sampleData2 = new TestTable1
        {
            pk = -2,
            myString = "Sample row #2.",
            myInt = 2,
            myDate = DateTime.Now,
        };

        /// <summary>
        /// create a database with test table structure
        /// and optionally add some initial data to it
        /// </summary>
        /// <param name="dbPath">filename of database to create</param>
        /// <param name="addSampleData">true to add initial data, false to leave unpopulated</param>
        /// <returns></returns>
        private SQLiteConnection GetDB(string dbPath, bool addSampleData)
        {
            // WARNING this will overwrite db if it already exists - i.e. fresh start
            if (File.Exists(dbPath))
                (new FileStream(dbPath, FileMode.Create, FileAccess.Write, FileShare.None)).Close();

            var db = new SQLiteConnection(dbPath, true);
            Assert.NotNull(db);

            // create our tables
            db.CreateTable<TestTable2>();
            db.CreateTable<TestTable1>();

            // populate with some data
            if (addSampleData)
            {
                sampleData2.myTable2Object = sampleData.myTable2Object;
                db.InsertOrReplace(sampleData.myTable2Object);
                db.InsertOrReplace(sampleData);
                db.InsertOrReplace(sampleData2);
            }

            return db;
        }

        /// <summary>
        /// loads SQLite and creates and populates our test DB
        /// </summary>
        [SetUp]
        public void SetupTest()
        {
#region add support for native DLLs under both Win32 and Win64
            try
            {
                SQLite.SQLiteLoader.SetNativeDllDirectory();
            }
            catch
            {
                Assert.Fail("Critical error initializing dynamic libraries!  ABORTING!");
            }
#endregion // add support for native DLLs under both Win32 and Win64

            // set current directory to expected value, i.e. directory of binaries, so relative paths for databases work as expected
            // Note: otherwise likely to fail as default current directory may be within protected dir, e.g. Visual Studio install directory
            Directory.SetCurrentDirectory(Path.GetDirectoryName((Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly())?.Location));

            dbPrimary = GetDB(dbPrimaryPath, addSampleData: true);
            dbReplica = GetDB(dbReplicaPath, addSampleData: false);
            Assert.NotNull(dbPrimary);
            Assert.NotNull(dbReplica);
        }

        /// <summary>
        /// deletes test DB and any other cleanup
        /// </summary>
        [TearDown]
        public void CleanupTest()
        {
            if (dbPrimary != null)
            {
                //dbPrimary.DropTable<TestTable1>()
                //dbPrimary.DropTable<TestTable2>()
                dbPrimary.Close();
            }
            if (dbReplica != null)
            {
                //dbReplica.DropTable<TestTable1>();
                //dbReplica.DropTable<TestTable2>();
                dbReplica.Close();
            }
            dbPrimary = null;
            dbReplica = null;
            File.Delete(dbPrimaryPath);
            File.Delete(dbReplicaPath);
            System.Threading.Thread.Sleep(5); // need to give system time to finish delete (adjust as needed to avoid random test failures)
            Assert.IsFalse(File.Exists(dbPrimaryPath), "Primary DB exists");
            Assert.IsFalse(File.Exists(dbReplicaPath), "Replica DB exists");
        }


        // Note: tests are ran in alphabetical order, no order is required, but
        // to help track down errors, tests are named in order to test basic
        // functionality prior to more advanced features.


        [Test, Category("SQLiteSession")]
        public void TestSession01_Setup()
        {
            // should automatically call setupTest() immediately followed by cleanupTest()
        }

        [Test, Category("SQLiteSession")]
        public void TestSession02_CreateandDeleteSession()
        {
            Assert.That(SQLiteSession.Create(dbPrimary, out session), Is.EqualTo(SQLite3.Result.OK));
            SQLiteSession.Delete(session);
        }

        [Test, Category("SQLiteSession")]
        public void TestSession03_WithAttachAndEmptySession()
        {
            Assert.That(SQLiteSession.Create(dbPrimary, out session), Is.EqualTo(SQLite3.Result.OK));

            Assert.That(SQLiteSession.AttachToTable(session, nameof(TestTable1)), Is.EqualTo(SQLite3.Result.OK));
            Assert.That(SQLiteSession.AttachToTable(session, nameof(TestTable2)), Is.EqualTo(SQLite3.Result.OK));

            Assert.That(SQLiteSession.IsEmptySession(session), Is.True);

            SQLiteSession.Delete(session);
        }

        [Test, Category("SQLiteSession")]
        public void TestSession04_WithAttachAllAndEmptySession()
        {
            // create a session for "main" database
            Assert.That(SQLiteSession.Create(dbPrimary, out session), Is.EqualTo(SQLite3.Result.OK));

            // attach all tables to it
            Assert.That(SQLiteSession.AttachToTable(session, null), Is.EqualTo(SQLite3.Result.OK));

            // enable watching for changes with this session
            Assert.That(SQLiteSession.Enable(session, SQLiteSession.EnableState.EnableSession), Is.EqualTo(SQLiteSession.EnableState.EnableSession));

            // validate no changes have occurred
            Assert.That(SQLiteSession.IsEmptySession(session), Is.True);

            // done with our session
            SQLiteSession.Delete(session);
        }

        [Test, Category("SQLiteSession")]
        public void TestSession05_BothDbWithAttachMainTablesAndEmptySession()
        {
            // close replica we just created and attach db file to our primary db as "replica"
            dbReplica.Close();
            dbPrimary.AttachDB(dbReplicaPath, "replica");

            // create a session for "main" database
            //Assert.That(SQLiteSession.Create(dbPrimary, out session), Is.EqualTo(SQLite3.Result.OK));
            Assert.That(dbPrimary.CreateSession("main", out session), Is.EqualTo(SQLite3.Result.OK));

            // attach all tables [from "main"] to it
            Assert.That(SQLiteSession.AttachToTable(session, null), Is.EqualTo(SQLite3.Result.OK));

            // enable watching for changes with this session
            Assert.That(SQLiteSession.Enable(session, SQLiteSession.EnableState.EnableSession), Is.EqualTo(SQLiteSession.EnableState.EnableSession));

            // validate no changes have occurred
            Assert.That(SQLiteSession.IsEmptySession(session), Is.True);

            // remove secondary db
            dbPrimary.DetachDB("replica");

            // done with our session
            SQLiteSession.Delete(session);
        }

        [Test, Category("SQLiteSession")]
        public void TestSession06_BothDbWithAttachMainTablesAndEmptySessionConvenience()
        {
            // close replica we just created and attach db file to our primary db as "replica"
            dbReplica.Close();
            dbPrimary.AttachDB(dbReplicaPath, "replica");

            // create a session for "main" database
            Assert.That(dbPrimary.CreateSessionAndAttachTable("main", out session, zTab: null), Is.EqualTo(SQLite3.Result.OK));

            // validate no changes have occurred
            Assert.That(SQLiteSession.IsEmptySession(session), Is.True);

            // remove secondary db
            dbPrimary.DetachDB("replica");

            // done with our session
            SQLiteSession.Delete(session);
        }

        [Test, Category("SQLiteSession")]
        public void TestSession07_CreateTableDiffForSession()
        {
            // close replica we just created and attach db file to our primary db as "replica"
            dbReplica.Close(); dbReplica = null;
            dbPrimary.AttachDB(dbReplicaPath, "replica");

            // verify that "main" has rows in TestTable1 and "replica" currently empty
            var mainTestTable1Rows = dbPrimary.Query<TestTable1>("Select * FROM main.TestTable1;");
            Assert.That(mainTestTable1Rows.Count, Is.GreaterThan(0));

            var replicaTestTable1Rows = dbPrimary.Query<TestTable1>("Select * FROM replica.TestTable1;");
            Assert.That(replicaTestTable1Rows.Count, Is.EqualTo(0));

            // create a session for "replica" database  <== note replica not main
            Assert.That(dbPrimary.CreateSession("main", out session), Is.EqualTo(SQLite3.Result.OK));

            // attach all tables [from "main"] to it
            Assert.That(SQLiteSession.AttachToTable(session, null), Is.EqualTo(SQLite3.Result.OK));

            // Note: we have not enabled watching for changes

            // validate no changes have occurred
            Assert.That(SQLiteSession.IsEmptySession(session), Is.True);

            // see what it would take to make replica like primary
#pragma warning disable IDE0018 // Inline variable declaration
            string errMsg;
#pragma warning restore IDE0018 // Inline variable declaration
            Assert.That(SQLiteSession.AddTableDiffToSession(session, "replica", nameof(TestTable1), out errMsg), Is.EqualTo(SQLite3.Result.OK));
            Assert.That(SQLiteSession.AddTableDiffToSession(session, "replica", nameof(TestTable2), out errMsg), Is.EqualTo(SQLite3.Result.OK));

            // session should no longer be empty, as tables differ and session should have the diff
            Assert.That(SQLiteSession.IsEmptySession(session), Is.False);

            // remove secondary db
            dbPrimary.DetachDB("replica");

            // done with our session
            SQLiteSession.Delete(session);
        }

        /// <summary>
        /// attach session to "main" and update table with new row
        /// </summary>
        private void MakeAndValidateTableChanges()
        {
            // create a session for "main" database
            Assert.That(dbPrimary.CreateSessionAndAttachTable("main", out session, zTab: null), Is.EqualTo(SQLite3.Result.OK));

            // validate no changes have occurred
            Assert.That(SQLiteSession.IsEmptySession(session), Is.True);

            // verify that "main" has 2 rows in TestTable1
            var mainTestTable1Rows = dbPrimary.Query<TestTable1>("Select * FROM main.TestTable1;");
            Assert.That(mainTestTable1Rows.Count, Is.EqualTo(2));

            // add a row to table
            var newRow = new TestTable1
            {
                myString = "test",
                myInt = 3,
                myDate = DateTime.Now,
                myTable2Object = sampleData.myTable2Object
            };
            Assert.That(dbPrimary.Insert(newRow), Is.EqualTo(1));

            // verify that "main" now has 3 rows in TestTable1
            mainTestTable1Rows = dbPrimary.Query<TestTable1>("Select * FROM main.TestTable1;");
            Assert.That(mainTestTable1Rows.Count, Is.EqualTo(3));

            // session should no longer be empty, as we have inserted a row
            Assert.That(SQLiteSession.IsEmptySession(session), Is.False);
        }

        [Test, Category("SQLiteSession")]
        public void TestSession08_ChangesTracked()
        {
            MakeAndValidateTableChanges();

            // done with our session
            SQLiteSession.Delete(session);
        }

        [Test, Category("SQLiteSession")]
        public void TestSession09_GenerateChangeSet()
        {
            MakeAndValidateTableChanges();

            // create a change set
            Assert.That(SQLiteSession.GenerateChangeSet(session, out SQLiteChangeSet changeSet), Is.EqualTo(SQLite3.Result.OK));
            Assert.That(changeSet.size, Is.GreaterThan(0));

            // done with our session
            SQLiteSession.Delete(session);
        }

        [Test, Category("SQLiteSession")]
        public void TestSession10_IterateChangeSet()
        {
            MakeAndValidateTableChanges();

            // create a change set
            Assert.That(SQLiteSession.GenerateChangeSet(session, out SQLiteChangeSet changeSet), Is.EqualTo(SQLite3.Result.OK));
            using (changeSet)
            {
                Assert.That(changeSet.size, Is.GreaterThan(0));

                // create an iterator so can iterate through change set
                Assert.That(SQLiteSession.ChangeSetStart(out Sqlite3ChangesetIterator iter, changeSet), Is.EqualTo(SQLite3.Result.OK));
                using (iter)
                {
                    SQLite3.Result result;
                    while ((result = SQLiteSession.ChangeSetNext(iter)) == SQLite3.Result.Row)
                    {
                        Assert.That(result, Is.EqualTo(SQLite3.Result.Row));

                        // get table, column, and other information about this change
                        Assert.That(SQLiteSession.ChangeSetOp(iter, out string table, out int columnCount, out SQLiteSession.ActionCode op, out bool indirect), Is.EqualTo(SQLite3.Result.OK));

                        Assert.That(table, Is.EqualTo("TestTable1"));
                        Assert.That(columnCount, Is.GreaterThan(0));
                        Assert.That(op, Is.EqualTo(SQLiteSession.ActionCode.SQLITE_INSERT));
                        Assert.That(indirect, Is.False);

                        // since insert, should be able to get new values inserted
#pragma warning disable IDE0018 // Inline variable declaration
                        SQLiteValue value; // 0th column is primary key
#pragma warning restore IDE0018 // Inline variable declaration
                        Assert.That(SQLiteSession.ChangeSetNewValue(iter, 1, out value), Is.EqualTo(SQLite3.Result.OK));
                        Assert.That(value.GetValue(), Is.EqualTo("test"));  // 1st column = myString
                        Assert.That(SQLiteSession.ChangeSetNewValue(iter, 2, out value), Is.EqualTo(SQLite3.Result.OK));
                        Assert.That(value.GetValue(), Is.EqualTo(3));  // 2nd column = myInt
                        // DateTime is stored as long, WARNING may vary, see storeDateTimeAsTicks when creating db connection
                        // Guid is stored as string

                        // verify we can retrieve all values regardless of type
                        for (int i = 0; i < columnCount; i++)
                        {
                            Assert.That(SQLiteSession.ChangeSetNewValue(iter, i, out value), Is.EqualTo(SQLite3.Result.OK));
                            Assert.That(value.GetValue(), Is.Not.Null);
                        }
                    }
                    Assert.That(result, Is.EqualTo(SQLite3.Result.Done));
                }
                Assert.That(iter.IsInvalid, Is.True);
            }
            Assert.That(changeSet.buffer.IsInvalid, Is.True);

            // done with our session
            SQLiteSession.Delete(session);
        }

        [Test, Category("SQLiteSession")]
        public void TestSession11_IterateChangeSetShowPKValues()
        {
            MakeAndValidateTableChanges();

            // create a change set
            Assert.That(SQLiteSession.GenerateChangeSet(session, out SQLiteChangeSet changeSet), Is.EqualTo(SQLite3.Result.OK));
            using (changeSet)
            {
                Assert.That(changeSet.size, Is.GreaterThan(0));

                // create an iterator so can iterate through change set
                Assert.That(SQLiteSession.ChangeSetStart(out Sqlite3ChangesetIterator iter, changeSet), Is.EqualTo(SQLite3.Result.OK));
                using (iter)
                {
                    SQLite3.Result result;
                    while ((result = SQLiteSession.ChangeSetNext(iter)) == SQLite3.Result.Row)
                    {
                        Assert.That(result, Is.EqualTo(SQLite3.Result.Row));

                        // get table, column, and other information about this change
                        Assert.That(SQLiteSession.ChangeSetOp(iter, out string table, out int columnCount, out SQLiteSession.ActionCode op, out bool indirect), Is.EqualTo(SQLite3.Result.OK));

                        Assert.That(table, Is.EqualTo("TestTable1"));
                        Assert.That(columnCount, Is.GreaterThan(0));
                        Assert.That(op, Is.EqualTo(SQLiteSession.ActionCode.SQLITE_INSERT));
                        Assert.That(indirect, Is.False);

                        // based on insert change
                        var pkValues = SQLiteSession.ChangeSetPrimaryKeyValues(iter);
                        Assert.That(pkValues.Count, Is.EqualTo(1));
                        Assert.That(pkValues[0].Item2, Is.EqualTo(0)); // 0th column is primary key, Item2==column#
                        SQLiteValue value = pkValues[0].Item1; 
                        Assert.That(value.HasValue(), Is.True);
                        System.Diagnostics.Debug.WriteLine($"PK is {value.GetValue()}.");
                    }
                    Assert.That(result, Is.EqualTo(SQLite3.Result.Done));
                }
                Assert.That(iter.IsInvalid, Is.True);
            }
            Assert.That(changeSet.buffer.IsInvalid, Is.True);

            // done with our session
            SQLiteSession.Delete(session);
        }

        [Test, Category("SQLiteSession")]
        public void TestSession12_PersistChangeSet()
        {
            MakeAndValidateTableChanges();

            // create a change set
            byte[] persistedChangeSet;
            Assert.That(SQLiteSession.GenerateChangeSet(session, out SQLiteChangeSet changeSet), Is.EqualTo(SQLite3.Result.OK));
            using (changeSet)
            {
                Assert.That(changeSet.size, Is.GreaterThan(0));

                persistedChangeSet = changeSet.ToByteArray();
                Assert.That(persistedChangeSet.Length, Is.EqualTo(changeSet.size));
            }
            Assert.That(changeSet.buffer.IsInvalid, Is.True);

            using (changeSet = new SQLiteChangeSet(persistedChangeSet))
            {
                Assert.That(changeSet.size, Is.EqualTo(persistedChangeSet.Length));

                // create an iterator so can iterate through change set
                Assert.That(SQLiteSession.ChangeSetStart(out Sqlite3ChangesetIterator iter, changeSet), Is.EqualTo(SQLite3.Result.OK));
                using (iter)
                {
                    SQLite3.Result result;
                    while ((result = SQLiteSession.ChangeSetNext(iter)) == SQLite3.Result.Row)
                    {
                        Assert.That(result, Is.EqualTo(SQLite3.Result.Row));

                        // get table, column, and other information about this change
                        Assert.That(SQLiteSession.ChangeSetOp(iter, out string table, out int columnCount, out SQLiteSession.ActionCode op, out bool indirect), Is.EqualTo(SQLite3.Result.OK));

                        Assert.That(table, Is.EqualTo("TestTable1"));
                        Assert.That(columnCount, Is.GreaterThan(0));
                        Assert.That(op, Is.EqualTo(SQLiteSession.ActionCode.SQLITE_INSERT));
                        Assert.That(indirect, Is.False);

                        // since insert, should be able to get new values inserted
#pragma warning disable IDE0018 // Inline variable declaration
                        SQLiteValue value; // 0th column is primary key
#pragma warning restore IDE0018 // Inline variable declaration
                        Assert.That(SQLiteSession.ChangeSetNewValue(iter, 1, out value), Is.EqualTo(SQLite3.Result.OK));
                        Assert.That(value.GetValue(), Is.EqualTo("test"));  // 1st column = myString
                        Assert.That(SQLiteSession.ChangeSetNewValue(iter, 2, out value), Is.EqualTo(SQLite3.Result.OK));
                        Assert.That(value.GetValue(), Is.EqualTo(3));  // 2nd column = myInt
                        // DateTime is stored as long, WARNING may vary, see storeDateTimeAsTicks when creating db connection
                        // Guid is stored as string

                        // verify we can retrieve all values regardless of type
                        for (int i = 0; i < columnCount; i++)
                        {
                            Assert.That(SQLiteSession.ChangeSetNewValue(iter, i, out value), Is.EqualTo(SQLite3.Result.OK));
                            object o = value.GetValue();
                            Assert.That(o, Is.Not.Null);
                        }
                    }
                    Assert.That(result, Is.EqualTo(SQLite3.Result.Done));
                }
                Assert.That(iter.IsInvalid, Is.True);
            }
            Assert.That(changeSet.buffer.IsInvalid, Is.True);

            // done with our session
            SQLiteSession.Delete(session);
        }

        [Test, Category("SQLiteSession")]
        public void TestSession42_ReplicateSimple()
        {
            // close replica we just created and attach db file to our primary db as "replica"
            dbReplica.Close();
            dbPrimary.AttachDB(dbReplicaPath, "replica");

            // verify that "main" has rows in TestTable1 and "replica" currently empty
            var mainTestTable1Rows = dbPrimary.Query<TestTable1>("Select * FROM main.TestTable1;");
            Assert.That(mainTestTable1Rows.Count, Is.GreaterThan(0));

            var replicaTestTable1Rows = dbPrimary.Query<TestTable1>("Select * FROM replica.TestTable1;");
            Assert.That(replicaTestTable1Rows.Count, Is.EqualTo(0));

            // create a session for "main" database
            Assert.That(dbPrimary.CreateSession("main", out session), Is.EqualTo(SQLite3.Result.OK));

            // attach all tables [from "main"] to it
            Assert.That(SQLiteSession.AttachToTable(session, null), Is.EqualTo(SQLite3.Result.OK));

            // Note: we have not enabled watching for changes

            // validate no changes have occurred
            Assert.That(SQLiteSession.IsEmptySession(session), Is.True);

            // see what it would take to make replica.table like main.table
#pragma warning disable IDE0018 // Inline variable declaration
            string errMsg;
#pragma warning restore IDE0018 // Inline variable declaration
            Assert.That(SQLiteSession.AddTableDiffToSession(session, "replica", nameof(TestTable1), out errMsg), Is.EqualTo(SQLite3.Result.OK));
            Assert.That(SQLiteSession.AddTableDiffToSession(session, "replica", nameof(TestTable2), out errMsg), Is.EqualTo(SQLite3.Result.OK));

            // session should no longer be empty, as tables differ and session should have the diff
            Assert.That(SQLiteSession.IsEmptySession(session), Is.False);

            // create change set
            Assert.That(SQLiteSession.GenerateChangeSet(session, out SQLiteChangeSet changeSet), Is.EqualTo(SQLite3.Result.OK));

            // done with our session
            SQLiteSession.Delete(session);

            // remove secondary db
            dbPrimary.DetachDB("replica");

            // reopen dbReplica
            dbReplica = new SQLiteConnection(dbReplicaPath, storeDateTimeAsTicks: true);

            // validate reopened replica db has no rows
            replicaTestTable1Rows = dbReplica.Query<TestTable1>("Select * FROM TestTable1;");
            Assert.That(replicaTestTable1Rows.Count, Is.EqualTo(0));

            // apply change set to dbReplica
            // should be no conflicts so conflict handler not called
            Assert.That(dbReplica.ApplySessionChangeSet(changeSet, null, null, null), Is.EqualTo(SQLite3.Result.OK));

            // validate has contents we expect
            replicaTestTable1Rows = dbReplica.Query<TestTable1>("Select * FROM TestTable1;");
            Assert.That(replicaTestTable1Rows.Count, Is.GreaterThan(0));

            // generate inverse change set
            Assert.That(SQLiteSession.InvertChangeSet(changeSet, out SQLiteChangeSet inverseChangeSet), Is.EqualTo(SQLite3.Result.OK));
            using (inverseChangeSet)
            {
                // apply the inverse
                Assert.That(dbReplica.ApplySessionChangeSet(inverseChangeSet, null, null, null), Is.EqualTo(SQLite3.Result.OK));

                // validate replica db once again has no rows
                replicaTestTable1Rows = dbReplica.Query<TestTable1>("Select * FROM TestTable1;");
                Assert.That(replicaTestTable1Rows.Count, Is.EqualTo(0));
            }

            // reset
            CleanupTest();
            SetupTest();

            // insert differing row to dbReplica with same primary key, i.e. force conflict
            // add a row to table
            var newRow = new TestTable1
            {
                pk = sampleData.pk,
                myString = "test conflict",  // <== only conflicting piece of data
                myInt = sampleData.myInt,
                myDate = sampleData.myDate,
                myTable2Object = sampleData.myTable2Object
            };
            Assert.That(dbReplica.Insert(newRow), Is.EqualTo(1));

            // apply change set to dbReplica
            // should now be a conflict so conflict handler is called
            Assert.That(dbReplica.ApplySessionChangeSet(changeSet, DummyFilterCallback /* null */, SQLiteSession.CallbackReplaceOnConflicts, null /*"my text context"*/), Is.EqualTo(SQLite3.Result.OK));

            // valid has contents we expect
            replicaTestTable1Rows = dbReplica.Query<TestTable1>("Select * FROM TestTable1;");
            Assert.That(replicaTestTable1Rows.Count, Is.GreaterThan(0));

            // release our change set buffer explicitly
            changeSet.Dispose();
        }

        public static bool DummyFilterCallback(IntPtr ctx, [MarshalAs(UnmanagedType.LPStr)] string tableName)
        {
            return true;
        }
    }
}
