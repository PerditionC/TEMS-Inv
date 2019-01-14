// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.command.action;
using TEMS.InventoryModel.entity;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    public abstract class MasterDetailItemWindowViewModelBase : SearchWindowViewModelBase
    {
        public ReferenceDataCache cache { get; private set; } = DataRepository.GetDataRepository.ReferenceData;

        public MasterDetailItemWindowViewModelBase(QueryResultEntitySelector resultEntitySelector) : this(resultEntitySelector, null) { }

        // anything that needs initializing
        public MasterDetailItemWindowViewModelBase(QueryResultEntitySelector resultEntitySelector, SearchFilterItems SearchFilter) : base(resultEntitySelector, SearchFilter)
        {
            // from this point on, trigger changes based on any changes to our SearchFilter
            PropertyChanged += MasterDetailItemWindowViewModelBase_PropertyChanged;
        }

        /// <summary>
        /// Called when any changes occur to ourselves, used to trigger loading a new CurrentItem
        /// when SelectedItem changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MasterDetailItemWindowViewModelBase_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!"IsChanged".Equals(e.PropertyName, StringComparison.InvariantCulture)) 
                logger.Debug($"MasterDetailItemWindowViewModelBase_PropertyChanged [{e.PropertyName}]\n");

            // if SelectedItem changed, also notify that CurrentItem has changed (since based on SelectedItem)
            if (string.Empty.Equals(e.PropertyName) || nameof(SelectedItem).Equals(e.PropertyName, StringComparison.InvariantCulture))
            {
                RaisePropertyChanged(nameof(CurrentItem));
            }
        }

        /// <summary>
        /// Called to convert a CurrentItem into a value to insert into Items collection.
        /// Only called if CurrentItem is set to a value that does not correspond to any
        /// existing SearchResult items in the Items collection; i.e. a newly created CurrentItem
        /// </summary>
        /// <param name="value">the current ItemBase object to convert from</param>
        /// <returns>the GenericItemResult that represents the ItemBase object</returns>
        protected virtual GenericItemResult ConvertItemsValueFromCurrent(ItemBase value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// returns a new (default initialized) item (for adding a new item)
        /// subclasses should initialize with a valid function if Adding is allowed
        /// </summary>
        protected Func<ItemBase> GetNewItem = null;

        /// <summary>
        /// actual item instance (not search result) selected from last Search (or null if nothing currently selected)
        /// </summary>
        public ItemBase CurrentItem
        {
            get
            {
                if (SelectedItem == null)
                {
                    return null;
                }
                else
                {
                    try
                    {
                        // if we have already loaded from db or just created then reuse existing reference, otherwise load it
                        if (SelectedItem.entity == null)
                        {
                            if ((SelectedItem is GenericItemResult) && (SelectedItem.id != Guid.Empty))
                            {
                                try
                                {
                                    var db = DataRepository.GetDataRepository;
                                    switch (SearchFilterCommand.resultEntitySelector)
                                    {
                                        case QueryResultEntitySelector.ItemInstance:
                                            CurrentItem = db.Load<ItemInstance>(SelectedItem.id);
                                            break;
                                        case QueryResultEntitySelector.Item:
                                            CurrentItem = db.Load<Item>(SelectedItem.id);
                                            break;
                                        case QueryResultEntitySelector.ItemType:
                                            CurrentItem = db.Load<ItemType>(SelectedItem.id);
                                            break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    // for now eat the exception, we expect this when newly added or cloned item not yet saved
                                }
                            }
                        }
                        // may still return null if SelectedItem does not correspond to DB item
                        return SelectedItem.entity;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, $"Error loading CurrentItem in {nameof(MasterDetailItemWindowViewModelBase_PropertyChanged)} - {ex.Message}");
                        throw;
                    }
                }
            }

            set
            {
                if (value == null)
                {
                    SelectedItem = null;
                }
                else
                {
                    // warn about possible loss of data if something selected and changed without being saved
                    if (IsSelectedItem && (SelectedItem.entity != null) && SelectedItem.entity.IsChanged && (SelectedItem.entity != value))
                    {
                        Mediator.InvokeCallback(nameof(YesNoDialogMessage),
                            new YesNoDialogMessage
                            {
                                caption = $"{CurrentItem.displayName} has been changed",
                                message = "Current item has been modified, do you wish to save changes?",
                                NoAction = (x) => { /* force reloading from db so changes are lost! */ SelectedItem.entity = null; },
                                YesAction = (x) => { if (CanSave()) DoSave(); },
                                ActionArgs = CurrentItem
                            });
                    }

                    // we need to find in Items then update SelectedItem
                    if (SelectedItem?.id != (Guid)(value.PrimaryKey))
                    {
                        var selectedItem = Items.FindItem((item) => { return item.id.Equals(value.PrimaryKey); });
                        if (selectedItem == null)
                        {
                            // add new item to Items collection (new item created)
                            selectedItem = ConvertItemsValueFromCurrent(value);
                            //Items.Add(selectedItem);
                        }

                        SelectedItem = selectedItem as GenericItemResult;
                    }

                    SelectedItem.entity = value;
                }

                // invoke message to notify others
                Mediator.InvokeCallback(nameof(CurrentItemChangedMessage), new CurrentItemChangedMessage() { CurrentItem = value as ItemInstance, SelectedItem = SelectedItem });
            }
        }

        #region SaveCommand

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
                logger.Error(e, $"Failed to save {CurrentItem.ToString()} - {e.Message}.");
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

        #endregion SaveCommand
    }
}
