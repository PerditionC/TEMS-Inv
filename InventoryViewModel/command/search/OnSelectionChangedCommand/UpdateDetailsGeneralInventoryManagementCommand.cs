// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using InventoryViewModel;
using TEMS.InventoryModel.entity.db;
using TEMS_Inventory.views;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// command invoked when the selected item in the SearchResults pane is changed
    /// this command should update the details pane view model accordingly
    /// </summary>
    public class UpdateDetailsGeneralInventoryManagementCommand : UpdateDetailsItemManagementCommand
    {
        public UpdateDetailsGeneralInventoryManagementCommand(DetailsViewModelBase detailsPaneVM) : base(detailsPaneVM, typeof(ItemInstance)) { }

        /// <summary>
        /// Maps item to detailsPane
        /// </summary>
        /// <param name="item"></param>
        protected override void doMapping(ItemBase item)
        {
            try
            {
                if (item is ItemInstance itemInstance)
                {
                    // note: order here matters, to ensure id of Instance is one shown
                    //Mapper.GetMapper().Map(itemInstance.item.itemType, detailsPaneVM);
                    //Mapper.GetMapper().Map(itemInstance.item, detailsPaneVM);
                    Mapper.GetMapper().Map(itemInstance, detailsPaneVM);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to map item to view model.");
                throw;
            }
        }
    }
}