// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.ObjectModel;

using SQLite;

using TEMS.InventoryModel.entity.db.user;
using TEMS.InventoryModel.util;
using TEMS.InventoryModel.util.attribute;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Information about a given equipment (unit) type
    /// Note: this is not derived from ReferenceData as it lacks a Guid id
    /// </summary>
    public class EquipmentUnitType : ItemBase
    {
        public EquipmentUnitType()
        {
        }

        public EquipmentUnitType(EquipmentUnitType copy) : base()
        {
            _name = copy.name;
            _description = copy.description;
            _unitCode = copy.unitCode;

            AcceptChanges();
        }

        // type of equipment (common/short unit name), e.g. MMRS, DMSU, SSU
        [PrimaryKey, MaxLength(6), NotNull]
        public string name { get { return _name; } set { SetProperty(ref _name, value, nameof(name)); RaisePropertyChanged(nameof(PrimaryKey)); } }

        private string _name = null;

        // longer description of equipment
        [MaxLength(256), NotNull]
        public string description { get { return _description; } set { SetProperty(ref _description, value, nameof(description)); } }

        private string _description = null;

        // 0 or 1 character code used in item number, e.g. "", "D", "S"
        [MaxLength(1), NotNull]
        public string unitCode { get { return _unitCode; } set { SetProperty(ref _unitCode, value, nameof(unitCode)); } }

        private string _unitCode = null;
    }

    /// <summary>
    /// Information about a given site location (jurisdiction)
    /// [A jurisdiction may be split to multiple site locations]
    /// </summary>
    public class SiteLocation : ReferenceData
    {
        public SiteLocation(Guid id) : base(id)
        {
        }

        public SiteLocation() : this(Guid.NewGuid())
        {
        }

        public SiteLocation(SiteLocation copyOfObj) : base(copyOfObj)
        {
            _locSuffix = copyOfObj.locSuffix;
            equipmentUnitTypesAvailable = copyOfObj.equipmentUnitTypesAvailable;
            AcceptChanges();
        }

        // id (Primary Key) and name (DisplayNameProperty) are derived from ReferenceData
        // where id is internal DB primary key
        // and name is the name of this site (jurisdiction), usually city name, e.g. Norfolk, Chesapeake, etc.


        // abbreviated site name appended to item number
        [FieldLabelAttribute(PrettyName: "Site Suffix", ToolTip = "Abbreviated site name appended to item number")]
        [MaxLength(6), NotNull]
        public string locSuffix { get { return _locSuffix; } set { SetProperty(ref _locSuffix, value, nameof(locSuffix)); } }

        private string _locSuffix = null;

        // what equipment and how many are available at this site
        [ManyToMany(typeof(SiteLocationEquipmentUnitTypeMapping))]
        public ObservableCollection<EquipmentUnitType> equipmentUnitTypesAvailable { get; set; } = new ObservableCollection<EquipmentUnitType>();
    }

    /// <summary>
    /// Provides the many-to-many mapping between site locations and equipment unit types
    /// </summary>
    public class SiteLocationEquipmentUnitTypeMapping : NotifyPropertyChanged
    {
        public SiteLocationEquipmentUnitTypeMapping() : base()
        {
            id = Guid.NewGuid();
        }

        public SiteLocationEquipmentUnitTypeMapping(Guid id) : base()
        {
            this.id = id;
        }

        // primary key
        [PrimaryKey]
        public Guid id { get; set; }

        // foreign key in SiteLocation table
        [NotNull, ForeignKey(foreignTableType: typeof(SiteLocation))]
        public Guid siteId { get; set; }

        // foreign key in EquipmentUnitType table
        [MaxLength(6), NotNull, ForeignKey(foreignTableType: typeof(EquipmentUnitType))]
        public string unitName { get; set; }
    }

    /// <summary>
    /// Provides the many-to-many mapping between a user and site locations they have access for
    /// </summary>
    public class UserSiteMapping : NotifyPropertyChanged
    {
        public UserSiteMapping() : base()
        {
            id = Guid.NewGuid();
        }

        public UserSiteMapping(Guid id) : base()
        {
            this.id = id;
        }

        // primary key
        [PrimaryKey]
        public Guid id { get; set; }

        // foreign key in SiteLocation table
        [NotNull, ForeignKey(foreignTableType: typeof(SiteLocation))]
        public Guid siteId { get; set; }

        // foreign key in UserDetail
        [MaxLength(32), NotNull, ForeignKey(foreignTableType: typeof(UserDetail))]
        public string userId { get; set; }
    }
}