// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using TEMS.InventoryModel.entity.db;

namespace TEMS_Inventory.views
{
    public class DetailsServiceViewModel : EventDetailsViewModel
    {
        public DetailsServiceViewModel(ItemService Event) : base(Event) { }

        public ItemService ItemServiceEvent { get { return Event as ItemService; } set { Event = value; } }

        // Replace Item Command
    }
}
