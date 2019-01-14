// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using SQLite;

using TEMS.InventoryModel.util.attribute;

namespace TEMS.InventoryModel.entity.db
{
    public enum ServiceFrequency
    {
        Days = 0,
        Weeks = 1,
        Months = 2,
        Years = 3
    }

    /// <summary>
    /// Provides the many-to-many mapping between items and required services
    /// </summary>
    public class ItemService : ItemBase
    {
        public ItemService() : base()
        {
            _id = Guid.NewGuid();
        }

        public ItemService(Guid id) : base()
        {
            _id = id;
        }

        // primary key
        [PrimaryKey]
        public Guid id { get { return _id; } set { SetProperty(ref _id, value, nameof(id)); } }

        private Guid _id;

        // displayed name for selection of reoccurring services
        [MaxLength(64), NotNull, DisplayNameProperty]
        public string name { get { return _name; } set { SetProperty(ref _name, value, nameof(name)); } }

        private string _name = null;

        // item requiring the service [maintenance]
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

        // what service is required
        [ManyToOne(nameof(categoryId))] // in DB as foreign key in ServiceCategory table
        public ServiceCategory category
        {
            get { return _category; }
            set
            {
                SetProperty(ref _category, value, nameof(category));
                categoryId = _category?.id ?? Guid.Empty;
            }
        }

        private ServiceCategory _category;

        [ForeignKey(nameof(category))]
        public Guid categoryId { get { return _categoryId; } set { SetProperty(ref _categoryId, value, nameof(categoryId)); } }

        private Guid _categoryId = Guid.Empty;

        // indicates if this is routine maintenance that is regularly scheduled (reoccurring service)
        public bool reoccurring { get; set; }

        // if reoccurring, how often must be done, e.g. annual, 3 months, weekly, ... Note: enum includes base of frequency
        public int lengthTilNextService { get; set; }

        // what unit of time lengthTilNextService indicates
        public ServiceFrequency serviceFrequency { get; set; }

        // general information about service, like get done at XYZ or do every 30,000 miles
        [MaxLength(256)]
        public string notes { get { return _notes; } set { SetProperty(ref _notes, value, nameof(notes)); } }

        private string _notes = null;

        /// <summary>
        /// extension method to calculate date of next service based on current service (if reoccurring)
        /// </summary>
        /// <param name="service">ItemService to base calculation on</param>
        /// <param name="start">Specific service event's date</param>
        /// <returns>If a reoccurring service then the date of next service based on ItemService's length until next service,
        ///          Otherwise if not reoccurring will return null - as no future date exists
        /// </returns>
        public static DateTime? NextService(ItemService service, DateTime start)
        {
            // return no date if not reoccurring, i.e. never
            if (!service.reoccurring) return null;

            // return new date based on frequency and how many periods to add
            switch (service.serviceFrequency)
            {
                case ServiceFrequency.Years:
                    return start.AddYears(service.lengthTilNextService);

                case ServiceFrequency.Months:
                    return start.AddMonths(service.lengthTilNextService);

                case ServiceFrequency.Weeks:
                    return start.AddDays(7 * service.lengthTilNextService);

                case ServiceFrequency.Days:
                    return start.AddDays(service.lengthTilNextService);

                default:
                    throw new ArgumentException("ItemService has invalid ServiceFrequency");
            }
        }
    }

    /// <summary>
    /// Provides the many-to-many mapping between items and services performed/pending
    /// </summary>
    public class ItemServiceHistory : ItemBase
    {
        public ItemServiceHistory() : base()
        {
            _id = Guid.NewGuid();
        }

        public ItemServiceHistory(Guid id) : base()
        {
            _id = id;
        }

        // primary key
        [PrimaryKey]
        public Guid id { get { return _id; } set { SetProperty(ref _id, value, nameof(id)); } }

        private Guid _id;

        // general service [maintenance] information, i.e. type, which item, if routine (repeated), ...
        [ManyToOne(nameof(serviceId))] // in DB as foreign key in ItemService table
        public ItemService service
        {
            get { return _service; }
            set
            {
                SetProperty(ref _service, value, nameof(service));
                serviceId = _service?.id ?? Guid.Empty;
            }
        }

        private ItemService _service;

        [ForeignKey(nameof(service))]
        public Guid serviceId { get { return _serviceId; } set { SetProperty(ref _serviceId, value, nameof(serviceId)); } }

        private Guid _serviceId = Guid.Empty;

        // when service is supposed to be done (when due)
        [NotNull]
        public DateTime serviceDue { get { return _serviceDue; } set { SetProperty(ref _serviceDue, value, nameof(serviceDue)); } }

        private DateTime _serviceDue;

        // when service was completed, null if still pending
        public DateTime? serviceCompleted { get { return _serviceCompleted; } set { SetProperty(ref _serviceCompleted, value, nameof(serviceCompleted)); } }

        private DateTime? _serviceCompleted = null;

        // specific information about this service, such as order#, waiting on parts, ...
        [SQLite.MaxLength(256)]
        public string notes { get { return _notes; } set { SetProperty(ref _notes, value, nameof(notes)); } }

        private string _notes = null;
    }
}