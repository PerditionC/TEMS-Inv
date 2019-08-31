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
    public class UpdateDetailsItemManagementCommand : OnSelectionChangedCommand
    {
        /// <summary>
        /// the name of the table to load corresponding item from, note that table names match Type names
        /// </summary>
        private string tableName;

        /// <summary>
        /// initialize command to update a basic view model that represents a single ItemBase derived view
        /// </summary>
        /// <param name="detailsPaneVM"></param>
        /// <param name="type">Must be one of typeof(ItemType), typeof(Item), or typeof(ItemInstance)</param>
        public UpdateDetailsItemManagementCommand(DetailsViewModelBase detailsPaneVM, System.Type type) : base(detailsPaneVM)
        {
            if (typeof(ItemBase).IsAssignableFrom(type))
            {
                tableName = type.Name;
            }
            else
            {
                throw new ArgumentException("type argument must be of an ItemBase derived object matching an item related table name in DB", nameof(type));
            }
        }

        /// <summary>
        /// update the details view model based on current selection
        /// </summary>
        /// <param name="selectedItem">SearchResult to display details about, may be null for nothing selected</param>
        protected override void UpdateDetailsPane(SearchResult selectedItem)
        {
            base.UpdateDetailsPane(selectedItem);

            // clear any existing binding values
            detailsPaneVM.clear();

            if (selectedItem != null)
            {
                // get full item data from DB
                object item = null;
                try
                {
                    if ((selectedItem.id != Guid.Empty) && !(selectedItem is GroupHeader))
                    {
                        var db = DataRepository.GetDataRepository;
                        item = db.Load(selectedItem.id, tableName);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Failed to load {selectedItem.id} from table {tableName}, details will be blank!");
                    detailsPaneVM.StatusMessage = $"Failed to load ({selectedItem.id}) from database.";
                    item = null;
                }

                // update displayed data
                if (item is ItemBase itemBase)
                {
                    doMapping(itemBase);
                }
            }
        }

        /// <summary>
        /// Maps item to detailsPane
        /// </summary>
        /// <param name="item"></param>
        protected virtual void doMapping(ItemBase item)
        {
            try
            {
                Mapper.GetMapper().Map(item, detailsPaneVM);
            }
            catch (Exception e)
            {
                logger.Error(e, "Failed to map item to view model.");
                throw;
            }
        }
    }
}