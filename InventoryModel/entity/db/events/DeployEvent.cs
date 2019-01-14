// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using SQLite;

using TEMS.InventoryModel.util.attribute;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Provides the many-to-many mapping between items and their deploy/recover events
    /// </summary>
    public class DeployEvent : ItemBase
    {
        public DeployEvent() : base()
        {
            _id = Guid.NewGuid();
        }

        public DeployEvent(Guid id) : base()
        {
            _id = id;
        }

        // primary key
        [PrimaryKey]
        public Guid id { get { return _id; } set { SetProperty(ref _id, value, nameof(id)); } }

        private Guid _id;

        // item deployed
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

        // who (userId) input into DB, i.e. who marked item as deployed
        [ForeignKey]
        [NotNull, MaxLength(32)]
        public string deployBy { get { return _deployBy; } set { SetProperty(ref _deployBy, value, nameof(deployBy)); } }

        private string _deployBy;

        // when (date + time) item was deployed
        [NotNull]
        public DateTime deployDate { get { return _deployDate; } set { SetProperty(ref _deployDate, value, nameof(deployDate)); } }

        private DateTime _deployDate;

        // who (userId) input recovery into DB, i.e. who returned item - marked item as 'Available',...
        // Note: null if not yet recovered
        [ForeignKey]
        [MaxLength(32)]
        public string recoverBy { get { return _recoverBy; } set { SetProperty(ref _recoverBy, value, nameof(recoverBy)); } }

        private string _recoverBy;

        // when (date + time) item was returned, null if not yet recovered
        public DateTime? recoverDate { get { return _recoverDate; } set { SetProperty(ref _recoverDate, value, nameof(recoverDate)); } }

        private DateTime? _recoverDate = null;

        // specific information about this deployment
        [MaxLength(256)]
        public string notes { get { return _notes; } set { SetProperty(ref _notes, value, nameof(notes)); } }

        private string _notes = null;
    }
}