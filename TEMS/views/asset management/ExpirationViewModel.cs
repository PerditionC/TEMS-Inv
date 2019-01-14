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
    public class ExpirationViewModel : BasicListAndDetailWithSearchFilterWindowViewModel
    {
        // anything that needs initializing for MSVC designer
        public ExpirationViewModel() : base() { }

        /// <summary>
        /// initialize our SearchFilter view model and any other controls needing intializing
        /// Note: moved out of constructor to avoid issues with MSVC design viewer
        /// </summary>
        public override void Initialize(DataRepository db, Func<ItemBase> GetNewItem)
        {
            base.Initialize(db, GetNewItem);
            // site & status are specific to an instance so don't use
            SearchFilter.SiteLocationVisible = false;
            SearchFilter.SiteLocationEnabled = false;
            SearchFilter.SelectItemStatusValuesVisible = false;
            SearchFilter.SelectItemStatusValuesEnabled = false;
            // and these are specific to item not item type
            SearchFilter.SelectEquipmentUnitsVisible = false;
            SearchFilter.SelectEquipmentUnitsEnabled = false;
        }

        /// <summary>
        /// load ItemResult based on item selected from list, ie load shadow object
        /// </summary>
        /// <param name="selListItem"></param>
        protected override void loadSelectedItem(ItemResult selListItem)
        {
            if ((selListItem?.pk != null) && (selListItem.pk != Guid.Empty))
            {
                selectedItem = db.db.Load<ItemType>(selListItem.pk);

                // if not currently editing anything then we match current item, but
                // we don't update currentItem otherwise as may be a clone, etc.
                if (isDetailViewInActive && EditCommand.CanExecute(null)) EditCommand.Execute(null);
            }
            else
            {
                selectedItem = null;
            }
        }


        protected override void DoSearch()
        {
            logger.Debug("Loading item types - DoSearch:\n" + SearchFilter.ToString());

            items = db.GetItemTypeList(SearchFilter);
            // autoselect if only 1 item type returned
            if (items.Count == 1) selectedListItem = items[0];

        }



        #region Commands

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand OpenManageVendorsWindowCommand
        {
            get { return InitializeCommand(ref _OpenManageVendorsWindowCommand, param => DoOpenManageVendorsWindowCommand(), param => isCurrentItem()); }
        }
        private ICommand _OpenManageVendorsWindowCommand;

        #endregion // Commands

        #region ICommand Actions

        private void DoOpenManageVendorsWindowCommand()
        {
            var newWin = new ManageVendorsWindow();
            ShowChildWindow(newWin);
        }

        #endregion // ICommand Actions
    }
}
