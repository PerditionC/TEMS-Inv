// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.userManager;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// An item (including any contained items if item is a container)
    /// in inventory have been returned from being deployed
    /// </summary>
    public class RecoverItemCommand : DeployRecoverItemCommandBase
    {
        public RecoverItemCommand() : this(statusAvailable)
        {
        }

        public RecoverItemCommand(ItemStatus itemStatus) : base()
        {
            _canExecute = IsItemInstanceDeployed;
            _execute = RecoverItem;

            this.itemStatus = itemStatus;
        }

        /// <summary>
        /// determines if after recovery item is marked as available or contaminated
        /// </summary>
        private readonly ItemStatus itemStatus;

        /// <summary>
        /// if parameter represents an ItemInstance and has status of deployed then
        /// can be returned/recovered.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static bool IsItemInstanceDeployed(object parameter)
        {
            return IsAnyItemInstanceStatus(parameter, statusDeployed);
        }

        /// <summary>
        /// updates item(s) to available and closes most recent deploy event
        /// </summary>
        /// <param name="parameter"></param>
        private void RecoverItem(object parameter)
        {
            ApplyIfStatus(parameter, statusDeployed, itemStatus, deployEvent, GetLastDeployEvent);
        }

        /// <summary>
        /// Obtain most recent deployment event
        /// If no deployment events still pending, then returns null otherwise
        /// returns the still deployed event.
        /// </summary>
        /// <param name="itemInstanceId"></param>
        /// <returns></returns>
        private static DeployEvent GetLastDeployEvent(ItemInstance itemInstance, DeployEvent baseDeployEvent)
        {
            var deployEvent = DataRepository.GetDataRepository.GetLatestDeploymentEventFor(itemInstance.id);
            if ((deployEvent == null) || (deployEvent.recoverDate != null))
            {
                logger.Warn($"Item {itemInstance.itemNumber} is not deployed, unable to get event and/or already recovered.");
                return null;
            }
            deployEvent.recoverDate = baseDeployEvent.recoverDate;
            deployEvent.recoverBy = baseDeployEvent.recoverBy;
            return deployEvent;
        }
    }
}