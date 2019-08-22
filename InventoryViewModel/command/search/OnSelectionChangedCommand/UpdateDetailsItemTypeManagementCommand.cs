// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util;
using TEMS_Inventory.views;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// command invoked when the selected item in the SearchResults pane is changed
    /// this command should update the details pane view model accordingly
    /// </summary>
    public abstract class OnSelectionChangedCommand : RelayCommand
    {
        protected DetailsViewModelBase detailsPaneVM;

        public OnSelectionChangedCommand(DetailsViewModelBase detailsPaneVM) : base()
        {
            this.detailsPaneVM = detailsPaneVM;
            //_canExecute = true;
            _execute = (selectedItem) => UpdateDetailsPane(selectedItem as SearchResult);
        }

        /// <summary>
        /// update the details view model based on current selection
        /// </summary>
        /// <param name="selectedItem">SearchResult to display details about, may be null for nothing selected</param>
        protected abstract void UpdateDetailsPane(SearchResult selectedItem);
    }
}