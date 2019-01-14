// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// A single item in inventory not already out of service is replaced with
    /// a new item, the old item is updated to be out of service status.
    /// </summary>
    public class ReplaceItemCommand : RelayCommand
    {
        public ReplaceItemCommand() : base(ReplaceItem, IsValidParameters)
        {
        }

        private static bool IsValidParameters(object parameter)
        {
            var outOfServiceStatusId = new Guid("13e9295e-bcff-458e-ac0e-c13359a8003a");

            if (parameter is ItemInstance itemInstance)
            {
                return outOfServiceStatusId != itemInstance.statusId;
            }
            if (parameter is GenericItemResult itemResult)
            {
                return outOfServiceStatusId != itemResult.statusId;
            }

            return false;
        }

        private static void ReplaceItem(object parameter)
        {
            logger.Trace(nameof(ReplaceItem));
            try
            {
                var db = DataRepository.GetDataRepository;
                var outOfServiceStatus = db.ReferenceData[nameof(ItemStatus)].ByName<ItemStatus>("Out for Service");
                var availableStatus = db.ReferenceData[nameof(ItemStatus)].ByName<ItemStatus>("Available");

                var itemInstance = parameter as ItemInstance;
                if (itemInstance is null)
                {
                    if (parameter is GenericItemResult itemResult)
                    {
                        itemInstance = db.Load<ItemInstance>(itemResult.id);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException($"Unsupported parameter - {parameter.ToString()}");
                    }
                }

                // we need a copy of current item, without unique identifiers stripped & ensure available
                var newItemInstance = itemInstance.GetClonedItem() as ItemInstance;
                newItemInstance.id = Guid.NewGuid(); // ensure not a duplicate
                newItemInstance.status = availableStatus;
                newItemInstance.serialNumber = null;
                newItemInstance.removedServiceDate = null;
                newItemInstance.isSealBroken = false;
                newItemInstance.notes = null; // keep notes?
                // update old item is out of service (a constraint violation will occur if we
                // attempt to have 2 active ItemInstances with the same item number
                itemInstance.status = outOfServiceStatus;
                itemInstance.removedServiceDate = DateTime.Now.Date;
                // then save new item
                //db.BeginTransaction();
                db.Save(itemInstance);
                db.Save(newItemInstance);
                //db.EndTransaction();
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in {nameof(ReplaceItem)} - {e.Message}");
                throw;
            }
        }
    }
}