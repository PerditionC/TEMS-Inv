// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Administration CRUD view model for updating ItemInstance table
    /// </summary>
    public class ItemInstanceManagementViewModel : ItemDetailsViewModel
    {
        public ItemInstanceManagementViewModel() : base() { }

        // *** TODO add all the properties from ItemInstance here
    }
}
