﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

using System;
#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

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
            throw new NotImplementedException();
        }


        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand ReplaceExpiredItemCommand
        {
            get { return InitializeCommand(ref _ServiceItemCommand, param => DoServiceItemCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _ServiceItemCommand;

        private void DoServiceItemCommand()
        {
            /*
            var newWin = new DetailsExpirationViewModel((currentItem as ItemInstance));
            ShowChildWindow(newWin);
            */
        }
    }
}
