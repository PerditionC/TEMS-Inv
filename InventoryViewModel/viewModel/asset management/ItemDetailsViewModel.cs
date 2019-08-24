// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.command.action;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.userManager;

namespace TEMS_Inventory.views
{
    public class ItemDetailsViewModel : DetailsViewModelBase
    {
        public ItemDetailsViewModel() : base()
        {
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
        /// command to persist information to our backing store (save to DB)
        /// </summary>
        public ICommand SaveCommand = new SaveItemCommand();
    }
}
