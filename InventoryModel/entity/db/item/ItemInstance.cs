// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using SQLite;

using TEMS.InventoryModel.util.attribute;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Details about this specific item at this specific site location
    /// i.e. pairs up an Item to a SiteLocation to provide a unique item instance
    /// </summary>
    public class ItemInstance : ItemBase
    {
        public ItemInstance(Guid id) : base(typeof(ItemInstance).GetProperty(nameof(ItemInstance.id)))
        {
            _id = id;
        }

        public ItemInstance() : this(Guid.NewGuid())
        {
        }

        public ItemInstance(ItemInstance copyOfObj) : this(copyOfObj.id)
        {
            item = copyOfObj.item; // we need to set object and fk_id field
            siteLocation = copyOfObj.siteLocation;
            serialNumber = copyOfObj.serialNumber;
            grantNumber = copyOfObj.grantNumber;
            status = copyOfObj.status;
            inServiceDate = copyOfObj.inServiceDate;
            removedServiceDate = copyOfObj.removedServiceDate;
            isSealBroken = copyOfObj.isSealBroken;
            hasBarcode = copyOfObj.hasBarcode;
            notes = copyOfObj.notes;

            AcceptChanges();
        }

        // internal DB primary key, unique per item
        // Note: GUID used for replication purposes
        [PrimaryKey]
        [HideProperty]
        public Guid id { get { return _id; } set { SetProperty(ref _id, value, nameof(id)); RaisePropertyChanged(nameof(PrimaryKey)); } }

        private Guid _id;

        // primary external id; as used in barcode, e.g. D236-19807-NFR
        [SQLite.Ignore]
        [HideProperty]
        [DisplayNameProperty]
        public string itemNumber
        {
            get { return $"{item?.itemNumber}-{siteLocation?.locSuffix}"; }
        }

        // item specific details for a the specific equipment unit this item instance is part of
        [ManyToOne(nameof(itemId))] // in DB as foreign key for Item table
        public Item item
        {
            get { return _item; }
            set
            {
                SetProperty(ref _item, value, nameof(item));
                itemId = _item?.id ?? Guid.Empty;
            }
        }

        private Item _item;

        [NotNull, ForeignKey(nameof(item))]
        public Guid itemId { get { return _itemId; } set { SetProperty(ref _itemId, value, nameof(itemId)); } }

        private Guid _itemId = Guid.Empty;

        // jurisdiction this specific item is located at
        [ManyToOne(nameof(siteLocationId))] // in DB as foreign key for SiteLocation table
        public SiteLocation siteLocation
        {
            get { return _siteLocation; }
            set
            {
                SetProperty(ref _siteLocation, value, nameof(siteLocation));
                siteLocationId = _siteLocation?.id ?? Guid.Empty;
            }
        }

        private SiteLocation _siteLocation;

        [ForeignKey(nameof(siteLocation))]
        public Guid siteLocationId { get { return _siteLocationId; } set { SetProperty(ref _siteLocationId, value, nameof(siteLocationId)); } }

        private Guid _siteLocationId = Guid.Empty;

        #region item specific details

        // unique serial # if applicable
        [MaxLength(32)]
        public string serialNumber
        {
            get { return _serialNumber; }
            set { SetProperty(ref _serialNumber, value, nameof(serialNumber)); }
        }

        private string _serialNumber = null;

        // grant #, external reference # for grant purposes
        [MaxLength(16)]
        public string grantNumber { get { return _grantNumber; } set { SetProperty(ref _grantNumber, value, nameof(grantNumber)); } }

        private string _grantNumber = null;

        // item status, Available, Damaged, etc.
        [ManyToOne(nameof(statusId))] // in DB as foreign key for ItemStatus table
        public ItemStatus status
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, value, nameof(status));
                statusId = _status?.id ?? Guid.Empty;
            }
        }

        private ItemStatus _status;

        [NotNull, ForeignKey(nameof(status))]
        public Guid statusId { get { return _statusId; } set { SetProperty(ref _statusId, value, nameof(statusId)); } }

        private Guid _statusId = Guid.Empty;

        // initial service date
        [NotNull]
        public DateTime inServiceDate { get { return _inServiceDate; } set { SetProperty(ref _inServiceDate, value, nameof(inServiceDate)); } }

        private DateTime _inServiceDate = DateTime.MinValue;

        // when removed from service, null if still in service
        public DateTime? removedServiceDate { get { return _removedServiceDate; } set { SetProperty(ref _removedServiceDate, value, nameof(removedServiceDate)); } }

        private DateTime? _removedServiceDate = null;

        // has seal been broken
        public bool isSealBroken { get { return _isSealBroken; } set { SetProperty(ref _isSealBroken, value, nameof(isSealBroken)); } }

        private bool _isSealBroken = false;

        // has barcode been affixed to item
        // barcode is itemNumber
        public bool hasBarcode { get { return _hasBarcode; } set { SetProperty(ref _hasBarcode, value, nameof(hasBarcode)); } }

        private bool _hasBarcode = false;

        // additional remarks about item
        [MaxLength(255)]
        public string notes { get { return _notes; } set { SetProperty(ref _notes, value, nameof(notes)); } }

        private string _notes = null;

        #endregion item specific details
    }
}