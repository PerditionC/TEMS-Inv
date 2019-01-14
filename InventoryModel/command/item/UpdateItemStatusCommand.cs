// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// An item(including any contained items if item is a container)
    /// in inventory have been deployed (taken to be used)
    /// </summary>
    public class UpdateItemStatusCommand : RelayCommand
    {
        protected UpdateItemStatusCommand() : base()
        {
        }

        protected UpdateItemStatusCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute)
        {
        }

        /// <summary>
        /// Any ItemInstance with this ICommand applied will annotate the
        /// corresponding Event with these Notes.
        /// </summary>
        public string Notes { get; set; } = null;

        /// <summary>
        /// returns ItemStatus object for status (from reference data cache)
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        protected static ItemStatus GetItemStatus(string status)
        {
            return DataRepository.GetDataRepository?.ReferenceData[nameof(ItemStatus)].ByName<ItemStatus>(status);
        }

        /// <summary>
        /// If parameter is a single item then returns true if it's status matches status.
        /// If parameter is a list, the returns true if *any* item in list has matching status.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        protected static bool IsAnyItemInstanceStatus(object parameter, ItemStatus status)
        {
            logger.Trace(nameof(IsAnyItemInstanceStatus));
            if ((status == null) || (status.id == Guid.Empty))
            {
                logger.Warn("Checking if item matches invalid status!");
                return false;
            }

            try
            {
                // if an actual ItemInstance then just check it
                if (parameter is ItemInstance itemInstance)
                    return status.id.Equals(itemInstance.statusId);

                // if a search result for an ItemInstance check it, children handled in next block
                if (parameter is GenericItemResult item)
                {
                    if (status.id.Equals(item.statusId))
                        return true;
                }

                // if a search result header or ItemInstance then check its children, item itself checked in prior block
                if (parameter is SearchResult itemResult)
                { 
                    // handle children recursively
                    if (itemResult.childCount > 0)
                    {
                        if (IsAnyItemInstanceStatus(itemResult.children, status))
                            return true;
                    }
                }

                // we return true if any item in list is in available status, i.e. can be deployed
                // items not available will not be deployed!
                if (parameter is IList<GenericItemResult> items)
                {
                    foreach (var searchResult in items)
                    {
                        if (IsAnyItemInstanceStatus(searchResult, status))
                            return true;
                    }
                }

                if (parameter is ObservableCollection<SearchResult> childItems)
                {
                    foreach (var searchResult in childItems)
                    {
                        if (IsAnyItemInstanceStatus(searchResult, status))
                            return true;
                    }
                }

                // either a bad parameter or status is other than requested
                return false;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in IsAnyItemInstanceStatus - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// If parameter is a single item then returns true if it's status matches status.
        /// If parameter is a list, the returns true if *all* item in list has matching status.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        protected static bool IsAllItemInstanceStatus(object parameter, ItemStatus status)
        {
            logger.Trace(nameof(IsAnyItemInstanceStatus));
            if ((status == null) || (status.id == Guid.Empty))
            {
                logger.Warn("Checking if item matches invalid status!");
                return false;
            }

            try
            {
                if (parameter is ItemInstance itemInstance)
                    return status.id.Equals(itemInstance.statusId);

                if (parameter is GenericItemResult itemResult)
                    return status.id.Equals(itemResult.statusId);

                // we return true only if all item in list match requested status
                if (parameter is IList<GenericItemResult> items)
                {
                    foreach (var item in items)
                    {
                        if (!status.id.Equals(item.statusId))
                            return false;
                    }
                    return true;
                }

                // either a bad parameter or status is other than requested
                return false;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in IsAnyItemInstanceStatus - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates ItemInstance status, saving ItemInstance and DeployEvent together within a transaction
        /// </summary>
        /// <param name="item"></param>
        /// <param name="status"></param>
        /// <param name="itemEvent"></param>
        protected static void UpdateStatus(ItemInstance item, ItemStatus status, ItemBase itemEvent)
        {
            item.status = status;
            try
            {
                var db = DataRepository.GetDataRepository;
                db.BeginTransaction();
                db.Save(item);
                if (itemEvent != null)
                {
                    db.Save(itemEvent);
                }
                db.EndTransaction();
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to update item {item.itemNumber} status to {status.name}!");
                throw;
            }
        }

        /// <summary>
        /// It there is an Event associate with a status change, then prior to calling UpdateStatus, call
        /// this method on the itemEvent to be associated with the status change.
        /// </summary>
        /// <param name="itemEvent"></param>
        protected static ItemBase CheckEvent(ItemInstance item, ItemStatus status, ItemBase itemEvent)
        {
            if (itemEvent == null)
            {
                logger.Error($"No associated Event, item {item.itemNumber} updated to status to {status.name} but Event not created/closed!");
            }

            return itemEvent;
        }
    }
}