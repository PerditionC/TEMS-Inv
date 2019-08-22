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
                updateStatusSearchFilter();
            }
        }
        public bool StatusDeployed
        {
            get { return !_StatusAvailable; }
            set { StatusAvailable = !value; }
        }
        private bool _StatusAvailable = true;

        private void updateStatusSearchFilter()
        {
            // save if currently disabled or not so we don't enable too early (if currently not enabled)
            var wasEnabled = SearchFilter.SearchFilterEnabled;
            // force as disabled while we update
            SearchFilter.SearchFilterEnabled = false;
            SearchFilter.SelectedItemStatusValues.Clear();
            if (_StatusAvailable)
            {
                // will auto change, but we want to happen immediately
                deployRecoverCommands.DeployRecoverItemInstanceCommandText = DeployRecoverItemInstanceAction.MenuDeploy;
                SearchFilter.SelectedItemStatusValues.Add(deployRecoverCommands.statusAvailable);
            }
            else
            {
                // will auto change, but we want to happen immediately
                deployRecoverCommands.DeployRecoverItemInstanceCommandText = DeployRecoverItemInstanceAction.MenuRecover;
                SearchFilter.SelectedItemStatusValues.Add(deployRecoverCommands.statusDeployed);
            }
            // trigger update is currently enabled
            SearchFilter.SearchFilterEnabled = wasEnabled;
        }
    }
}
