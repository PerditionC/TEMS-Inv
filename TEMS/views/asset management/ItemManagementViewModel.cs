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
    public class ItemManagementViewModel : BasicListAndDetailWithSearchFilterWindowViewModel
    {
        // anything that needs initializing for MSVC designer
        public ItemManagementViewModel() : base() { }

        /// <summary>
        /// initialize our SearchFilter view model and any other controls needing intializing
        /// Note: moved out of constructor to avoid issues with MSVC design viewer
        /// </summary>
        public override void Initialize(DataRepository db, Func<ItemBase> GetNewItem)
        {
            base.Initialize(db, GetNewItem);
            // site & status are specific to an instance so don't use
            SearchFilter.SiteLocationVisible = false;
            SearchFilter.SiteLocationEnabled = false;
            SearchFilter.SelectItemStatusValuesVisible = false;
            SearchFilter.SelectItemStatusValuesEnabled = false;
        }

        /// <summary>
        /// returns filled in result to use with item list
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public override QueryResultsBase ItemResult(object primaryKey)
        {
            return db.QueryItemResult((Guid)primaryKey);
        }

        /// <summary>
        /// load ItemInstance based on item selected from list, ie load shadow object
        /// </summary>
        /// <param name="selListItem"></param>
        protected override void loadSelectedItem(ItemResult selListItem)
        {
            if ((selListItem?.pk != null) && (selListItem.pk != Guid.Empty))
            {
                selectedItem = db.db.Load<Item>(selListItem.pk);

#if false
                // if not currently editing anything then we match current item, but
                // we don't update currentItem otherwise as may be a clone, etc.
                if (isDetailViewInActive && EditCommand.CanExecute(null)) EditCommand.Execute(null);
#else
                // update detail view immediately when a new item is selected
                // note this will loose changes for a clone
                if (currentItem != null && currentItem.IsChanged)
                {
                    var x = MessageBox.Show("Current item has been modified, do you wish to save changes?", $"Changes to {currentItem.displayName} will be lost!", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                    if (x == MessageBoxResult.Yes)
                    {
                        if (SaveCommand.CanExecute(null)) SaveCommand.Execute(null);
                    }
                }
                if (EditCommand.CanExecute(null)) EditCommand.Execute(null);
#endif
            }
            else
            {
                selectedItem = null;
            }
        }


        #region Open item selection dialog

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand OpenSelectItemTypeWindowCommand
        {
            get { return InitializeCommand(ref _OpenSelectItemTypeWindowCommand, param => DoOpenSelectItemTypeWindowCommand(), param => isCurrentItem()); }
        }
        private ICommand _OpenSelectItemTypeWindowCommand;

        private void DoOpenSelectItemTypeWindowCommand()
        {
            // TODO implement me!!!
            var newWin = new ItemTypeManagementWindow();
            var searchFilter = newWin.ViewModel.SearchFilter;
            searchFilter.SearchFilterVisible = false;
            searchFilter.SearchText = (currentItem as Item)?.itemNumber?.ToString() ?? "";
            ShowChildWindow(newWin);
        }

        #endregion // Open item selection dialog

        #region Open item edit Window

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand OpenEditItemTypeWindowCommand
        {
            get { return InitializeCommand(ref _OpenEditItemTypeWindowCommand, param => DoOpenEditItemTypeWindowCommand(), param => isCurrentItem()); }
        }
        private ICommand _OpenEditItemTypeWindowCommand;

        private void DoOpenEditItemTypeWindowCommand()
        {
            var newWin = new ItemTypeManagementWindow();
            var searchFilter = newWin.ViewModel.SearchFilter;
            searchFilter.SearchFilterVisible = false;
            searchFilter.SearchText = (currentItem as Item)?.itemNumber?.ToString() ?? "";
            ShowChildWindow(newWin);
        }

        #endregion // Open item edit Window

        protected override void DoSave()
        {
            // first actually save this item and update display
            base.DoSave();

            // now we need to ensure there is an item instance for each location that has this item
            // TODO
        }

        protected override void DoClone()
        {
            base.DoClone();

            // TODO need to fixup clone so it also clones the instance items
        }

        protected override void DoDelete()
        {
            base.DoDelete();

            // TODO delete needs to remove all the instance items as well
        }
    }
}
