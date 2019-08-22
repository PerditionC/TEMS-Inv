// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using TEMS.InventoryModel.entity.db;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// Either a single item in inventory received regular maintenance
    /// or was previously damaged has been repaired
    /// and returned ready for future deployment (use)
    /// (does not apply to items within item if a container)
    /// </summary>
    public class ServiceItemCommand : UpdateItemStatusCommand
    {
        public ServiceItemCommand() : base(ServiceItem, IsValidParameters)
        {
        }

        /// <summary>
        /// Can update status of item to be out for service
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static bool IsValidParameters(object parameter)
        {
            if ((!(parameter is ItemServiceHistory serviceEvent)) ||
                // service can't be due in the past (?), could be past due or added after the fact
                //(DateTime.Compare(serviceEvent.serviceDue, DateTime.Now) < 0) ||
                (serviceEvent.service == null) ||
                (serviceEvent.service.itemInstance == null) ||
                (serviceEvent.service.category == null))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add new service event record to service history and update item's status to out for service.
        /// </summary>
        /// <param name="parameter">A ItemServiceHistory with information about the service done to item</param>
        private static void ServiceItem(object parameter)
        {
            logger.Trace(nameof(ServiceItem));

            try
            {
                if (!(parameter is ItemServiceHistory serviceEvent)) throw new ArgumentException("Attempting to ServiceItem on unsupported parameter!");
                var itemInstance = serviceEvent.service.itemInstance;
                var status = GetItemStatus("Out for Service");
                UpdateStatus(itemInstance, status, CheckEvent(itemInstance, status, serviceEvent));
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in {nameof(ServiceItem)} - {e.Message}");
                throw;
            }
        }
    }
}