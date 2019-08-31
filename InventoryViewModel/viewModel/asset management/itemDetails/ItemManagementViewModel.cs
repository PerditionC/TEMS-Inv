// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Administration CRUD view model for updating Item table
    /// </summary>
    public class ItemManagementViewModel : ItemDetailsViewModel
    {
        public ItemManagementViewModel() : base() { }

        /// <summary>
        /// Initialize to nothing selected to display details of
        /// </summary>
        public override void clear()
        {
            base.clear();

            itemId = 0;
            itemType = null;
            vehicleLocation = null;
            vehicleCompartment = null;
            count = 1;
            expirationDate = null;
            parent = null;
            notes = null;
            unitType = null;
        }


        /// <summary>
        /// returns a List of all possible Item's that could contain the currently selected item
        /// </summary>
        public IList<GenericItemResult> PossibleParents
        {
            get
            {
                if (_AllBinsAndModules == null)
                {
                    _AllBinsAndModules = DataRepository.GetDataRepository.AllBinsAndModules();
                }

                if (itemType == null)
                {
                    // don't know what item is yet, so allow any bin or module or none
                    _possibleParents = _AllBinsAndModules;
                }
                else if (itemType.isBin)
                {
                    // bins are top level only
                    _possibleParents = new List<GenericItemResult>(0);
                }
                else if (itemType.isModule)
                {
                    // modules are top level or in a bin only
                    _possibleParents = _AllBinsAndModules.Where(x => ((unitType == null) || unitType.name.Equals(x.unitTypeName, StringComparison.InvariantCultureIgnoreCase)) && x.isBin).ToList();
                }
                else /* !.isBin && !.isModule == .isItem */
                {
                    // items can be top level or in a bin or module
                    _possibleParents = _AllBinsAndModules.Where(x => (unitType == null) || unitType.name.Equals(x.unitTypeName, StringComparison.InvariantCultureIgnoreCase)).ToList();
                }

                // need to also update selected item, note we set underlying value and assume same trigger to retrieve PossibleParents triggers retrieval of new value, needed so we don't replace the parent property
                _SelectedParent = _possibleParents.Where(x => x.id == parent?.id).FirstOrDefault();
                return _possibleParents;
            }
        }
        private IList<GenericItemResult> _AllBinsAndModules = null;
        private IList<GenericItemResult> _possibleParents = new List<GenericItemResult>(0);


        #region Open ItemType edit Window

        /// <summary>
        /// Command to open edit ItemType window with this Item's ItemType selected so can be modified/viewed
        /// </summary>
        public ICommand OpenEditItemTypeWindowCommand
        {
            get { return InitializeCommand(ref _OpenEditItemTypeWindowCommand, param => DoOpenEditItemTypeWindowCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _OpenEditItemTypeWindowCommand;

        private void DoOpenEditItemTypeWindowCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "ManageItemTypes", searchText = (CurrentItem == null) ? null : itemNumber?.ToString() });
        }

        #endregion // Open item edit Window


        #region Open ItemType selection Dialog

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand OpenSelectItemTypeWindowCommand
        {
            get { return InitializeCommand(ref _OpenSelectItemTypeWindowCommand, param => DoOpenSelectItemTypeWindowCommand(), param => IsCurrentItemNotNull); }
        }
        private ICommand _OpenSelectItemTypeWindowCommand;

        private void DoOpenSelectItemTypeWindowCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "SelectItemType", callback = (x) => { if (x != null) if (x is ItemType newItemType) itemType = newItemType; } });
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
                        // get a new unique id, upon saving this will then map to a new (cloned) record
                        var defaultItem = DataRepository.GetDataRepository.GetInitializedItem(parent, itemType);
                        guid = defaultItem.id;
                        itemId = defaultItem.itemId;
                        CurrentItem.id = guid;
                        CurrentItem.entity = null;
                    },
                    param => { return (CurrentItem != null) && (CurrentItem is GenericItemResult); }  // Item is selected and actual Item not header placeholder
                );
            }
        }
        private ICommand _CloneCommand;


        /// <summary>
        /// Command to add an item
        /// An item is added to the hierarchy based on current selected
        ///     current is a top level object or bin or module then add to it
        ///     otherwise add to current selection's parent
        ///     i.e. click add with a bin or bin header selected add to that bin
        ///     but click add with a non-bin/non-module item or Items header selected then add to parent bin/module/trailer
        /// </summary>
        public ICommand AddCommand
        {
            get
            {
                return InitializeCommand(
                    ref _AddCommand,
                    param =>
                    {
                        // determine parent
                        Item parent;
                        if (CurrentItem is GenericItemResult)
                        {
                            if ((itemType != null) && (itemType.isBin || itemType.isModule))
                            {
                                parent = (CurrentItem?.entity as Item);
                            }
                            else
                            {
                                parent = (CurrentItem?.entity as Item)?.parent;
                            }
                        }
                        else
                        {
                            throw new NotImplementedException("Add logic for adding item with header selected!");
                        }

                        // create the object
                        var defaultItem = DataRepository.GetDataRepository.GetInitializedItem(parent, itemType);
                        guid = defaultItem.id;
                        itemId = defaultItem.itemId;
                        CurrentItem.id = guid;
                        CurrentItem.entity = null;

                        // at this point since this VM reflects an Item that does not yet exist in DB - attempts to retrieve CurrentItem.entity will fail!
                    },
                    param => { return CurrentItem?.entityType != null; }  // simple flag, we set entityType to "Item" after saved, otherwise an unsaved potentially not fully valid Item so can't Add again yet
                );
            }
        }
        private ICommand _AddCommand;




        #region Item properties

        // external id #, multiple items may have same itemId if represents the same item just for a different siteLocation
        // Note: identical itemId implies itemNumber differs only by locSuffix
        public int itemId { get { return _itemId; } set { SetProperty(ref _itemId, value, nameof(itemId)); RaisePropertyChanged(nameof(itemNumber)); } }
        private int _itemId = 0;

        // partial external id, does not include site location
        // e.g. D236-19807
        public string itemNumber
        {
            get
            {
                if (CurrentItem is null)
                {
                    return "No selection";
                }
                else if (CurrentItem is GenericItemResult)
                {
                    return unitType?.unitCode + string.Format("{0:D}-{1:D}", itemType?.itemTypeId, itemId /* oldItemId */);
                }
                else if (CurrentItem is GroupHeader)
                {
                    if (CurrentItem is BinGroupHeader)
                        return "Bin";
                    else if (CurrentItem is ModuleGroupHeader)
                        return "Module";
                    else if (CurrentItem is ItemGroupHeader)
                        return "Item";
                    else
                        return "...";
                }
                else
                {
                    return "";
                }
            }
        }

        // general details about this item
        public ItemType itemType
        {
            get { return _itemType; }
            set
            {
                SetProperty(ref _itemType, value, nameof(itemType));
                RaisePropertyChanged(nameof(itemNumber));
                RaisePropertyChanged(nameof(PossibleParents));
                RaisePropertyChanged(nameof(SelectedParent));
            }
        }
        private ItemType _itemType;

        // vehicle location, usually trailer
        public VehicleLocation vehicleLocation
        {
            get { return _vehicleLocation; }
            set
            {
                SetProperty(ref _vehicleLocation, value, nameof(vehicleLocation));
            }
        }
        private VehicleLocation _vehicleLocation;

        // vehicle compartment, if not trailer then description of where in vehicle
        public string vehicleCompartment { get { return _vehicleCompartment; } set { SetProperty(ref _vehicleCompartment, value, nameof(vehicleCompartment)); } }
        private string _vehicleCompartment = null;

        // for non-serialized items, how many of this specific item (same type, location, bin, etc)
        public int count
        {
            get { return _count; }
            set
            {
                // count can not be negative
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("count", value, "count of items must be nonnegative (>=0)");
                }
                SetProperty(ref _count, value, nameof(count));
            }
        }

        private int _count = 1;

        // for items that expire, when expires; null if does not expire
        // update with new expiration date (reset) when item replenished/restocked
        // Note: items currently expire & are replaced at same time for any item of a given type regardless of site location
        // however, may not be replaced on all differing equipments at same time; i.e. all widgets in MMRS replaced at
        // same time, but widgets in DMSU may be done at a different time.  Move to ItemInstance if expiration becomes site controlled.
        // see ItemType.expirationRestockCategory to determine if expirationDate required and if annual date or date specific
        public DateTime? expirationDate { get { return _expirationDate; } set { SetProperty(ref _expirationDate, value, nameof(expirationDate)); } }
        private DateTime? _expirationDate = null;

        public bool RequiresExpirationDate
        {
            get
            {
                try
                {
                    if (itemType?.expirationRestockCategory != null)
                    {
                        return ((itemType?.expirationRestockCategory ?? ExpirationCategory.None) != ExpirationCategory.None);
                    }
                }
                catch (Exception e)
                {
                    logger.Warn(e, $"Failed to determine if current item requires expiration date - {e.Message}");
                }
                return false;
            }
        }


        // bin or module assigned to, null if not in a bin or module; in DB as foreign key for bin/module in Item (this) table
        public Item parent
        {
            get { return _parent; }
            set
            {
                SetProperty(ref _parent, value, nameof(parent));
            }
        }
        private Item _parent = null;
        public GenericItemResult SelectedParent
        {
            get { return _SelectedParent; }
            set
            {
                SetProperty(ref _SelectedParent, value, nameof(SelectedParent));
                if (value ==null)
                {
                    parent = null;
                }
                else
                {
                    parent = _SelectedParent.entity as Item;
                }
            }
        }
        private GenericItemResult _SelectedParent = null;


        // additional remarks about item
        public string notes { get { return _notes; } set { SetProperty(ref _notes, value, nameof(notes)); } }
        private string _notes = null;

        // below items are set when item is created or cloned

        // equipment type item belongs to (MMRS, DMSU, SSU)
        public EquipmentUnitType unitType
        {
            get { return _unitType; }
            set
            {
                SetProperty(ref _unitType, value, nameof(unitType));
                RaisePropertyChanged(nameof(itemNumber));
                RaisePropertyChanged(nameof(PossibleParents));
            }
        }
        private EquipmentUnitType _unitType = null;

        #endregion // Item properties
    }
}
