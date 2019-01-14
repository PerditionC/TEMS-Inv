// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.events;
using TEMS.InventoryModel.entity.db.query;


namespace TEMS_Inventory.views
{
    public class ServiceHistoryViewModel : BasicHistoryWindowViewModel
    {
        // anything that needs initializing for MSVC designer
        public ServiceHistoryViewModel() : base() { }

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
            logger.Debug("Loading service events - DoSearch:\n" + SearchFilter.ToString());
            // TODO use SearchFilter to limit itemInstances events returned for
            items = new ObservableCollection<ItemBase>(db.db.LoadAll<ItemServiceHistory>());
        }

        #endregion // Item Search/Filter - refresh items

        #region ICommand Actions

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        protected override void DoNewEventCommand()
        {
            var newWin = new ServiceItemSelectWindow();

            if (isItemSelected())
            {
                var searchFilter = newWin.ViewModel.SearchFilter;
                var itemService = SelectedItem as ItemServiceHistory;
                searchFilter.SearchText = itemService.service?.itemInstance?.itemNumber ?? "";
                searchFilter.ItemTypeMatching = SearchFilterItemMatching.OnlyExact;
                searchFilter.SearchFilterVisible = false;
            }

            ShowWindow(newWin);
        }

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        protected override void DoUpdateCommand()
        {
            var newWin = new ServiceDetailsWindow();

                var itemService = SelectedItem as ItemServiceHistory;
                //searchFilter.SearchText = itemService?.service?.itemInstance?.itemNumber ?? "";
            ShowWindow(newWin);
        }

        #endregion // ICommand Actions
    }
}
