// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.userManager;

namespace TEMS_Inventory.views
{
    public class HistoryDamagedMissingViewModel : EventHistoryViewModelBase
    {
        public HistoryDamagedMissingViewModel() : base() { }

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        protected override void DoUpdateCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "DamagedMissingDetails", args = SelectedEvent as DamageMissingEvent });
        }


        /// <summary>
        /// Command to create an event record of item as damaged
        /// </summary>
        public ICommand DamagedCommand
        {
            get { return InitializeCommand(ref _DamagedCommand, param => DoDamagedCommand(), param => IsEventSelected); }
        }
        private ICommand _DamagedCommand;

        private void DoDamagedCommand()
        {
            var currentUser = UserManager.GetUserManager.CurrentUser();
            var damagedMissingEvent = new DamageMissingEvent()
            {
                eventType = DamageMissingEventType.Damage,
                itemInstance = CurrentItem.entity as ItemInstance,
                discoveryDate = DateTime.Now,
                inputBy = currentUser.userId,
                reportedBy = currentUser.displayName
            };
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "DamagedMissingDetails", args = damagedMissingEvent as DamageMissingEvent });
            SelectedEvent = damagedMissingEvent;
        }


        /// <summary>
        /// Command to create an event record of item as missing
        /// </summary>
        public ICommand MissingCommand
        {
            get { return InitializeCommand(ref _MissingCommand, param => DoMissingCommand(), param => IsEventSelected); }
        }
        private ICommand _MissingCommand;

        private void DoMissingCommand()
        {
            var currentUser = UserManager.GetUserManager.CurrentUser();
            var damagedMissingEvent = new DamageMissingEvent()
            {
                eventType = DamageMissingEventType.Missing,
                itemInstance = CurrentItem.entity as ItemInstance,
                discoveryDate = DateTime.Now,
                inputBy = currentUser.userId,
                reportedBy = currentUser.displayName
            };
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "DamagedMissingDetails", args = damagedMissingEvent as DamageMissingEvent });
            SelectedEvent = damagedMissingEvent;
        }

    }
}
