// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Reflection;
using NLog;

using SQLite;

using TEMS.InventoryModel.util.attribute;

// Note: if these are moved to another name space, ReferenceDataCache must be updated with new name
namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Base class for reference data, all have the same format for storage,
    /// but each represents a different set of data, hence would be stored in different tables
    /// </summary>
    public class ReferenceData : ItemBase
    {
        public ReferenceData(Guid id, PropertyInfo pkProp) : base(pkProp)
        {
            logger = LogManager.GetCurrentClassLogger();
            logger.Trace(nameof(ReferenceData));

            _id = id;
        }
        public ReferenceData(Guid id) : this(id, null)
        {
        }
        public ReferenceData() : this(Guid.NewGuid())
        {
        }

        public ReferenceData(ReferenceData copyOfObj) : this(copyOfObj.id)
        {
            _name = copyOfObj.name; AcceptChanges();
        }

        [PrimaryKey]
        public Guid id { get { return _id; } set { SetProperty(ref _id, value, nameof(id)); RaisePropertyChanged(nameof(PrimaryKey)); } }

        private Guid _id;

        [MaxLength(128), NotNull, DisplayNameProperty]
        public string name { get { return _name; } set { SetProperty(ref _name, value, nameof(name)); } }

        private string _name = null;
    }

    /// <summary>
    /// Mapping of unit of measure (each, box, lb, ...) codes to descriptive name
    /// </summary>
    public class UnitOfMeasure : ReferenceData
    {
        public UnitOfMeasure() : base()
        {
        }

        public UnitOfMeasure(Guid id) : base(id)
        {
        }

        public UnitOfMeasure(UnitOfMeasure copyOfObj) : base(copyOfObj)
        {
        }
    }

    /// <summary>
    /// Mapping of battery type (AA, , ...) codes to descriptive name
    /// </summary>
    public class BatteryType : ReferenceData
    {
        public BatteryType() : base()
        {
        }

        public BatteryType(Guid id) : base(id)
        {
        }

        public BatteryType(BatteryType copyOfObj) : base(copyOfObj)
        {
        }
    }

    /// <summary>
    /// Mapping of vehicle location (trailer, F650 truck, ...) codes to descriptive name
    /// </summary>
    public class VehicleLocation : ReferenceData
    {
        public VehicleLocation() : base()
        {
        }

        public VehicleLocation(Guid id) : base(id)
        {
        }

        public VehicleLocation(VehicleLocation copyOfObj) : base(copyOfObj)
        {
        }
    }

    /// <summary>
    /// Mapping of status (available, damaged, ...) codes to descriptive name
    /// </summary>
    public class ItemStatus : ReferenceData
    {
        public ItemStatus() : base()
        {
        }

        public ItemStatus(Guid id) : base(id)
        {
        }

        public ItemStatus(ItemStatus copyOfObj) : base(copyOfObj)
        {
        }
    }

    /// <summary>
    /// Mapping of item category (ppe, shelter, ...) codes to descriptive name
    /// </summary>
    public class ItemCategory : ReferenceData
    {
        public ItemCategory() : base()
        {
        }

        public ItemCategory(Guid id) : base(id)
        {
        }

        public ItemCategory(ItemCategory copyOfObj) : base(copyOfObj)
        {
        }
    }

    /// <summary>
    /// Mapping of service category (, ...) codes to descriptive name
    /// </summary>
    public class ServiceCategory : ReferenceData
    {
        public ServiceCategory() : base()
        {
        }

        public ServiceCategory(Guid id) : base(id)
        {
        }

        public ServiceCategory(ServiceCategory copyOfObj) : base(copyOfObj)
        {
        }
    }
}