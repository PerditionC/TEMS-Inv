// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;

namespace TEMS_Inventory.views
{
    public class DeployRecoverHistoryViewModel : BasicHistoryWindowViewModel
    {
        // anything that needs initializing for MSVC designer
        public DeployRecoverHistoryViewModel() : base() { }

        /// <summary>
        /// initialize our SearchFilter view model and any other controls needing intializing
        /// Note: moved out of constructor to avoid issues with MSVC design viewer
        /// </summary>
        public override void Initialize(DataRepository db, Func<ItemBase> GetNewItem)
        {
            base.Initialize(db, GetNewItem);
        }

        #region Item Search/Filter - refresh items

        protected override void DoSearch()
        {
            logger.Debug("Loading events - DoSearch:\n" + SearchFilter.ToString());
            // TODO use SearchFilter to limit itemInstances events returned for
            items = new ObservableCollection<ItemBase>(db.db.LoadAll<DeployEvent>());
        }

        #endregion // Item Search/Filter - refresh items

        #region ICommand Actions

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        protected override void DoNewEventCommand()
        {
            var newWin = new DeployRecoverWindow();
            newWin.Owner = App.Current.MainWindow;
            //App.Current.MainWindow = newWin;

            if (isItemSelected())
            {
                var searchFilter = newWin.ViewModel.SearchFilter;
                var DeployEvent = SelectedItem as DeployEvent;
                searchFilter.SearchText = DeployEvent?.itemInstance?.itemNumber ?? "";
                searchFilter.ItemTypeMatching = SearchFilterItemMatching.OnlyExact;
                searchFilter.SearchFilterVisible = false;
            }

            newWin.ShowDialog();
        }

        protected override void DoUpdateCommand()
        {
            throw new NotImplementedException();
        }

        #endregion // ICommand Actions
    }
}
