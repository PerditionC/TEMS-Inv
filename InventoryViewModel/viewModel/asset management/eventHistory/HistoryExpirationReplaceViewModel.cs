// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.userManager;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;

namespace TEMS_Inventory.views
{
    public class HistoryExpirationReplaceViewModel : EventHistoryViewModelBase
    {
        public HistoryExpirationReplaceViewModel() : base() { }

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        protected override void DoUpdateCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "ExpirationDetails", args = SelectedEvent });
        }


        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand ReplaceExpiredItemCommand
        {
            get { return InitializeCommand(ref _ReplaceExpiredItemCommand, param => DoReplaceExpiredItemCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _ReplaceExpiredItemCommand;

        private void DoReplaceExpiredItemCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "ExpirationDetails" });
        }
    }
}
