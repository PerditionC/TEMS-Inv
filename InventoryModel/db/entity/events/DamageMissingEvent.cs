// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using SQLite;

using TEMS.InventoryModel.util.attribute;

namespace TEMS.InventoryModel.entity.db
{
    public enum DamageMissingEventType
    {
        Damage = 0,
        Missing = 1
    }

    /// <summary>
    /// Provides the many-to-many mapping between items and damage/missing events
    /// </summary>
    public class DamageMissingEvent : ItemBase
    {
        public DamageMissingEvent() : base()
        {
            _id = Guid.NewGuid();
        }

        public DamageMissingEvent(Guid id) : base()
        {
            _id = id;
        }

        // primary key
        [PrimaryKey]
        public Guid id { get { return _id; } set { SetProperty(ref _id, value, nameof(id)); } }

        private Guid _id;

        // item damaged or missing
        [ManyToOne(nameof(itemInstanceId))] // in DB as foreign key in ItemInstance table
        public ItemInstance itemInstance
        {
            get { return _itemInstance; }
            set
            {
                SetProperty(ref _itemInstance, value, nameof(itemInstance));
                itemInstanceId = _itemInstance?.id ?? Guid.Empty;
            }
        }

        private ItemInstance _itemInstance;

        [ForeignKey(nameof(itemInstance))]
        public Guid itemInstanceId { get { return _itemInstanceId; } set { SetProperty(ref _itemInstanceId, value, nameof(itemInstanceId)); } }

        private Guid _itemInstanceId = Guid.Empty;

        // who (userId) input into DB
        [ForeignKey]
        [NotNull, MaxLength(32)]
        public string inputBy { get { return _inputBy; } set { SetProperty(ref _inputBy, value, nameof(inputBy)); } }

        private string _inputBy;

        // who (name, possibly contact info - may be used for auto-complete values) reported event
        [NotNull, MaxLength(256)]
        public string reportedBy { get { return _reportedBy; } set { SetProperty(ref _reportedBy, value, nameof(reportedBy)); } }

        private string _reportedBy;

        // when issue was identified/reported
        [NotNull]
        public DateTime discoveryDate { get { return _discoveryDate; } set { SetProperty(ref _discoveryDate, value, nameof(discoveryDate)); } }

        private DateTime _discoveryDate;

        // what kind of event is this, damage or missing
        [NotNull]
        public DamageMissingEventType eventType { get { return _eventType; } set { SetProperty(ref _eventType, value, nameof(eventType)); } }

        private DamageMissingEventType _eventType;

        // specific details as what happened/etc
        [MaxLength(256)]
        public string notes { get { return _notes; } set { SetProperty(ref _notes, value, nameof(notes)); } }

        private string _notes = null;
    }
}