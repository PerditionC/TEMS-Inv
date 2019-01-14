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
    public class DamagedMissingItemSelectViewModel : BasicListAndDetailWithSearchFilterWindowViewModel
    {
        // anything that needs initializing for MSVC designer
        public DamagedMissingItemSelectViewModel() : base() { }

        /// <summary>
        /// initialize our SearchFilter view model and any other controls needing intializing
        /// Note: moved out of constructor to avoid issues with MSVC design viewer
        /// </summary>
        public override void Initialize(DataRepository db, Func<ItemBase> GetNewItem)
        {
            base.Initialize(db, GetNewItem);
            SearchFilter.SelectItemStatusValuesVisible = false;
            //statusAvailable = SearchFilter.ItemStatusValues.FirstOrDefault(x => string.Equals(((ItemStatus)x).name, "Available", StringComparison.InvariantCultureIgnoreCase)) as ItemStatus;
            //statusDeployed = SearchFilter.ItemStatusValues.FirstOrDefault(x => string.Equals(((ItemStatus)x).name, "Deployed", StringComparison.InvariantCultureIgnoreCase)) as ItemStatus;
        }

        /// <summary>
        /// load ItemInstance based on item selected from list, ie load shadow object
        /// </summary>
        /// <param name="selListItem"></param>
        /// <returns></returns>
        protected override void loadSelectedItem(ItemResult selListItem)
        {
            if ((selListItem?.instancePk != null) && (selListItem.instancePk != Guid.Empty))
            {
                selectedItem = db.db.Load<ItemInstance>(selListItem.instancePk);
                // we don't use DoEdit as we don't need a clone of shadow selectedItem for currentItem
                currentItem = selectedItem;  //DoEdit();
                // Note: need same object so changes triggered via items bound to selectedListItem will show in detail view
                selListItem.entity = currentItem;
            }
            else
            {
                selectedItem = null;
                currentItem = null;
            }
        }

        #region Commands

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand DamagedCommand
        {
            get { return InitializeCommand(ref _DamagedCommand, param => DoDamagedCommand(), param => isCurrentItem()); }
        }
        private ICommand _DamagedCommand;

        private void DoDamagedCommand()
        {
            var newWin = new DamagedMissingDetailsWindow();
            //searchFilter.SearchText = (currentItem as ItemInstance)?.itemNumber?.ToString() ?? "";
            ShowChildWindow(newWin);
        }


        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand MissingCommand
        {
            get { return InitializeCommand(ref _MissingCommand, param => DoMissingCommand(), param => isCurrentItem()); }
        }
        private ICommand _MissingCommand;

        private void DoMissingCommand()
        {
            var newWin = new DamagedMissingDetailsWindow();
            ShowChildWindow(newWin);
        }

        #endregion // Commands
    }
}
