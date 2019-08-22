// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;

namespace TEMS_Inventory.views
{
    public class HistoryDamagedMissingViewModel : HistoryWindowViewModel
    {
        public HistoryDamagedMissingViewModel() : this(null) { }
        public HistoryDamagedMissingViewModel(SearchFilterOptions SearchFilter) : base(SearchFilter) { }

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        protected override void DoUpdateCommand()
        {
            var viewModel = new DetailsDamagedMissingViewModel(SelectedEvent as DamageMissingEvent);
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
        }

        /// <summary>
        /// Invoked when the current SelectedItem changes to update corresponding Events collection for newly selected item(s)
        /// </summary>
        /// <param name="SelectedItem"></param>
        public override void UpdateEvents(SearchResult SelectedItem)
        {
            Events = new ObservableCollection<ItemBase>(((IEnumerable)DataRepository.GetDataRepository.GetDamageMissingEvents(SelectedItem)).Cast<ItemBase>());
        }


        /// <summary>
        /// Command to create an event record of item as damaged
        /// </summary>
        public ICommand DamagedCommand
        {
            get { return InitializeCommand(ref _DamagedCommand, param => DoDamagedCommand(), param => isCurrentItem()); }
        }
        private ICommand _DamagedCommand;

        private void DoDamagedCommand()
        {
            var newWin = new DamagedMissingDetailsWindow();
            //searchFilter.SearchText = (currentItem as ItemInstance)?.itemNumber?.ToString() ?? "";
            ShowChildWindow(newWin);
        }


        /// <summary>
        /// Command to create an event record of item as missing
        /// </summary>
        public ICommand MissingCommand
        {
            get { return InitializeCommand(ref _MissingCommand, param => DoMissingCommand(), param => isCurrentItem()); }
        }
        private ICommand _MissingCommand;

        private void DoMissingCommand()
        {
            var newWin = new DamagedMissingDetailsWindow();
            ShowChildWindow(newWin);
        }

    }
}
