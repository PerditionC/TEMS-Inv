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
    /// When an Item is added, ensures all corresponding ItemInstance's are also added .
    /// </summary>
    public class AddItemCommand : RelayCommand
    {
        public AddItemCommand() : base()
        {
            // can only be passed to base if static, we need instance data for callback so set here
            _execute = AddItem;
            _canExecute = IsValidParameters;
        }

        /// <summary>
        /// callback that is invoked after item is loaded
        /// </summary>
        //public Action<object> ItemLoaded = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static bool IsValidParameters(object parameters)
        {
            if (parameters is Item item)
            {
                // validate that Item is not already in DB, can not add if already exists
                if ((item.id != null) && (item.id != Guid.Empty))
                {
                    // Note: AddItem should not be called if ItemInstances already exist, but
                    // not a problem if Item exists, however, its a simpler check for Item as
                    // it should not exist without corresponding ItemInstances.
                    return !DataRepository.GetDataRepository.Exists(item);
                }
            }
            logger.Warn($"{nameof(AddItemCommand.IsValidParameters)} invoked with invalid parameter, must pass in Item object!");
            return false;
        }

        /// <summary>
        /// Given a new Item, adds Item to db along with corresponding ItemInstances.
        /// </summary>
        /// <param name="parameters"></param>
        private void AddItem(object parameters)
        {
            Item item = (Item)parameters; // throw error if any other object type provided
            if (item == null) throw new ArgumentException("AddItem can only add new Item objects!");

            try
            {
                var db = DataRepository.GetDataRepository;

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
            var sites = DataRepository.GetDataRepository.ReferenceData[nameof(SiteLocation)].Where(site => ((SiteLocation)site).equipmentUnitTypesAvailable.Any(unitType => unitType == item.unitType) );
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