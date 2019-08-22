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
    public class ServiceItemCompleteCommand : UpdateItemStatusCommand
    {
        public ServiceItemCompleteCommand() : base(ServiceItemIsComplete, IsValidParameters)
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
                (serviceEvent.service == null) ||
                (serviceEvent.service.itemInstance == null) ||
                (serviceEvent.service.category == null))
            {
                return false;
            }

            // Note: we don't check serviceComplete date as may already be set to actual completion in the past
            return true;
        }

        /// <summary>
        /// Add new service event record to service history and update item's status to out for service.
        /// </summary>
        /// <param name="parameter">A ItemServiceHistory with information about the service done to item</param>
        private static void ServiceItemIsComplete(object parameter)
        {
            logger.Trace(nameof(ServiceItemIsComplete));

            try
            {
                if (!(parameter is ItemServiceHistory serviceEvent)) throw new ArgumentException("Attempting to complete ServiceItem on unsupported parameter!");

                // update service event to complete
                if (serviceEvent.serviceCompleted == null) serviceEvent.serviceCompleted = DateTime.Now.Date;
                var itemInstance = serviceEvent.service.itemInstance;
                ItemStatus status = GetItemStatus("Available");
                UpdateStatus(itemInstance, status, serviceEvent);

                // if reoccurring then add new pending one
                ItemServiceHistory nextService = GetNextServiceEvent(serviceEvent);
                if (nextService != null)
                {
                    DataRepository.GetDataRepository.Save(nextService);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in {nameof(ServiceItemIsComplete)} - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// If serviceEvent corresponds with a reoccurring ItemService then returns next
        /// ItemServiceHistory for service based on current service's completion date.
        /// Return null if not reoccurring service or current service is not yet completed.
        /// </summary>
        /// <param name="serviceEvent"></param>
        /// <returns></returns>
        public static ItemServiceHistory GetNextServiceEvent(ItemServiceHistory serviceEvent)
        {
            if ((!serviceEvent.service.reoccurring) || (serviceEvent.serviceCompleted == null)) return null;

            DateTime nextServiceDue;
            switch (serviceEvent.service.serviceFrequency)
            {
                case ServiceFrequency.Days:
                    nextServiceDue = ((DateTime)serviceEvent.serviceCompleted).AddDays(serviceEvent.service.lengthTilNextService);
                    break;

                case ServiceFrequency.Weeks:
                    nextServiceDue = ((DateTime)serviceEvent.serviceCompleted).AddDays(7 * serviceEvent.service.lengthTilNextService);
                    break;

                case ServiceFrequency.Months:
                    nextServiceDue = ((DateTime)serviceEvent.serviceCompleted).AddMonths(serviceEvent.service.lengthTilNextService);
                    break;

                case ServiceFrequency.Years:
                    nextServiceDue = ((DateTime)serviceEvent.serviceCompleted).AddYears(serviceEvent.service.lengthTilNextService);
                    break;

                default:
                    throw new ArgumentOutOfRangeException($"{nameof(ServiceFrequency)} invalid value! {serviceEvent.service.serviceFrequency}");
            }

            var nextService = new ItemServiceHistory
            {
                service = serviceEvent.service,
                serviceDue = nextServiceDue
            };
            return nextService;
        }
    }
}