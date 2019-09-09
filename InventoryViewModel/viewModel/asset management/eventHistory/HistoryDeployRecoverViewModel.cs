// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.command.action;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.userManager;
using TEMS.InventoryModel.util.attribute;

namespace TEMS_Inventory.views
{
    public class HistoryDeployRecoverViewModel : EventHistoryViewModelBase
    {
        public HistoryDeployRecoverViewModel() : base()
        {
            base.PropertyChanged += HistoryDeployRecoverViewModel_PropertyChanged;
        }

        private void HistoryDeployRecoverViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (string.Equals(nameof(CurrentItem), e.PropertyName, StringComparison.InvariantCulture))
            {
                RaisePropertyChanged(nameof(DeployMode));
                RaisePropertyChanged(nameof(RecoverMode));
            }
        }

        /// <summary>
        /// Command to open window with deployment details for review/edit
        /// </summary>
        protected override void DoUpdateCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "DeployRecoverDetails", args = SelectedEvent as DeployEvent });
        }


        /// <summary>
        /// true if current item is available, false otherwise
        /// </summary>
        public bool DeployMode
        {
            get { return IsCurrentItemEditable && string.Equals("Available",(CurrentItem.entity as ItemInstance)?.status?.name, StringComparison.InvariantCultureIgnoreCase); }
        }

        /// <summary>
        /// true if current item is deployed (not available), false otherwise
        /// </summary>
        public bool RecoverMode
        {
            get { return IsCurrentItemEditable && string.Equals("Deployed", (CurrentItem.entity as ItemInstance)?.status?.name, StringComparison.InvariantCultureIgnoreCase); }
        }


        /// <summary>
        /// Command to create an event record of item as damaged
        /// </summary>
        public ICommand DeployRecoverCommand
        {
            get { return InitializeCommand(ref _DeployRecoverCommand, param => DoDeployRecoverCommand(), param => IsCurrentItemEditable); }
        }
        private ICommand _DeployRecoverCommand;

        private void DoDeployRecoverCommand()
        {
            DeployEvent deployEvent;
            if (DeployMode)
            {
                deployEvent = GetNewDeployEvent(CurrentItem.entity as ItemInstance);
            }
            else
            {
                deployEvent = GetLastDeployEvent(CurrentItem.entity as ItemInstance);
            }
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "DeployRecoverDetails", args = deployEvent as DeployEvent });
            SelectedEvent = deployEvent;
        }

        /// <summary>
        /// creates a new DeployEvent to store information about this deployment
        /// </summary>
        /// <param name="itemInstance"></param>
        /// <returns></returns>
        private DeployEvent GetNewDeployEvent(ItemInstance itemInstance)
        {
            var currentUser = UserManager.GetUserManager.CurrentUser();
            var deployEvent = new DeployEvent
            {
                // set default/common values, specific event values set by caller
                itemInstance = itemInstance,
                deployDate = DateTime.Now,
                deployBy = currentUser?.userId,
                notes = string.Empty
            };
            return deployEvent;
        }

        /// <summary>
        /// Obtain most recent deployment event
        /// If no deployment events still pending, then returns null otherwise
        /// returns the still deployed event.
        /// </summary>
        /// <param name="itemInstanceId"></param>
        /// <returns></returns>
        private static DeployEvent GetLastDeployEvent(ItemInstance itemInstance)
        {
            var deployEvent = DataRepository.GetDataRepository.GetLatestDeploymentEventFor(itemInstance.id);
            if ((deployEvent == null) || (deployEvent.recoverDate != null))
            {
                logger.Warn($"Item {itemInstance.itemNumber} is not deployed, unable to get event and/or already recovered.");
                return null;
            }
            deployEvent.recoverDate = DateTime.Now;
            deployEvent.recoverBy = UserManager.GetUserManager.CurrentUser().userId;
            return deployEvent;
        }


        // primary external id; as used in barcode, e.g. D236-19807-NFR
        [DisplayNameProperty]
        public string itemNumber
        {
            get
            {
                if (CurrentItem is GenericItemResult)
                {
                    var val = (CurrentItem as GenericItemResult)?.itemNumber;
                    return $"{val ?? "?"}";
                }
                else
                {
                    return "No selection";
                }
            }
        }
    }
}
