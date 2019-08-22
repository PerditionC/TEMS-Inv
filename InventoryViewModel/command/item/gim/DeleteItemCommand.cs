// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.util;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// When an Item is deleted, ensures all corresponding ItemInstance's are also deleted.
    /// </summary>
    public class DeleteItemCommand : RelayCommand
    {
        public DeleteItemCommand() : base()
        {
            // can only be passed to base if static, we need instance data for callback so set here
            _execute = DeleteItem;
            _canExecute = IsValidParameters;
        }

        /// <summary>
        /// validates parameters are valid
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static bool IsValidParameters(object parameters)
        {
            // parameters may be null
            if (parameters is Item item)
            {
                // validate that Item is not already in DB, can not add if already exists
                if ((item.id != null) && (item.id != Guid.Empty))
                {
                    // verify Item exists (can't delete what isn't there)
                    return DataRepository.GetDataRepository.Exists(item);
                }
            }
            //logger.Warn($"{nameof(DeleteItemCommand.IsValidParameters)} invoked with invalid parameter, must pass in Item object!");
            return false;
        }

        /// <summary>
        /// Given an Item, removes Item from db along with corresponding ItemInstances.
        /// </summary>
        /// <param name="parameters"></param>
        private void DeleteItem(object parameters)
        {
            Item item = (Item)parameters; // throw error if any other object type provided
            if (item == null) throw new ArgumentException("DeleteItem can only remove Item objects!");

            var db = DataRepository.GetDataRepository;
            try
            {
                // for each site, add a corresponding new item instance
                var itemInstanceList = db.AllItemInstancesForItem(item.id);

                // wrap saving item and itemInstances in transaction so all or none saved
                db.BeginTransaction();
                {
                    // save our Item and ItemInstances
                    foreach (var ii in itemInstanceList)
                    {
                        db.Delete(ii);
                    }
                    db.Delete(item);
                }
                // close transaction, actually commit the changes to db (any constraint violations occur here)
                db.EndTransaction();
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to delete item {item.itemNumber}!");
                // attempt to cancel transaction if exception during delete
                try
                {
                    db.EndTransaction();
                } catch (Exception) { /* eat exception */ }
                throw;
            }
        }
    }
}