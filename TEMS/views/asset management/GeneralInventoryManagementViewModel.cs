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
    public class GeneralInventoryManagementViewModel : BasicListAndDetailWithSearchFilterWindowViewModel
    {
        public DeployRecoverItemInstanceAction deployRecoverCommands { get; set; } = new DeployRecoverItemInstanceAction();

        // anything that needs initializing for MSVC designer
        public GeneralInventoryManagementViewModel() : base() { }

        public override void Initialize(DataRepository db, Func<ItemBase> GetNewItem)
        {
            base.Initialize(db, GetNewItem);
            deployRecoverCommands.db = db;
        }

        /// <summary>
        /// does active user have admin privileges or just normal user privileges
        /// true if limited to user privileges
        /// </summary>
        public bool isAdmin
        {
            get { return SearchFilter.User.isAdmin; }
        }

        /// <summary>
        /// load ItemInstance based on item selected from list, ie load shadow object
        /// </summary>
        /// <param name="selListItem"></param>
        /// <returns></returns>
        protected override void loadSelectedItem(ItemResult selListItem)
        {
            try
            {
                if ((selListItem?.instancePk != null) && (selListItem.instancePk != Guid.Empty))
                {
                    selectedItem = db.db.Load<ItemInstance>(selListItem.instancePk);
                    // we don't use DoEdit as we don't need a clone of shadow selectedItem for currentItem
                    currentItem = selectedItem;  //DoEdit();
                                                 // Note: need same object so changes triggered via items bound to selectedListItem will show in detail view
                    selListItem.entity = currentItem;
                }
                else
                {
                    selectedItem = null;
                    currentItem = null;
                }
            }catch (Exception e)
            {
                logger.Error(e, $"GIM:loadSelectedItem - {e.Message}");
                throw;
            }
        }


        #region Commands

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand OpenEditItemWindowCommand
        {
            get { return InitializeCommand(ref _OpenEditItemWindowCommand, param => DoOpenEditItemWindowCommand(), param => isCurrentItem()); }
        }
        private ICommand _OpenEditItemWindowCommand;

        /// <summary>
        /// Command to open print labels window with this item selected so can be printed
        /// </summary>
        public ICommand OpenPrintBarcodeWindowCommand
        {
            get { return InitializeCommand(ref _OpenPrintBarcodeWindowCommand, param => DoOpenPrintBarcodeWindowCommand(), param => isCurrentItem()); }
        }
        private ICommand _OpenPrintBarcodeWindowCommand;


        /// <summary>
        /// Command to add an item
        /// </summary>
        public ICommand AddServiceDetailsCommand
        {
            get { return InitializeCommand(ref _AddServiceDetailsCommand, param => DoAddServiceDetailsCommand(), param => isCurrentItem()); }
        }
        private ICommand _AddServiceDetailsCommand;


        /// <summary>
        /// Command to add an item that is initially same as selected item
        /// </summary>
        public ICommand ViewServiceHistoryCommand
        {
            get { return InitializeCommand(ref _ViewServiceHistoryCommand, param => DoViewServiceHistoryCommand(), param => isCurrentItem()); }
        }
        private ICommand _ViewServiceHistoryCommand;


        /// <summary>
        /// Command to remove selected item from items collection and remove (delete or mark deleted) in backend DB
        /// </summary>
        public ICommand ViewDeployRecoverHistoryCommand
        {
            get { return InitializeCommand(ref _ViewDeployRecoverHistoryCommand, param => DoViewDeployRecoverHistoryCommand(), param => isCurrentItem()); }
        }
        private ICommand _ViewDeployRecoverHistoryCommand;


        /// <summary>
        /// Command to actually persist currentItem, if newly added also adds to the list and sets as selected item
        /// </summary>
        public ICommand AddDamagedDetailsCommand
        {
            get { return InitializeCommand(ref _AddDamagedDetailsCommand, param => DoAddDamagedDetailsCommand(), param => isCurrentItem()); }
        }
        private ICommand _AddDamagedDetailsCommand;


        /// <summary>
        /// Command to actually persist currentItem, if newly added also adds to the list and sets as selected item
        /// </summary>
        public ICommand AddMissingDetailsCommand
        {
            get { return InitializeCommand(ref _AddMissingDetailsCommand, param => DoAddMissingDetailsCommand(), param => isCurrentItem()); }
        }
        private ICommand _AddMissingDetailsCommand;


        /// <summary>
        /// Command to actually persist currentItem, if newly added also adds to the list and sets as selected item
        /// </summary>
        public ICommand ViewDamagedMissingOverviewCommand
        {
            get { return InitializeCommand(ref _ViewDamagedMissingOverviewCommand, param => DoViewDamagedMissingOverviewCommand(), param => isCurrentItem()); }
        }
        private ICommand _ViewDamagedMissingOverviewCommand;

        #endregion // Commands

        #region ICommand Actions

        private void DoOpenEditItemWindowCommand()
        {
            var newWin = new ItemManagementWindow();
            var searchFilter = newWin.ViewModel.SearchFilter;
            searchFilter.SearchFilterVisible = false;
            searchFilter.SearchText = (currentItem as ItemInstance)?.itemNumber?.ToString() ?? "";
            searchFilter.ItemTypeMatching = SearchFilterItemMatching.OnlyExact;
            ShowChildWindow(newWin);
        }

        private void DoOpenPrintBarcodeWindowCommand()
        {
            var newWin = new ViewPrintLabelsWindow();
            /*
            var searchFilter = newWin.ViewModel.SearchFilter;
            searchFilter.SearchFilterVisible = false;
            searchFilter.SiteLocationVisible = false;
            searchFilter.SelectItemStatusValuesVisible = false;
            searchFilter.SearchText = (currentItem as ItemInstance)?.itemNumber?.ToString() ?? "";
            searchFilter.ItemTypeMatching = SearchFilterItemMatching.OnlyExact;
            */
            ShowChildWindow(newWin);
        }

        private void DoAddServiceDetailsCommand()
        {
            var newWin = new ServiceDetailsWindow(currentItem);
            ShowChildWindow(newWin);
        }

        private void DoViewServiceHistoryCommand()
        {
            var newWin = new ServiceHistoryWindow();
            var searchFilter = newWin.ViewModel.SearchFilter;
            searchFilter.SearchFilterVisible = false;
            searchFilter.SiteLocationVisible = false;
            searchFilter.SelectItemStatusValuesVisible = false;
            searchFilter.SearchText = (currentItem as ItemInstance)?.itemNumber?.ToString() ?? "";
            searchFilter.ItemTypeMatching = SearchFilterItemMatching.OnlyExact;
            ShowChildWindow(newWin);
        }

        private void DoViewDeployRecoverHistoryCommand()
        {
            var newWin = new DeployRecoverHistoryWindow();
            var searchFilter = newWin.ViewModel.SearchFilter;
            searchFilter.SearchFilterVisible = false;
            searchFilter.SiteLocationVisible = false;
            searchFilter.SelectItemStatusValuesVisible = false;
            searchFilter.SearchText = (currentItem as ItemInstance)?.itemNumber?.ToString() ?? "";
            searchFilter.ItemTypeMatching = SearchFilterItemMatching.OnlyExact;
            ShowChildWindow(newWin);
        }

        private void DoAddDamagedDetailsCommand()
        {
            var newWin = new DamagedMissingDetailsWindow();
            // newWin.ItemInstance = currentItem;
            ShowChildWindow(newWin);
        }

        private void DoAddMissingDetailsCommand()
        {
            var newWin = new DamagedMissingDetailsWindow();
            // newWin.ItemInstance = currentItem;
            ShowChildWindow(newWin);
        }

        private void DoViewDamagedMissingOverviewCommand()
        {
            var newWin = new DamagedMissingOverviewWindow();
            var searchFilter = newWin.ViewModel.SearchFilter;
            searchFilter.SearchFilterVisible = false;
            searchFilter.SelectItemStatusValuesVisible = false;
            searchFilter.SearchText = (currentItem as ItemInstance)?.itemNumber?.ToString() ?? "";
            searchFilter.ItemTypeMatching = SearchFilterItemMatching.OnlyExact;
            ShowChildWindow(newWin);
        }

        #endregion // ICommand Actions
    }
}
