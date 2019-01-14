// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using TEMS.InventoryModel.entity.db;

namespace TEMS_Inventory.views
{
    public class DetailsDeployRecoverViewModel : EventDetailsViewModel
    {
        public DetailsDeployRecoverViewModel(DeployEvent Event) : base(Event) { }

        public DeployEvent DeployEvent { get { return Event as DeployEvent; } set { Event = value; } }

        // Recover Item Command
    }
}
