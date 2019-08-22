// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// A single item in inventory previously not Available
    /// has been returned ready for future deployment (use)
    /// (does not apply to items within item if a container)
    /// </summary>
    public class ReturnToInventoryCommand : UpdateItemStatusCommand
    {
        public ReturnToInventoryCommand() : base(ReturnItem, IsItemInstanceNotAvailable)
        {
        }

        /// <summary>
        /// if parameter represents an ItemInstance and has status other than Available
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static bool IsItemInstanceNotAvailable(object parameter)
        {
            return !IsAllItemInstanceStatus(parameter, GetItemStatus("Available"));
        }

        /// <summary>
        /// updates item(s) to damaged and creates new damaged/missing event
        /// </summary>
        /// <param name="parameters"></param>
        private static void ReturnItem(object parameter)
        {
            logger.Trace(nameof(ReturnItem));
            ItemStatus status = GetItemStatus("Available");

            try
            {
                if (parameter is ItemInstance itemInstance)
                {
                    UpdateStatus(itemInstance, status, null);
                    return;
                }

                var db = DataRepository.GetDataRepository;

                if (parameter is GenericItemResult itemResult)
                {
                    var item = db.Load<ItemInstance>(itemResult.id);
                    UpdateStatus(item, status, null);
                    return;
                }

                // user selected a list of damaged items
                if (parameter is IList<GenericItemResult> items)
                {
                    foreach (var i in items)
                    {
                        var item = db.Load<ItemInstance>(i.id);
                        UpdateStatus(item, status, null);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in ReturnItem - {e.Message}");
                throw;
            }
        }
    }
}