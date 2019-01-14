// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using NLog;

using StatePrinting;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Interface to database entities - all queries, updates, etc. handled via an instance of this class.
    /// Uses Database class to perform actual interaction with back-end database.
    /// </summary>
    public sealed partial class DataRepository : IPersistableRepository,
        IUserRepository, IEventRepository, IVendorRepository, IGEMRepository, IReportRepository, IDisposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Stateprinter statePrinter = new Stateprinter();

        // must specify database file on creation
        // will throw an error if database not found or invalid format
        public DataRepository(string dbPath)
        {
            logger.Trace(nameof(DataRepository));
            db = new Database(dbPath);

            // initialize our reference data cache
            ReferenceData = new ReferenceDataCache(db);

            // store as Singleton so we can access as needed by various ICommands, ViewModels, etc.
            GetDataRepository = this;
        }

        // maintains our connection to the database
        public Database db { get; private set; }

        // cache of reference data
        public ReferenceDataCache ReferenceData { get; private set; }

        /// <summary>
        /// Returns the current DataRepository instance.
        /// Note this will return null until the it is created
        /// by the application - WARNING only 1 should be created per
        /// application instance (last one instantiated is returned).
        /// </summary>
        public static DataRepository GetDataRepository { get; private set; } = null;

        /// <summary>
        /// cleanup database connection
        /// </summary>
        public void Dispose()
        {
            db?.Dispose();
            db = null;
        }

        /// <summary>
        /// initiate a set of changes that needs to be grouped together,
        /// either all succeed or all fail
        /// </summary>
        public void BeginTransaction()
        {
            db.BeginTransaction();
        }

        /// <summary>
        /// mark end of group changes, actually commits results
        /// </summary>
        public void EndTransaction()
        {
            try
            {
                db.EndTransaction(EndTransactionAction.COMMIT);
            }
            catch (Exception e)
            {
                logger.Error(e, "DB commit failed!");
                db.EndTransaction(EndTransactionAction.ROLLBACK);
                throw;
            }
        }

        #region document and image lookups

        /// <summary>
        /// Loads data[] document or image meta data represents.
        /// </summary>
        /// <param name="documentOrImage"></param>
        /// <returns></returns>
        public RawData LoadData(RawData documentOrImage) { return documentOrImage; /* TODO */ }

        #endregion document and image lookups
    }
}