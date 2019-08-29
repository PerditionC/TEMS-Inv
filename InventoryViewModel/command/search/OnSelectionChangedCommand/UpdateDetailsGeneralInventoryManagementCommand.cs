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

            if (selectedItem != null)
            {
                // get full item data from DB
                object item;
                try
                {
                    if ((selectedItem.id != Guid.Empty) && !(selectedItem is GroupHeader))
                    {
                        var db = DataRepository.GetDataRepository;
                        item = db.Load(selectedItem.id, "ItemInstance");
                    }
                    else
                    {
                        item = null;
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Failed to load {selectedItem.id} from table {"ItemInstance"}, details will be blank!");
                    detailsPaneVM.StatusMessage = $"Failed to load ({selectedItem.id}) from database.";
                    item = null;
                }

                // update displayed data
                if (item != null)
                {
                    //if (detailsPaneVM is ItemTypeManagementViewModel viewModel)
                    {
                        if (detailsPaneVM is GeneralInventoryManagementViewModel detailsPane)
                        {
                            if (item is ItemInstance itemInstance)
                            {
                                // note: order here matters, to ensure id of Instance is one shown
                                Mapper.GetMapper().Map(itemInstance.item.itemType, detailsPane);
                                Mapper.GetMapper().Map(itemInstance.item, detailsPane);
                                Mapper.GetMapper().Map(itemInstance, detailsPane);
                            }
                        }
                    }
                }
                else
                {
                    detailsPaneVM.clear();
                }
            }
        }
    }
}