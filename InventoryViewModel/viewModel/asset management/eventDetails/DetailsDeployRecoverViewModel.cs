// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using TEMS.InventoryModel.command.action;
using TEMS.InventoryModel.entity.db;

namespace TEMS_Inventory.views
{
    public class DetailsDeployRecoverViewModel : EventDetailsViewModel
    {
        public DetailsDeployRecoverViewModel(DeployEvent Event) : base(Event) { }

        public DeployEvent DeployEvent { get { return Event as DeployEvent; } set { Event = value; } }

        // Recover Item Command
        private RecoverItemCommand recoverCommand = new RecoverItemCommand();
        private DeployItemCommand deployCommand = new DeployItemCommand();




        /// <summary>
        /// Are we working with items with a status of Available (true) or Deployed (false)?
        /// </summary>
        public bool StatusAvailable
        {
            get { return _StatusAvailable; }
            set
            {
                SetProperty(ref _StatusAvailable, value, nameof(StatusAvailable));
                RaisePropertyChanged(nameof(StatusDeployed));
                //updateStatusSearchFilter();
            }
        }
        public bool StatusDeployed
        {
            get { return !_StatusAvailable; }
            set { StatusAvailable = !value; }
        }
        private bool _StatusAvailable = true;

    }
}
