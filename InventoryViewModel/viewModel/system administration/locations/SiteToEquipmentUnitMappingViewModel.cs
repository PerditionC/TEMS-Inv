// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;


namespace TEMS_Inventory.views
{
    public class SiteToEquipmentUnitMappingViewModel : ViewModelBase
    {
        // anything that needs initializing for MSVC designer
        public SiteToEquipmentUnitMappingViewModel() : base() { }

        #region Commands

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand OpenManageVendorsWindowCommand
        {
            get { return InitializeCommand(ref _OpenManageVendorsWindowCommand, param => DoOpenManageVendorsWindowCommand(), null); }
        }
        private ICommand _OpenManageVendorsWindowCommand;

        #endregion // Commands

        #region ICommand Actions

        private void DoOpenManageVendorsWindowCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName="ManageVendors" });
        }

        #endregion // ICommand Actions
    }
}
