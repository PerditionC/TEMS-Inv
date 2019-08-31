// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.ObjectModel;
using SQLite;
using TEMS.InventoryModel.util.attribute;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// See expirationRestockCategory, possible values for item expiration and restock category
    /// </summary>
    public enum ExpirationCategory
    {
        None = 0,
        AnnualRestock = 1,
        DateSpecificRestock = 2
    }

    /// <summary>
    /// General details about given item
    /// </summary>
    public class ItemType : ReferenceData
    {
        public ItemType(Guid id) : base(id, typeof(ItemType).GetProperty(nameof(ItemType.id))) { }
        public ItemType() : this(Guid.NewGuid()) { }
        //public ItemType(ItemType copyOfObj) : base(copyOfObj) { /* TODO */ }

        // id (Primary Key) and name (DisplayNameProperty) are derived from ReferenceData
        // where id is internal DB primary key, unique per item type, GUID used for replication
        // and name is the item name/description, e.g. XL Rubber Gloves


        // external id #, items with same item type id correspond to items of same type
        // Note: identical itemId implies itemNumber differs only by locSuffix
        // WARNING: we currently save via INSERT OR REPLACE, if PK or UNIQUE items conflict the old
        // item is replaced with the new item instead of inserting a new item!
        [NotNull, Unique]
        public int itemTypeId { get { return _itemTypeId; } set { SetProperty(ref _itemTypeId, value, nameof(itemTypeId)); } }
        private int _itemTypeId = 0;

        // item make, manufacturer or name brand, e.g. Abbott
        [MaxLength(64)]
        public string make { get { return _make; } set { SetProperty(ref _make, value, nameof(make)); } }
        private string _make = null;

        // item model number, e.g. Precision Xtra
        [MaxLength(64)]
        public string model { get { return _model; } set { SetProperty(ref _model, value, nameof(model)); } }
        private string _model = null;

        // None if does not expire, otherwise is expiration an annual date or date specific restock
        public ExpirationCategory expirationRestockCategory
        {
            get { return _expirationRestockCategory; }
            set { SetProperty(ref _expirationRestockCategory, value, nameof(expirationRestockCategory)); }
        }
        private ExpirationCategory _expirationRestockCategory = ExpirationCategory.None;

        // how much a single unit of item costs in 10th cents
        // *** Assume any price is rounded to the nearest 10th of a cent and displayed in dollars as needed
        // *** Only valid for this item's vendor (i.e. determined by vendor)
        [NotNull]
        public long cost { get { return _cost; } set { SetProperty(ref _cost, value, nameof(cost)); } }
        private long _cost = 0;

        // how much a single unit of item weighs
        public double weight { get { return _weight; } set { SetProperty(ref _weight, value, nameof(weight)); } }
        private double _weight;

        // unit of measure, what determines a single unit
        [ManyToOne(nameof(unitOfMeasureId))] // in DB as foreign key for UnitOfMeasure table
        public UnitOfMeasure unitOfMeasure
        {
            get { return _unitOfMeasure; }
            set
            {
                SetProperty(ref _unitOfMeasure, value, nameof(unitOfMeasure));
                // update foreign key when value changes!
                unitOfMeasureId = _unitOfMeasure.id;
            }
        }
        private UnitOfMeasure _unitOfMeasure = null;
        [ForeignKey(nameof(unitOfMeasure))]
        public Guid unitOfMeasureId
        {
            get { return _unitOfMeasureId; }
            set { SetProperty(ref _unitOfMeasureId, value, nameof(unitOfMeasureId)); }
        }
        private Guid _unitOfMeasureId;

        // item category, purpose of item, e.g. Treatment
        [ManyToOne(nameof(itemCategoryId))]
        [FieldLabel(PrettyName: "Category")]
        public ItemCategory category
        {
            get { return _category; }
            set
            {
                SetProperty(ref _category, value, nameof(category));
                // update foreign key when value changes!
                itemCategoryId = _category.id;
            }
        }
        private ItemCategory _category = null;
        [ForeignKey(nameof(category))]
        public Guid itemCategoryId
        {
            get { return _itemCategoryId; }
            set { SetProperty(ref _itemCategoryId, value, nameof(itemCategoryId)); }
        }
        private Guid _itemCategoryId;

        // count of batteries item requires, 0 if it doesn't require any
        [NotNull]
        public int batteryCount
        {
            get { return _batteryCount; }
            set { SetProperty(ref _batteryCount, value, nameof(batteryCount)); }
        }
        private int _batteryCount = 0;

        // what type of batteries item requires, set to predefined type "None" if does not require any
        [ManyToOne(nameof(batteryTypeId))]  // in DB as foreign key for BatteryType table
        [FieldLabel(PrettyName: "Battery 55 Type", ToolTip = "Which battery item requires, use 'None' if no batteries are required.")]
        public BatteryType batteryType
        {
            get { return _batteryType; }
            set
            {
                SetProperty(ref _batteryType, value, nameof(batteryType));
                // update foreign key when value changes!
                batteryTypeId = _batteryType.id;
            }
        }
        private BatteryType _batteryType = null;
        [ForeignKey(nameof(batteryType))]
        [FieldLabel(PrettyName: "Battery Type ID", ToolTip = "Database primary key for battery information.")]
        public Guid batteryTypeId
        {
            get { return _batteryTypeId; }
            set { SetProperty(ref _batteryTypeId, value, nameof(batteryTypeId)); }
        }
        private Guid _batteryTypeId;

        // additional items (not tracked separately) associated with this one
        [MaxLength(32)]
        //[FieldLabel(PrettyName: "Additional Items", ToolTip = "Additional items (not tracked separately) associated with this one.")]
        public string associatedItems { get { return _associatedItems; } set { SetProperty(ref _associatedItems, value, nameof(associatedItems)); } }
        private string _associatedItems = null;

        // is this a bin (can contain other bins, modules, or items), DB should add an index for this value
        public bool isBin
        {
            get { return _isBin; }
            set
            {
                SetProperty(ref _isBin, value, nameof(isBin));
                if (value && _isModule) isModule = false; // can not be both
            }
        }
        private bool _isBin;

        // is this a modules (can contain other modules or items), DB should add an index for this value
        public bool isModule
        {
            get { return _isModule; }
            set
            {
                SetProperty(ref _isModule, value, nameof(isModule));
                if (value && _isBin) isBin = false; // can not be both
            }
        }
        private bool _isModule;

        //SELECT distinct VendorDetail.id, VendorDetail.name, ItemType.name, ItemType.id FROM ItemType JOIN (Item JOIN (ItemInstance JOIN (VendorSiteAccountInfo JOIN VendorDetail ON VendorSiteAccountInfo.vendorDetailId=VendorDetail.id) ON ItemInstance.vendorSiteAccountInfoId=VendorSiteAccountInfo.id) ON ItemInstance.itemId=Item.id) ON Item.itemTypeId=ItemType.id ORDER By ItemType.name, VendorDetail.name;
        // vendor information, currently all items are centrally purchased so
        // may assume same item type has same vendor regardless of where item ultimately goes
        [ManyToOne(nameof(vendorId))]  // in DB as foreign key for VendorDetail table
        [FieldLabel(PrettyName: "Vendor", ToolTip = "Vendor for item.")]
        public VendorDetail vendor
        {
            get { return _vendor; }
            set
            {
                SetProperty(ref _vendor, value, nameof(vendor));
                // update foreign key when value changes!
                vendorId = _vendor.id;
            }
        }
        private VendorDetail _vendor = null;
        [ForeignKey(nameof(vendor))]
        [FieldLabel(PrettyName: "Vendor ID", ToolTip = "Database primary key for vendor information.")]
        public Guid vendorId
        {
            get { return _vendorId; }
            set { SetProperty(ref _vendorId, value, nameof(vendorId)); }
        }
        private Guid _vendorId;

        // additional remarks, applies to all items of this type regardless of unit or site
        [MaxLength(255)]
        public string notes { get { return _notes; } set { SetProperty(ref _notes, value, nameof(notes)); } }
        private string _notes = null;


        // image data for picture(s) of item (png, gif, jpg, etc. - raw data)
        // images are added to separate table (not usually required so data loaded only when needed)
        // foreign key(s) into Image table, empty set if none specified
        [ManyToManyAttribute(intermediateType: typeof(ItemTypeImageMapping))]
        public ObservableCollection<Image> images
        {
            get { return _images; }
            set { SetProperty(ref _images, value, nameof(images)); }
        }
        private ObservableCollection<Image> _images = new ObservableCollection<Image>();

        // documentation such as user manuals for item (txt, pdf, etc. - raw data)
        // documents are added to separate table (not usually required so data loaded only when needed)
        // foreign key(s) into Document table, empty set if none specified
        [ManyToMany(intermediateType: typeof(ItemTypeDocumentMapping))]
        public ObservableCollection<Document> documents
        {
            get { return _documents; }
            set { SetProperty(ref _documents, value, nameof(documents)); }
        }
        private ObservableCollection<Document> _documents = new ObservableCollection<Document>();
    }

    public class ItemTypeImageMapping
    {
        public ItemTypeImageMapping() : base() { id = Guid.NewGuid(); }
        public ItemTypeImageMapping(Guid id) : base() { this.id = id; }

        [PrimaryKey]
        public Guid id { get; set; }

        [ForeignKey(foreignTableType: typeof(ItemType))]
        public Guid itemTypeId { get; set; }
        [ForeignKey(foreignTableType:typeof(Image))]
        public Guid imageId { get; set; }
    }

    public class ItemTypeDocumentMapping : ItemBase
    {
        public ItemTypeDocumentMapping() : base() { id = Guid.NewGuid(); }
        public ItemTypeDocumentMapping(Guid id) : base() { this.id = id; }

        [PrimaryKey]
        public Guid id { get; set; }

        [ForeignKey(foreignTableType:typeof(ItemType))]
        public Guid itemTypeId { get; set; }
        [ForeignKey(foreignTableType:typeof(Document))]
        public Guid documentId { get; set; }
    }
}
