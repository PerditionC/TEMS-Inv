// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using TEMS.InventoryModel.entity.db;

namespace TEMS_Inventory.views
{
    public class DetailsDamagedMissingViewModel : EventDetailsViewModel
    {
        public DetailsDamagedMissingViewModel(DamageMissingEvent Event) : base(Event) { }

        public DamageMissingEvent DamageMissingEvent { get { return Event as DamageMissingEvent; } set { Event = value; } }

        // Return To Inventory Command
        // Send for Repairs Command
        // Replace Item Command
    }
}
