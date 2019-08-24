// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using InventoryViewModel;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS_Inventory.views;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// command invoked when the selected item in the SearchResults pane is changed
    /// this command should update the details pane view model accordingly
    /// </summary>
    public class UpdateDetailsGeneralInventoryManagementCommand : OnSelectionChangedCommand
    {
        public UpdateDetailsGeneralInventoryManagementCommand(DetailsViewModelBase detailsPaneVM) : base(detailsPaneVM)
        {
        }

        /// <summary>
        /// update the details view model based on current selection
        /// </summary>
        /// <param name="selectedItem">SearchResult to display details about, may be null for nothing selected</param>
        protected override void UpdateDetailsPane(SearchResult selectedItem)
        {
            base.UpdateDetailsPane(selectedItem);

            var db = DataRepository.GetDataRepository;
            var item = db.Load<ItemInstance>(selectedItem.id);
            if (detailsPaneVM is GeneralInventoryManagementViewModel detailsPane)
            {
                Mapper.GetMapper().Map(item.item.itemType, detailsPane);
                Mapper.GetMapper().Map(item.item, detailsPane);
                Mapper.GetMapper().Map(item, detailsPane);
            }
        }
    }
}