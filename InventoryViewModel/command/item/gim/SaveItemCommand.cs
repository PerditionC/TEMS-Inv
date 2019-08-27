// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.util;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// Saves (inserts/updates) entity in the database.
    /// Parameter is a single object, the entity to save
    /// If entity is an Item, and not already in the DB then will also insert (create) associated ItemInstance records
    /// </summary>
    public class SaveItemCommand : RelayCommand
    {
        public SaveItemCommand() : base(UpdateItem, IsValidParameters) { }

        private static bool IsValidParameters(object entity)
        {
            // by default assume can always save
            var canSave = true;

            // if entity supports change tracking then disable save if no changes
            if (entity is IChangeTracking itemWithChangeTracking) canSave &= itemWithChangeTracking.IsChanged;

            // if entity supports Null constraint checking then disable save if constraint not satisfied
            if (entity is ItemBase itemWithNullConstraintCheck) canSave &= itemWithNullConstraintCheck.AreNonNullConstraintsSatisfied();

            return canSave;
        }

        private static void UpdateItem(object entity)
        {
            var db = DataRepository.GetDataRepository;


            if (entity is Item item)
            {
                if (item == null) throw new ArgumentException("AddItem can only add new Item objects!");

                // determine if Item is already in DB so updating, or does not exist so need to insert along with associated ItemInstances
                if ((item.id == null) || (item.id == Guid.Empty) || /* item.id != null/Empty && */ !db.Exists(item))
                try
                {

                    // for each site, add a corresponding new item instance
                    var itemInstanceList = GetItemInstances(item);

                    // wrap saving item and itemInstances in transaction so all or none saved
                    db.BeginTransaction();
                    {
                        // save our Item and ItemInstances
                        db.Save(item);
                        foreach (var ii in itemInstanceList)
                        {
                            db.Save(ii);
                        }
                    }
                    // close transaction, actually commit the changes to db (any constraint violations occur here)
                    db.EndTransaction();
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Failed to add item {item.itemNumber}!");
                    throw;
                }
            }
            else
            {
                // perform the save, note may throw Exception if unable to save
                db.Save(entity);
            }
        }


        /// <summary>
        /// helper method to iterate through sites and create new ItemInstance objects
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sites"></param>
        /// <returns>List/ItemInstance> objects created</returns>
        private static List<ItemInstance> GetItemInstances(Item item)
        {
            // for each site, add a corresponding new item instance
            var itemInstanceList = new List<ItemInstance>();
            var available = (ItemStatus)DataRepository.GetDataRepository.ReferenceData[nameof(ItemStatus)].ByName<ItemStatus>("Available");
            var sites = DataRepository.GetDataRepository.ReferenceData[nameof(SiteLocation)].Where(site => ((SiteLocation)site).equipmentUnitTypesAvailable.Any(unitType => unitType == item.unitType));
            foreach (var site in sites)
            {
                var itemInstance = new ItemInstance()
                {
                    item = item,
                    siteLocation = (SiteLocation)site,
                    status = available,
                    inServiceDate = DateTime.Now.Date,

                    /* left as defaults
                     * serialNumber
                     * grantNumber
                     * removedServiceDate
                     * isSealBroken
                     * hasBarcode
                     * notes
                     */
                };
                itemInstanceList.Add(itemInstance);
            }
            return itemInstanceList;
        }
    }
}