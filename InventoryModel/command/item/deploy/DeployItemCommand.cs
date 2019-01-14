// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.userManager;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// An item(including any contained items if item is a container)
    /// in inventory have been deployed (taken to be used)
    /// </summary>
    public sealed class DeployItemCommand : DeployRecoverItemCommandBase
    {
        public DeployItemCommand() : base()
        {
            _execute = DeployItem;
            _canExecute = IsItemInstanceAvailable;
        }

        /// <summary>
        /// if parameter represents an ItemInstance and has status of available then
        /// can be deployed.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static bool IsItemInstanceAvailable(object parameter)
        {
            return IsAnyItemInstanceStatus(parameter, statusAvailable);
        }

        /// <summary>
        /// updates item(s) to deployed and creates new deploy event
        /// </summary>
        /// <param name="parameters"></param>
        private void DeployItem(object parameter)
        {
            ApplyIfStatus(parameter, statusAvailable, statusDeployed, GetNewEvent);
        }

        /// <summary>
        /// creates a new DeployEvent to store information about this deployment
        /// </summary>
        /// <param name="itemInstance"></param>
        /// <returns></returns>
        private DeployEvent GetNewEvent(ItemInstance itemInstance)
        {
            var deployEvent = new DeployEvent
            {
                // set default/common values, specific event values set by caller
                itemInstance = itemInstance,
                deployBy = UserManager.GetUserManager.CurrentUser().userId,
                deployDate = DateTime.Now,
                notes = this.Notes
            };
            return deployEvent;
        }
    }
}