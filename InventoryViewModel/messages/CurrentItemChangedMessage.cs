// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// message argument passed to indicate current ItemInstance changed
    /// </summary>
    public class CurrentItemChangedMessage
    {
        public SearchResult SelectedItem;
        public ItemInstance CurrentItem;
    }
}
