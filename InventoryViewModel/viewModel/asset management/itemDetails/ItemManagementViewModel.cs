// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.command.action;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.userManager;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Administration CRUD view model for updating Item table
    /// </summary>
    public class ItemManagementViewModel : ViewModelBase
    {
        public ItemManagementViewModel() : base() { }

        /// <summary>
        /// does active user have administrative privileges or just normal user privileges
        /// true if limited to user privileges
        /// </summary>
        public bool IsAdmin
        {
            get { return UserManager.GetUserManager.CurrentUser().isAdmin; }
        }

        /// <summary>
        /// Is there a currently active (non-null) item
        /// </summary>
        public bool IsActiveItem
        {
            get { return _CurrentItem != null; }
        }

        /// <summary>
        /// the current item for display (and edit)
        /// </summary>
        public Item CurrentItem
        {
            get { return _CurrentItem; }
            set
            {
                SetProperty(ref _CurrentItem, value, nameof(CurrentItem));
                RaisePropertyChanged(nameof(IsActiveItem));
            }
        }
        private Item _CurrentItem = null;


        public IList<Item> PossibleParents
        {
            get
            {
                if (_AllBinsAndModules == null) _AllBinsAndModules = DataRepository.GetDataRepository.AllBinsAndModules();

                if (CurrentItem != _childItem)
                {
                    _childItem = CurrentItem as Item;
                    if (_childItem == null)
                    {
                        // no item, no parent
                        _possibleParents = new List<Item>(0);
                    }
                    else if (_childItem.itemType == null)
                    {
                        // don't know what item is yet, so allow any bin or module or none
                        _possibleParents = _AllBinsAndModules;
                    }
                    else if (_childItem.itemType.isBin)
                    {
                        // bins are top level only
                        _possibleParents = new List<Item>(0);
                    }
                    else if (_childItem.itemType.isModule)
                    {
                        // modules are top level or in a bin only
                        _possibleParents = _AllBinsAndModules.Where(x => (x.unitType.unitCode == _childItem.unitType.unitCode) && x.itemType.isBin).ToList();
                    }
                    else /* !.isBin && !.isModule == .isItem */
                    {
                        // items can be top level or in a bin or module
                        _possibleParents = _AllBinsAndModules.Where(x => (x.unitType.unitCode == _childItem.unitType.unitCode)).ToList();
                    }
                }

                return _possibleParents;
            }
        }
        private IList<Item> _AllBinsAndModules = null;
        private Item _childItem = null;
        private IList<Item> _possibleParents = new List<Item>(0);


        #region Open ItemType edit Window

        /// <summary>
        /// Command to open edit ItemType window with this Item's ItemType selected so can be modified/viewed
        /// </summary>
        public ICommand OpenEditItemTypeWindowCommand
        {
            get { return InitializeCommand(ref _OpenEditItemTypeWindowCommand, param => DoOpenEditItemTypeWindowCommand(), param => IsActiveItem); }
        }
        private ICommand _OpenEditItemTypeWindowCommand;

        private void DoOpenEditItemTypeWindowCommand()
        {
            /*
            var viewModel = new ItemTypeManagementViewModel();
            var searchFilter = viewModel.SearchFilter;
            searchFilter.SearchFilterVisible = false;
            searchFilter.SearchText = (CurrentItem as Item)?.itemNumber?.ToString() ?? "";
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
            */
        }

        #endregion // Open item edit Window


        #region Open ItemType selection Dialog

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand OpenSelectItemTypeWindowCommand
        {
            get { return InitializeCommand(ref _OpenSelectItemTypeWindowCommand, param => DoOpenSelectItemTypeWindowCommand(), param => IsActiveItem); }
        }
        private ICommand _OpenSelectItemTypeWindowCommand;

        private void DoOpenSelectItemTypeWindowCommand()
        {
            var viewModel = new ItemTypeManagementViewModel();
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
        }

        #endregion // Open item edit Window


        /// <summary>
        /// Command to add an item that is initially same as selected item
        /// </summary>
        public ICommand CloneCommand
        {
            get
            {
                return InitializeCommand(
                    ref _CloneCommand,
                    param =>
                    {
                        var item = CurrentItem;
                        var clonedItem = DataRepository.GetDataRepository.GetInitializedItem(item.parent, item.itemType);
                        clonedItem.count = item.count;
                        clonedItem.expirationDate = item.expirationDate;
                        clonedItem.notes = item.notes;
                        clonedItem.vehicleCompartment = item.vehicleCompartment;
                        clonedItem.vehicleLocation = item.vehicleLocation;
                        CurrentItem = clonedItem;
                    },
                    param => { return (CurrentItem != null); }
                );
            }
        }
        private ICommand _CloneCommand;


        /// <summary>
        /// Command to add an item
        /// </summary>
        public ICommand AddCommand
        {
            get
            {
                return InitializeCommand(
                    ref _AddCommand,
                    param =>
                    {
                        // use current selection as parent for newly added item
                        var parent = (Item)CurrentItem;
                        // see if SelectedItem is a header and use it's parent as parent
                        if (parent == null)
                        {
                            /*
                            if ((SelectedItem != null) && (SelectedItem is GroupHeader))
                                parent = SelectedItem.parent.entity as Item;
                            */
                        }
                        // but if make sure it is bin or module, otherwise no parent (top level)
                        else if (!(parent.itemType.isBin || parent.itemType.isModule))
                        {
                            parent = parent.parent;
                        }
                        CurrentItem = DataRepository.GetDataRepository.GetInitializedItem(parent, null);
                    },
                    param => { return true; }
                );
            }
        }
        private ICommand _AddCommand;

        // ensure Item and ItemInstances correctly added together (used when saved not added)
        ICommand addItemCommand = new AddItemCommand();

        /// <summary>
        /// Perform the actual save, the default implementation saves CurrentItem
        /// This should be overridden if additional or other tasks are needed, e.g. saving
        /// unrelated entities as well as CurrentItem
        /// Note: should only be called if CanSave() is true, so CurrentItem will not
        /// be null, IsChanged is true, and NONNULL constraints should be satisfied.
        /// </summary>
        protected void SaveEntity()
        {
            // for each city with equipments item is in, then ensure an item instance exists (added only if Item does not already exist)
            if (addItemCommand.CanExecute(CurrentItem)) addItemCommand.Execute(CurrentItem);

            // and ensure changes are saved
            DataRepository.GetDataRepository.Save(CurrentItem);

            // after saving update tree (will reload from db hence must be done after saving)
            var childItem = CurrentItem as Item;
            /*
            if (SelectedItem?.parent?.parent?.parentId != childItem?.parentId)
            {
                if (SearchFilterCommand.CanExecute(null)) SearchFilterCommand.Execute(null);
                CurrentItem = childItem;
                if (CurrentItem == null) logger.Debug("CurrentItem null after saving.");
            }
            */
        }


        /// <summary>
        /// Command to remove an item (and associated ItemInstances)
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return InitializeCommand(
                    ref _DeleteCommand,
                    param =>
                    {
                        try
                        {
                            // remove from list
                            /*
                            var pList = SelectedItem.parent.children;
                            var index = pList.IndexOf(SelectedItem);
                            if (index >= 0) pList.RemoveAt(index);
                            */
                            // and from DB
                            DeleteItemCommand.Execute(CurrentItem);
                            // indicate nothing selected
                            CurrentItem = null; // SelectedItem is set to parent header
                        }
                        catch (Exception e)
                        {
                            // don't throw
                            StatusMessage = $"Failed to remove Item - {e.Message}";
                        }
                    },
                    param => { return DeleteItemCommand.CanExecute(CurrentItem); }
                );
            }
        }
        private ICommand _DeleteCommand;
        private ICommand DeleteItemCommand = new DeleteItemCommand();
    }
}
