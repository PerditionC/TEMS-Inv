// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows.Input;
using TEMS.InventoryModel.entity.db;

namespace TEMS_Inventory.views
{
    public class ReplicationViewModel : ViewModelBase
    {
        // anything that needs initializing for MSVC designer
        public ReplicationViewModel() : base() { }

        // the name (including full/relative path) of the database to sync changes (replicate) from
        public string replicaDB
        {
            get { return _replicaDb; }
            set {
                SetProperty(ref _replicaDb, value, nameof(replicaDB));
                message = $"Replicate changes from {_replicaDb} to current DB";
            }
        }
        private string _replicaDb = null; //"replica.db";

        // message shown to use to indicate current status - TODO improve UI
        public string message {
            get { return _message; }
            set { SetProperty(ref _message, value, nameof(message)); }
        }
        private string _message = "Please select DB to replicate changes from to current DB";


        #region Commands

        /// <summary>
        /// Command to sync changes to and from replica
        /// </summary>
        public ICommand SyncDbCommand
        {
            get { return InitializeCommand(ref _SyncDbCommand, param => DoSyncDbCommand(), param => !String.IsNullOrWhiteSpace(replicaDB)); }
        }
        private ICommand _SyncDbCommand;

        /// <summary>
        /// Command to sync changes from replica
        /// </summary>
        public ICommand SyncFromDbCommand
        {
            get { return InitializeCommand(ref _SyncFromDbCommand, param => DoSyncFromDbCommand(), param => !String.IsNullOrWhiteSpace(replicaDB)); }
        }
        private ICommand _SyncFromDbCommand;

        /// <summary>
        /// Command to sync changes to replica
        /// </summary>
        public ICommand SyncToDbCommand
        {
            get { return InitializeCommand(ref _SyncToDbCommand, param => DoSyncToDbCommand(), param => !String.IsNullOrWhiteSpace(replicaDB)); }
        }
        private ICommand _SyncToDbCommand;

        #endregion // Commands

        #region ICommand Actions

        private void DoSyncDbCommand()
        {
            var db = DataRepository.GetDataRepository;
            message = $"Synchronizing changes to and from {replicaDB} ...";
            // pull changes first, then push
            db.db.SyncFromDb(replicaDB);
            db.db.SyncToDb(replicaDB);
            message = "Synchronization complete.";
        }

        private void DoSyncFromDbCommand()
        {
            var db = DataRepository.GetDataRepository;
            message = $"Synchronizing changes from {replicaDB} ...";
            db.db.SyncFromDb(replicaDB);
            message = "Synchronization complete.";
        }

        private void DoSyncToDbCommand()
        {
            var db = DataRepository.GetDataRepository;
            message = $"Synchronizing changes to {replicaDB} ...";
            db.db.SyncToDb(replicaDB);
            message = "Synchronization complete.";
        }

        #endregion // ICommand Actions
    }
}
