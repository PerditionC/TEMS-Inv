// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using TEMS.InventoryModel.entity.db;

namespace TEMS_Inventory.views
{
    public class DetailsExpirationReplaceViewModel : EventDetailsViewModel
    {
        public DetailsExpirationReplaceViewModel(ItemBase Event) : base(Event) { }

        public ItemBase ExpirationEvent { get { return Event; } set { Event = value; } }

        // Replenish Expired Item Command  -- updates expiration date, if annual adds year, if date then prompts for new expiration date
    }
}
