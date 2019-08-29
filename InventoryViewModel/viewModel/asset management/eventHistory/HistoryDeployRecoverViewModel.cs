// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using TEMS.InventoryModel.entity.db;

namespace TEMS_Inventory.views
{
    public class HistoryDeployRecoverViewModel : EventHistoryViewModelBase
    {
        public HistoryDeployRecoverViewModel() : base() { }

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        protected override void DoUpdateCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "DeployRecoverDetails", args = SelectedEvent as DeployEvent });
        }
    }
}
