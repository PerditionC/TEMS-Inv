// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;


namespace TEMS_Inventory.views
{
    public class ServiceDetailsViewModel : BasicDetailWindowViewModel
    {
        // anything that needs initializing for MSVC designer
        public ServiceDetailsViewModel() : base() { }

        /// <summary>
        /// initialize our SearchFilter view model and any other controls needing intializing
        /// Note: moved out of constructor to avoid issues with MSVC design viewer
        /// </summary>
        public override void Initialize(DataRepository db, Func<ItemBase> GetNewItem)
        {
            base.Initialize(db, GetNewItem);
        }

        #region Item Search/Filter - refresh items

        /// <summary>
        /// Returns a ObservableCollection of ItemBase [DamageMissingEvent]
        /// </summary>
        protected override void DoSearch()
        {
            logger.Debug("Loading service events - DoSearch:\n");
        }

        #endregion // Item Search/Filter - refresh items

        #region ICommand Actions

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        protected override void DoUpdateCommand()
        {
        }

        #endregion // ICommand Actions
    }
}
