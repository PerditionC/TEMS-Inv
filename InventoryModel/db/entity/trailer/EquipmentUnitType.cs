// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using SQLite;

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
}