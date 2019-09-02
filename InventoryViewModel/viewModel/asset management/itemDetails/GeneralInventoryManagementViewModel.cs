// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.userManager;



/*  search bar at top, it varies based on detail window
 *  result tree on the left of ItemInstance
 *  right side is detail window
 *      detail window is one of:
 *      ItemInstance details - can edit serial #, seal broken, print barcode, update notes; if admin In Service date & status
 *      History of ....
 *  bottom is actions
 */

namespace TEMS_Inventory.views
{
    public class GeneralInventoryManagementViewModel : ItemInstanceManagementViewModel
    {
        public GeneralInventoryManagementViewModel() : base() { }


        /// <summary>
        /// Command to open print labels window with this item selected so can be printed
        /// </summary>
        public ICommand OpenPrintBarcodeWindowCommand
        {
            get { return InitializeCommand(ref _OpenPrintBarcodeWindowCommand, param => DoOpenPrintBarcodeWindowCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _OpenPrintBarcodeWindowCommand;

        private void DoOpenPrintBarcodeWindowCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "Labels", searchText = (CurrentItem == null) ? null : itemNumber?.ToString() });
        }


        /// <summary>
        /// Command to add an item
        /// </summary>
        public ICommand AddServiceDetailsCommand
        {
            get { return InitializeCommand(ref _AddServiceDetailsCommand, param => DoAddServiceDetailsCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _AddServiceDetailsCommand;

        private void DoAddServiceDetailsCommand()
        {
            var serviceEvent = new ItemService()
            {
                //itemInstance = CurrentItem as ItemInstance
            };
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "ServiceDetails", args = serviceEvent });
        }


        /// <summary>
        /// Command to add an item that is initially same as selected item
        /// </summary>
        public ICommand ViewServiceHistoryCommand
        {
            get { return InitializeCommand(ref _ViewServiceHistoryCommand, param => DoViewServiceHistoryCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _ViewServiceHistoryCommand;

        private void DoViewServiceHistoryCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "Service", searchText = (CurrentItem == null) ? null : itemNumber?.ToString() });
        }


        /// <summary>
        /// Command to remove selected item from items collection and remove (delete or mark deleted) in back end DB
        /// </summary>
        public ICommand ViewDeployRecoverHistoryCommand
        {
            get { return InitializeCommand(ref _ViewDeployRecoverHistoryCommand, param => DoViewDeployRecoverHistoryCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _ViewDeployRecoverHistoryCommand;

        private void DoViewDeployRecoverHistoryCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "DeployRecover", searchText = (CurrentItem == null) ? null : itemNumber?.ToString() });
        }


        /// <summary>
        /// Command to actually persist currentItem, if newly added also adds to the list and sets as selected item
        /// </summary>
        public ICommand AddDamagedDetailsCommand
        {
            get { return InitializeCommand(ref _AddDamagedDetailsCommand, param => DoAddDamagedDetailsCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _AddDamagedDetailsCommand;

        private void DoAddDamagedDetailsCommand()
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
        }


        /// <summary>
        /// Command to actually persist currentItem, if newly added also adds to the list and sets as selected item
        /// </summary>
        public ICommand AddMissingDetailsCommand
        {
            get { return InitializeCommand(ref _AddMissingDetailsCommand, param => DoAddMissingDetailsCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _AddMissingDetailsCommand;

        private void DoAddMissingDetailsCommand()
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
        }

        /// <summary>
        /// Command to remove selected item from items collection and remove (delete or mark deleted) in back end DB
        /// </summary>
        public ICommand ViewDamagedMissingHistoryCommand
        {
            get { return InitializeCommand(ref _ViewDamagedMissingHistoryCommand, param => DoViewDamagedMissingHistoryCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _ViewDamagedMissingHistoryCommand;

        private void DoViewDamagedMissingHistoryCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "DamagedMissing", searchText = (CurrentItem == null) ? null : itemNumber?.ToString() });
        }

        /// <summary>
        /// Command to remove selected item from items collection and remove (delete or mark deleted) in back end DB
        /// </summary>
        public ICommand DeployRecoverItemCommand
        {
            get { return InitializeCommand(ref _DeployRecoverItemCommand, param => DoDeployRecoverItemCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _DeployRecoverItemCommand;

        private void DoDeployRecoverItemCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "DeployRecover", searchText = (CurrentItem == null) ? null : itemNumber?.ToString() });
        }

        


        #region SaveCommand
#if false

        /// <summary>
        /// Command to actually persist currentItem, if newly added also adds to the list and sets as selected item
        /// </summary>
        public ICommand SaveCommand
        {
            get { return InitializeCommand(ref _SaveCommand, param => DoSave(), param => CanSave()); }
        }
        private ICommand _SaveCommand;

        private bool CanSave()
        {
            // can only save (insert or update) if all NonNull constraints satisfied
            // Note: CurrentItem is null when selected item is a header only item
            if (CurrentItem != null)
                return CurrentItem.IsChanged && CurrentItem.AreNonNullConstraintsSatisfied();

            // if no current item, then nothing to persist
            return false;
        }

        /// <summary>
        /// saves to DB and updates status with results of save
        /// </summary>
        private void DoSave()
        {
            try
            {
                SaveEntity();
                StatusMessage = "Saved.";

                // update list
                {
                    // see if current item is already in list (i.e. editing versus a new addition)
                    // set selectedItem to either null or clone of currentItem
                    SearchResult currentSelectedItem = null;
                    var pk = CurrentItem?.PrimaryKey;
                    if (pk != null)
                    {
                        // null if no items found with matching PrimaryKey
                        //currentSelectedItem = Items.Where(i => pk.Equals(i.id)).FirstOrDefault();
                        currentSelectedItem = Items.FindItem((item) => { return item.id.Equals(pk); });
                    }

                    // update list with item (either in-place if editing, or at end if new item)
                    // changes not immediately shown until save as list shows SelectedItem which
                    // is a different object than CurrentItem
                    if (currentSelectedItem != null)
                    {
                        // replace old item with updated item in items collection
                        var pList = currentSelectedItem.parent.children;
                        var index = pList.IndexOf(currentSelectedItem);
                        currentSelectedItem = ConvertItemsValueFromCurrent(CurrentItem);
                        pList.RemoveAt(index);  // Note: we are removing currently selected item which will set SelectedItem to null, thus setting CurrentItem to null
                        pList.Insert(index, currentSelectedItem);
                    }
                    else
                    {
                        // Add a new item to collection
                        if (CurrentItem != null)
                        {
                            currentSelectedItem = ConvertItemsValueFromCurrent(CurrentItem);
                            Items.Add(currentSelectedItem);
                        }
                    }

                    // update which item is selected so set public property
                    SelectedItem = currentSelectedItem;
                }
            }
            catch (/*SavedFailed*/Exception e)
            {
                logger.Error(e, $"Failed to save {CurrentItem?.ToString()} - {e.Message}.");
                // let application continue
                StatusMessage = $"Save failed - {e.Message}";
            }

        }

        /// <summary>
        /// Perform the actual save, the default implementation saves CurrentItem
        /// This should be overridden if additional or other tasks are needed, e.g. saving
        /// unrelated entities as well as CurrentItem
        /// Note: should only be called if CanSave() is true, so CurrentItem will not
        /// be null, IsChanged is true, and NONNULL constraints should be satisfied.
        /// </summary>
        protected virtual void SaveEntity()
        {
            DataRepository.GetDataRepository.Save(CurrentItem);
        }
#endif
        #endregion SaveCommand
    }
}
