// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;

namespace TEMS_Inventory.views
{
    public class DeployRecoverViewModel : BasicListAndDetailWithSearchFilterWindowViewModel
    {
        // anything that needs initializing for MSVC designer
        public DeployRecoverViewModel() : base() { }

        public DeployRecoverItemInstanceAction deployRecoverCommands { get; set; } = new DeployRecoverItemInstanceAction();

        /// <summary>
        /// initialize our SearchFilter view model and any other controls needing intializing
        /// Note: moved out of constructor to avoid issues with MSVC design viewer
        /// </summary>
        public override void Initialize(DataRepository db, Func<ItemBase> GetNewItem)
        {
            base.Initialize(db, GetNewItem);
            deployRecoverCommands.db = db;
            SearchFilter.SelectItemStatusValuesVisible = false;
            //statusAvailable = SearchFilter.ItemStatusValues.FirstOrDefault(x => string.Equals(((ItemStatus)x).name, "Available", StringComparison.InvariantCultureIgnoreCase)) as ItemStatus;
            //statusDeployed = SearchFilter.ItemStatusValues.FirstOrDefault(x => string.Equals(((ItemStatus)x).name, "Deployed", StringComparison.InvariantCultureIgnoreCase)) as ItemStatus;
            updateStatusSearchFilter();
        }

        /// <summary>
        /// load ItemInstance based on item selected from list, ie load shadow object
        /// </summary>
        /// <param name="selListItem"></param>
        protected override void loadSelectedItem(ItemResult selListItem)
        {
            if ((selListItem?.pk != null) && (selListItem.pk != Guid.Empty))
            {
                selectedItem = db.db.Load<Item>(selListItem.pk);

                // if not currently editing anything then we match current item, but
                // we don't update currentItem otherwise as may be a clone, etc.
                if (isDetailViewInActive && EditCommand.CanExecute(null)) EditCommand.Execute(null);
            }
            else
            {
                selectedItem = null;
            }
        }

        #region Properties

        /// <summary>
        /// Are we working with items with a status of Available (true) or Deployed (false)?
        /// </summary>
        public bool StatusAvailable
        {
            get { return _StatusAvailable; }
            set
            {
                SetProperty(ref _StatusAvailable, value, nameof(StatusAvailable));
                RaisePropertyChanged(nameof(StatusDeployed));
                updateStatusSearchFilter();
            }
        }
        public bool StatusDeployed
        {
            get { return !_StatusAvailable; }
            set { StatusAvailable = !value; }
        }
        private bool _StatusAvailable = true;

        private void updateStatusSearchFilter()
        {
            // save if currently disabled or not so we don't enable too early (if currently not enabled)
            var wasEnabled = SearchFilter.SearchFilterEnabled;
            // force as disabled while we update
            SearchFilter.SearchFilterEnabled = false; 
            SearchFilter.SelectedItemStatusValues.Clear();
            if (_StatusAvailable)
            {
                // will auto change, but we want to happen immediately
                deployRecoverCommands.DeployRecoverItemInstanceCommandText = DeployRecoverItemInstanceAction.MenuDeploy;
                SearchFilter.SelectedItemStatusValues.Add(deployRecoverCommands.statusAvailable);
            }
            else
            {
                // will auto change, but we want to happen immediately
                deployRecoverCommands.DeployRecoverItemInstanceCommandText = DeployRecoverItemInstanceAction.MenuRecover;
                SearchFilter.SelectedItemStatusValues.Add(deployRecoverCommands.statusDeployed);
            }
            // trigger update is currently enabled
            SearchFilter.SearchFilterEnabled = wasEnabled; 
        }

        #endregion // Properties

    }
}
