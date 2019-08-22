// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;

namespace TEMS_Inventory.views
{
    public class ServiceHistoryViewModel : HistoryWindowViewModel
    {
        public ServiceHistoryViewModel() : this(null) { }
        public ServiceHistoryViewModel(SearchFilterOptions SearchFilter) : base(SearchFilter) { }

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        protected override void DoUpdateCommand()
        {
            var viewModel = new DetailsServiceViewModel(SelectedEvent as ItemService);
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
        }


        /// ****** PUT THIS IN ICommand OnUpdateServiceHistorySelection *********
        /// <summary>
        /// Invoked when the current SelectedItem changes to update corresponding Events collection for newly selected item(s)
        /// </summary>
        /// <param name="SelectedItem"></param>
        public override void UpdateEvents(SearchResult SelectedItem)
        {
            Events = new ObservableCollection<ItemBase>(((IEnumerable)DataRepository.GetDataRepository.GetItemServiceEvents(SelectedItem)).Cast<ItemBase>());
        }

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
        public ICommand ServiceItemCommand
        {
            get { return InitializeCommand(ref _ServiceItemCommand, param => DoServiceItemCommand(), param => isCurrentItem()); }
        }
        private ICommand _ServiceItemCommand;

        private void DoServiceItemCommand()
        {
            var newWin = new ServiceDetailsWindow((currentItem as ItemInstance));
            ShowChildWindow(newWin);
        }
    }
}
