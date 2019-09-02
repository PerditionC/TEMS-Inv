// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using SQLite;
using TEMS.InventoryModel.util.attribute;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Details about a specific item within an equipment unit,
    /// same for all items of this type within the same physical bin/module/spot within this equipment unit
    /// </summary>
    public class Item : ItemBase
    {
        public Item(Guid id) : base(typeof(Item).GetProperty(nameof(Item.id)))
        {
            _id = id;
        }

        public Item() : this(Guid.NewGuid())
        {
        }

        // internal DB primary key, unique per item
        // Note: GUID used for replication purposes
        [PrimaryKey]
        [HideProperty]
        public Guid id
        {
            get { return _id; }
            set
            {
                SetProperty(ref _id, value, nameof(id), new string[] { nameof(PrimaryKey) });
                //SetProperty(ref _id, value, nameof(id), new List<string>() { nameof(PrimaryKey) });
                //RaisePropertyChanged(nameof(PrimaryKey));
            }
        }
        private Guid _id;

        // external id #, multiple items may have same itemId if represents the same item just for a different siteLocation
        // Note: identical itemId implies itemNumber differs only by locSuffix
        [NotNull]
        public int itemId
        {
            get { return _itemId; }
            set
            {
                SetProperty(ref _itemId, value, nameof(itemId));
                RaisePropertyChanged(nameof(itemNumber));
                RaisePropertyChanged(nameof(itemDescription));
            }
        }
        private int _itemId = 0;

        // partial external id, does not include site location
        // e.g. D236-19807
        [SQLite.Ignore]
        //[DisplayNameProperty]
        public string itemNumber
        {
            get { return unitType?.unitCode + string.Format("{0:D}-{1:D}", itemType?.itemTypeId, itemId /* oldItemId */); }
        }

        // partial external id, does not include site location
        // e.g. D236-19807
        [SQLite.Ignore]
        [DisplayNameProperty]
        public string itemDescription
        {
            get { return $"{itemNumber} : {itemType?.name}"; }
        }

        // general details about this item
        [ManyToOne(nameof(itemTypeId))] // in DB as foreign key for ItemType table
        public ItemType itemType
        {
            get { return _itemType; }
            set
            {
                SetProperty(ref _itemType, value, nameof(itemType));
                itemTypeId = _itemType?.id ?? Guid.Empty;
                RaisePropertyChanged(nameof(itemNumber));
                RaisePropertyChanged(nameof(itemDescription));
            }
        }
        private ItemType _itemType;

        [NotNull, ForeignKey(nameof(itemType))]
        public Guid itemTypeId { get { return _itemTypeId; } set { SetProperty(ref _itemTypeId, value, nameof(itemTypeId)); } }
        private Guid _itemTypeId = Guid.Empty;


        // vehicle location, usually trailer
        [ManyToOne(nameof(vehicleLocationId))] // in DB as foreign key for VehicleLocation table
        public VehicleLocation vehicleLocation
        {
            get { return _vehicleLocation; }
            set
            {
                SetProperty(ref _vehicleLocation, value, nameof(vehicleLocation));
                vehicleLocationId = _vehicleLocation?.id ?? Guid.Empty;
            }
        }
        private VehicleLocation _vehicleLocation;

        [NotNull, ForeignKey(nameof(vehicleLocation))]
        public Guid vehicleLocationId { get { return _vehicleLocationId; } set { SetProperty(ref _vehicleLocationId, value, nameof(vehicleLocationId)); } }
        private Guid _vehicleLocationId = Guid.Empty;

        // vehicle compartment, if not trailer then description of where in vehicle
        [MaxLength(32)]
        public string vehicleCompartment { get { return _vehicleCompartment; } set { SetProperty(ref _vehicleCompartment, value, nameof(vehicleCompartment)); } }
        private string _vehicleCompartment = null;

        // for non-serialized items, how many of this specific item (same type, location, bin, etc)
        [NotNull]
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

        // bin or module assigned to, null if not in a bin or module; in DB as foreign key for bin/module in Item (this) table
        [ManyToOne(nameof(parentId))]
        public Item parent
        {
            get { return _parent; }
            set
            {
                SetProperty(ref _parent, value, nameof(parent));
                // update foreign key when value changes!
                parentId = _parent?.id;
            }
        }
        private Item _parent = null;

        [ForeignKey(nameof(parent))]
        public Guid? parentId { get { return _parentId; } set { SetProperty(ref _parentId, value, nameof(parentId)); } }
        private Guid? _parentId = null;

        // additional remarks about item
        [MaxLength(255)]
        public string notes { get { return _notes; } set { SetProperty(ref _notes, value, nameof(notes)); } }
        private string _notes = null;

        // below items are set when item is created or cloned

        // equipment type item belongs to (MMRS, DMSU, SSU)
        [ManyToOne(nameof(unitTypeName))] // in DB as foreign key for EquipmentUnitType table
        public EquipmentUnitType unitType
        {
            get { return _unitType; }
            set
            {
                SetProperty(ref _unitType, value, nameof(unitType));
                // update foreign key when value changes!
                unitTypeName = _unitType.name;
            }
        }
        private EquipmentUnitType _unitType = null;

        [ForeignKey(nameof(unitType)), MaxLength(6)]
        public string unitTypeName { get { return _unitName; } set { SetProperty(ref _unitName, value, nameof(unitTypeName)); } }
        private string _unitName = null;
    }
}