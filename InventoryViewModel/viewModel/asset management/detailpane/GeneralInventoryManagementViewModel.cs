// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;



/*  search bar at top, it varies based on detail window
 *  result tree on the left of ItemInstance
 *  right side is detail window
 *      detail window is one of:
 *      ItemInstance details - can edit serial #, seal broken, print barcode, update notes; if admin In Service date & status
 *      History of ....
 *  bottom is actions
 */

namespace TEMS_Inventory.views
{
    public class GeneralInventoryManagementViewModel : MasterDetailItemWindowViewModelBase
    {
        public GeneralInventoryManagementViewModel() : this(null) { }
        public GeneralInventoryManagementViewModel(SearchFilterOptions SearchFilter) : base(QueryResultEntitySelector.ItemInstance, SearchFilter) { }

        /// <summary>
        /// does active user have administrative privileges or just normal user privileges
        /// true if limited to user privileges
        /// </summary>
        public bool IsAdmin
        {
            get { return SearchFilter.User.isAdmin; }
        }


        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand OpenEditItemWindowCommand
        {
            get { return InitializeCommand(ref _OpenEditItemWindowCommand, param => DoOpenEditItemWindowCommand(), param => { return IsSelectedItem && IsAdmin; }); }
        }
        private ICommand _OpenEditItemWindowCommand;

        private void DoOpenEditItemWindowCommand()
        {
            var viewModel = new ItemManagementViewModel();
            viewModel.SearchFilter.InitializeAs(SearchFilter);
            viewModel.SearchFilter.SearchFilterVisible = false;
            viewModel.SearchFilter.SearchText = (CurrentItem as ItemInstance)?.itemNumber?.ToString() ?? SearchFilter.SearchText;
            viewModel.SearchFilter.ItemTypeMatching = SearchFilterItemMatching.OnlyExact;
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
        }


        /// <summary>
        /// Command to open print labels window with this item selected so can be printed
        /// </summary>
        public ICommand OpenPrintBarcodeWindowCommand
        {
            get { return InitializeCommand(ref _OpenPrintBarcodeWindowCommand, param => DoOpenPrintBarcodeWindowCommand(), param => IsSelectedItem); }
        }
        private ICommand _OpenPrintBarcodeWindowCommand;

        private void DoOpenPrintBarcodeWindowCommand()
        {
            var viewModel = new ViewPrintLabelsViewModel();
            /*
            var searchFilter = viewModel.SearchFilter;
            searchFilter.SearchFilterVisible = false;
            searchFilter.SiteLocationVisible = false;
            searchFilter.SelectItemStatusValuesVisible = false;
            searchFilter.SearchText = (currentItem as ItemInstance)?.itemNumber?.ToString() ?? "";
            searchFilter.ItemTypeMatching = SearchFilterItemMatching.OnlyExact;
            */
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
        }


        /// <summary>
        /// Command to add an item
        /// </summary>
        public ICommand AddServiceDetailsCommand
        {
            get { return InitializeCommand(ref _AddServiceDetailsCommand, param => DoAddServiceDetailsCommand(), param => IsSelectedItem); }
        }
        private ICommand _AddServiceDetailsCommand;

        private void DoAddServiceDetailsCommand()
        {
            var serviceEvent = new ItemService()
            {
                itemInstance = CurrentItem as ItemInstance
            };
            var viewModel = new DetailsServiceViewModel(serviceEvent);
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
        }


        /// <summary>
        /// Command to add an item that is initially same as selected item
        /// </summary>
        public ICommand ViewServiceHistoryCommand
        {
            get { return InitializeCommand(ref _ViewServiceHistoryCommand, param => DoViewServiceHistoryCommand(), param => IsSelectedItem); }
        }
        private ICommand _ViewServiceHistoryCommand;

        private void DoViewServiceHistoryCommand()
        {
            var viewModel = new ServiceHistoryViewModel();
            //viewModel.Events = ???
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
        }


        /// <summary>
        /// Command to remove selected item from items collection and remove (delete or mark deleted) in back end DB
        /// </summary>
        public ICommand ViewDeployRecoverHistoryCommand
        {
            get { return InitializeCommand(ref _ViewDeployRecoverHistoryCommand, param => DoViewDeployRecoverHistoryCommand(), param => IsSelectedItem); }
        }
        private ICommand _ViewDeployRecoverHistoryCommand;

        private void DoViewDeployRecoverHistoryCommand()
        {
            var viewModel = new DeployRecoverHistoryViewModel
            {
                Events = null // all events for  (CurrentItem as ItemInstance)?.itemNumber?.ToString() ?? "";
            };
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
        }


        /// <summary>
        /// Command to actually persist currentItem, if newly added also adds to the list and sets as selected item
        /// </summary>
        public ICommand AddDamagedDetailsCommand
        {
            get { return InitializeCommand(ref _AddDamagedDetailsCommand, param => DoAddDamagedDetailsCommand(), param => IsSelectedItem); }
        }
        private ICommand _AddDamagedDetailsCommand;

        private void DoAddDamagedDetailsCommand()
        {
            var damageEvent = new DamageMissingEvent()
            {
                eventType = DamageMissingEventType.Damage,
                itemInstance = CurrentItem as ItemInstance
            };
            var viewModel = new DetailsDamagedMissingViewModel(damageEvent);
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
        }


        /// <summary>
        /// Command to actually persist currentItem, if newly added also adds to the list and sets as selected item
        /// </summary>
        public ICommand AddMissingDetailsCommand
        {
            get { return InitializeCommand(ref _AddMissingDetailsCommand, param => DoAddMissingDetailsCommand(), param => IsSelectedItem); }
        }
        private ICommand _AddMissingDetailsCommand;

        private void DoAddMissingDetailsCommand()
        {
            var missingEvent = new DamageMissingEvent()
            {
                eventType = DamageMissingEventType.Missing,
                itemInstance = CurrentItem as ItemInstance
            };
            var viewModel = new DetailsDamagedMissingViewModel(missingEvent);
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
        }
    }
}
