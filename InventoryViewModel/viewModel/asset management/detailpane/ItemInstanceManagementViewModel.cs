// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.userManager;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Administration CRUD view model for updating ItemInstance table
    /// </summary>
    public class ItemInstanceManagementViewModel : ViewModelBase
    {
        public ItemInstanceManagementViewModel() : base() { }

        /// <summary>
        /// does active user have administrative privileges or just normal user privileges
        /// true if limited to user privileges
        /// </summary>
        public bool IsAdmin
        {
            get { return UserManager.GetUserManager.CurrentUser().isAdmin; }
        }

        /// <summary>
        /// Is there a currently active (non-null) item
        /// </summary>
        public bool IsActiveItem
        {
            get { return _CurrentItem != null; }
        }

        /// <summary>
        /// the current item for display (and edit)
        /// </summary>
        public ItemInstance CurrentItem
        {
            get { return _CurrentItem; }
            set
            {
                SetProperty(ref _CurrentItem, value, nameof(CurrentItem));
                RaisePropertyChanged(nameof(IsActiveItem));
            }
        }
        private ItemInstance _CurrentItem = null;
    }
}
