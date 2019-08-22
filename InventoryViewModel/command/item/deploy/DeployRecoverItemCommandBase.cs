// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// An item(including any contained items if item is a container)
    /// in inventory have been deployed (taken to be used)
    /// </summary>
    public class DeployRecoverItemCommandBase : UpdateItemStatusCommand
    {
        protected DeployRecoverItemCommandBase() : base()
        {
        }

        protected DeployRecoverItemCommandBase(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute)
        {
        }

        // cache [on first use] a copy of reference status that we are updating/querying on
        private static ItemStatus _statusAvailable = null;

        public static ItemStatus statusAvailable
        {
            get
            {
                if (_statusAvailable == null)
                {
                    _statusAvailable = GetItemStatus("Available");
                }

                return _statusAvailable;
            }
        }

        // cache [on first use] a copy of reference status that we are updating/querying on
        private static ItemStatus _statusDeployed = null;

        public static ItemStatus statusDeployed
        {
            get
            {
                if (_statusDeployed == null)
                {
                    _statusDeployed = GetItemStatus("Deployed");
                }

                return _statusDeployed;
            }
        }

        // cache [on first use] a copy of reference status that we are updating/querying on
        private static ItemStatus _statusContaminated = null;

        public static ItemStatus statusContaminated
        {
            get
            {
                if (_statusContaminated == null)
                {
                    _statusContaminated = GetItemStatus("Contaminated");
                }

                return _statusContaminated;
            }
        }

        /// <summary>
        /// applies status change to all applicable parameter
        /// if parameter is a single item then updates it,
        /// if parameter is a list, then tests each current status and
        /// updates if applicable
        /// </summary>
        /// <param name="parameters"></param>
        protected static void ApplyIfStatus(object parameter, ItemStatus requiredStatus, ItemStatus newStatus, Func<ItemInstance, DeployEvent> GetNewEvent)
        {
            logger.Trace(nameof(ApplyIfStatus));
            if ((newStatus == null) || (newStatus.id == Guid.Empty))
            {
                logger.Warn("Can't update to invalid status!");
                return;
            }

            try
            {
                if (parameter is ItemInstance itemInstance)
                {
                    if (requiredStatus.id.Equals(itemInstance.statusId)) UpdateStatus(itemInstance, newStatus, CheckEvent(itemInstance, newStatus, GetNewEvent(itemInstance)));
                    return;
                }

                var db = DataRepository.GetDataRepository;

                if (parameter is GenericItemResult itemResult)
                {
                    if (requiredStatus.id.Equals(itemResult.statusId))
                    {
                        var item = db.Load<ItemInstance>(itemResult.id);
                        UpdateStatus(item, newStatus, CheckEvent(item, newStatus, GetNewEvent(item)));
                    }
                    return;
                }

                // we return true if any item in list is in available status, i.e. can be deployed
                // items not available with not be deployed!
                if (parameter is IList<GenericItemResult> items)
                {
                    foreach (var i in items)
                    {
                        if (requiredStatus.id.Equals(i.statusId))
                        {
                            var item = db.Load<ItemInstance>(i.id);
                            UpdateStatus(item, newStatus, CheckEvent(item, newStatus, GetNewEvent(item)));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in ApplyIfStatus - {e.Message}");
                throw;
            }
        }
    }
}