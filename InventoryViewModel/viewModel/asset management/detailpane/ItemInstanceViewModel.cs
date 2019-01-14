// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.userManager;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    public class ItemInstanceViewModel : ViewModelBase
    {
        public ItemInstanceViewModel() : base()
        {
            Mediator.Register(nameof(CurrentItemChangedMessage), (msg) => { this.CurrentItem = ((CurrentItemChangedMessage)msg).CurrentItem; });
            Mediator.Register(nameof(CurrentUserChangedMessage), (msg) => { RaisePropertyChanged(nameof(IsAdmin)); });
        }

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
