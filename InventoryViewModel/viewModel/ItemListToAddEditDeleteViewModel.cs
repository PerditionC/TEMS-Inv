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
using TEMS.InventoryModel.entity;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    public class ItemListToAddEditDeleteViewModel : ViewModelBase
    {
        public ItemListToAddEditDeleteViewModel() : this(null) { }
        public ItemListToAddEditDeleteViewModel(Func<ItemBase> GetNewItem) : this(GetNewItem, new ObservableCollection<ItemBase>()) { }
        public ItemListToAddEditDeleteViewModel(Func<ItemBase> GetNewItem, ObservableCollection<ItemBase> items) : base()
        {
            Initialize(GetNewItem, items);
        }

        public void Initialize(Func<ItemBase> GetNewItem, ObservableCollection<ItemBase> items)
        {
            this.GetNewItem = GetNewItem;
            this.items = items ?? new ObservableCollection<ItemBase>();  // empty list instead of null
        }

        /// <summary>
        /// returns a new (default initialized) item when adding to list
        /// </summary>
        private Func<ItemBase> GetNewItem;

        // list of all items that currently exist
        // we use ItemBase so we can ensure all nonNull constraints are satisfied prior to saving
        private ObservableCollection<ItemBase> _items;
        public ObservableCollection<ItemBase> items
        {
            get { return _items; }
            set
            {
                currentItem = null;
                SetProperty(ref _items, value, nameof(items));
            }
        }

        // item within items that is currently selected, null if none currently selected
        private ItemBase _selectedItem = null;
        public ItemBase selectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value, nameof(selectedItem));
                RaisePropertyChanged(nameof(isItemSelected));
                // automatically setup selected item as item we are editing
                if (EditCommand.CanExecute(null)) EditCommand.Execute(null);
            }
        }


        // specific item that we are editing (or just added), may not be in items yet
        private ItemBase _currentItem = null;
        public ItemBase currentItem
        {
            get { return _currentItem; }
            set
            {
                SetProperty(ref _currentItem, value, nameof(currentItem));
                RaisePropertyChanged("isDetailViewInActive");
            }
        }

        // used to deactivate detail section if no backing current item
        public bool isDetailViewInActive
        {
            get { return currentItem == null; }
        }


        #region Commands

        private ICommand _AddCommand;
        private ICommand _CloneCommand;
        private ICommand _EditCommand;
        private ICommand _DeleteCommand;
        private ICommand _SaveCommand;

        /// <summary>
        /// Command to add an item
        /// </summary>
        public ICommand AddCommand
        {
            get { return InitializeCommand(ref _AddCommand, param => this.DoAdd(), param => this.CanAdd()); }
        }

        /// <summary>
        /// Command to add an item that is initially same as selected item
        /// </summary>
        public ICommand CloneCommand
        {
            get { return InitializeCommand(ref _CloneCommand, param => this.DoClone(), param => this.CanClone()); }
        }

        /// <summary>
        /// Command to enable making changes (edits) to selected item in items collection
        /// </summary>
        public ICommand EditCommand
        {
            get { return InitializeCommand(ref _EditCommand, param => this.DoEdit(), param => this.CanEdit()); }
        }

        /// <summary>
        /// Command to remove selected item from items collection and remove (delete or mark deleted) in backend DB
        /// </summary>
        public ICommand DeleteCommand
        {
            get { return InitializeCommand(ref _DeleteCommand, param => this.DoDelete(), param => this.CanDelete()); }
        }

        /// <summary>
        /// Command to actually persist currentItem, if newly added also adds to the list and sets as selected item
        /// </summary>
        public ICommand SaveCommand
        {
            get { return InitializeCommand(ref _SaveCommand, param => this.DoSave(), param => this.CanSave()); }
        }

        #endregion // Commands

        #region ICommand Predicates

        public bool isItemSelected
        {
            get { return selectedItem != null; }
        }

        private bool CanAdd()
        {
            // TODO should we warn if unsaved changes?
            return GetNewItem != null;
        }

        private bool CanClone()
        {
            // currently only allow cloning items with GUID primary key, as we can easily create new PK values
            if (isItemSelected)
            {
                Object pk = selectedItem.PrimaryKey;
                return (pk is Guid);
            }

            return false;
        }

        private bool CanEdit()
        {
            return isItemSelected;
        }

        private bool CanDelete()
        {
            // TODO verify item not in use also!
            return isItemSelected;
        }

        private bool CanSave()
        {
            // can only save (insert or update) if all NonNull constraints satisfied
            if (currentItem != null)
                return currentItem.IsChanged && currentItem.AreNonNullConstraintsSatisfied();

            // if no current item, then nothing to persist
            return false;
        }

        #endregion // ICommand Predicates

        #region ICommand Actions

        /// <summary>
        /// Creates a new (blank) item.
        /// Sets current item to newly created item.
        /// </summary>
        private void DoAdd()
        {
            currentItem = GetNewItem();

            // since editing a new item, nothing to have selected
            selectedItem = null;
        }

        /// <summary>
        /// Creates a new item, initialized from currently selected item (ie clone of selected item).
        /// Sets current item to newly created item.
        /// </summary>
        private void DoClone()
        {
            var clonedItem = selectedItem.GetClonedItem();
            // since we are adding a new item, important that its primary key is changed!
            Object pk = clonedItem.PrimaryKey;
            if (pk is Guid)
            {
                clonedItem.PrimaryKey = Guid.NewGuid();
            }
            else if (pk.GetType().IsPrimitive)
            {
                var typeCode = Type.GetTypeCode(pk.GetType());
                if (typeCode != TypeCode.Object && typeCode != TypeCode.Boolean /* && typeCode != TypeCode.Char */)
                {
                    // assume integer of some sort, and assume setting to 0 will cause it to be set via an AUTONUMBER mechanism
                    clonedItem.PrimaryKey = 0;
                }
                else
                {
                    // not really sure how a Boolean or other Primitive object would be a primary key but ...
                    clonedItem.PrimaryKey = null;
                }
            }
            else
            {
                throw new NotImplementedException("Item's primary key type is unknown/unhandled.");
            }

            // now update after all changes are complete, otherwise change sometimes missed
            currentItem = clonedItem;
            // since editing a new item, nothing to have selected  **** don't do this, it unsets current Item
            //selectedItem = null;
        }

        /// <summary>
        /// Sets current item to selected item
        /// </summary>
        private void DoEdit()
        {
            currentItem = selectedItem.GetClonedItem();
        }

        /// <summary>
        /// default implementation just deletes from database if db is open
        /// Allows subclasses to alter how deletion occurs, e.g. remove additional tables or files
        /// </summary>
        /// <param name="item"></param>
        protected virtual void DoDeleteItem(ItemBase item)
        {
            var db = DataRepository.GetDataRepository;
            if (db != null) db.Delete(selectedItem);
        }

        /// <summary>
        /// Removes the selected item from list of items including marking deleted in database
        /// </summary>
        private void DoDelete()
        {
            Mediator.InvokeCallback(nameof(YesNoDialogMessage),
                new YesNoDialogMessage
                {
                    caption = "Delete",
                    message = "Do you want to delete this item?",
                    NoAction = (x) => { /* do nothing */ },
                    YesAction = (x) =>
                    {
                        try
                        {
                            // actually remove from DB
                            DoDeleteItem(selectedItem);
                            // update display
                            items.Remove(selectedItem);
                            //RaisePropertyChanged(nameof(items));
                        }
                        catch (Exception e)
                        {
                            logger.Error(e, "Failed to delete selected item!");
                            //throw; swallow error, todo - show an error!
                        }
                    },

                });


            // either item removed (no item) or delete canceled/failed - either way don't leave item selected
            selectedItem = null;
            currentItem = null;
        }

        /// <summary>
        /// adds to display list and saves to DB if provided
        /// </summary>
        private void DoSave()
        {
            var db = DataRepository.GetDataRepository;

            try
            {
                db.Save(currentItem);

                // update list
                {
                    // see if current item is already in list (i.e. editing versus a new addition)
                    // set selectedItem to either null or clone of currentItem
                    ItemBase currentSelectedItem = null;
                    var pk = currentItem?.PrimaryKey;
                    if (pk != null)
                    {
                        // null if no items found with matching PrimaryKey
                        currentSelectedItem = items.Where(i => pk.Equals(i.PrimaryKey)).FirstOrDefault();
                    }

                    // update list with item (either in-place if editing, or at end if new item)
                    // Note: use clone instead of actual item so future changes to currentItem not
                    // immediately shown in list - user must activate Save command to persist changes.
                    if (currentSelectedItem != null)
                    {
                        // replace old item with updated item in items collection
                        var index = items.IndexOf(currentSelectedItem);
                        items.RemoveAt(index);  // Note: we are removing currently selected item which will set SelectedItem to null, thus setting CurrentItem to null
                        items.Insert(index, currentItem);
                    }
                    else
                    {
                        // Add a new item to collection
                        items.Add(currentItem);
                    }

                    // update which item is selected so set public property
                    selectedItem = currentItem;
                    //RaisePropertyChanged("items"); // TODO should just trigger new item added change
                }
            }
            catch (SavedFailedException e)
            {
                logger.Error(e, $"Failed to save item {currentItem.ToString()}.");
                // TODO warn user save failed!!!
            }
        }

        #endregion // ICommand Actions
    }
}
