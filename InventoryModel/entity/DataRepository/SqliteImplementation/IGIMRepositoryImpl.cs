// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;

using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Interface to database entities - all queries, updates, etc. handled via an instance of this class.
    /// Uses Database class to perform actual interaction with back-end database.
    /// </summary>
    public sealed partial class DataRepository : IGEMRepository
    {
        /// <summary>
        /// gets next unused ItemTypeId for ItemType
        /// </summary>
        /// <returns></returns>
        private int GetNextItemTypeId()
        {
            try
            {
                return db.ExecuteScalar<int>("SELECT coalesce(MAX(itemTypeId),0) FROM ItemType;") + 1; // on empty DB MAX() returns NULL so coalesce to 0
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to retrieve next free ItemTypeId! - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// returns a new ItemType instance with default values, also initialized any properties that
        /// refer to existing reference items and other items that require the DB to properly initialize.
        /// </summary>
        /// <returns></returns>
        public ItemType GetInitializedItemType()
        {
            try
            {
                var itemType = new ItemType
                {
                    // very important to keep this new item from replacing existing item!!!
                    // WARNING: this is only place that we create possible unresolvable db replication conflict!
                    itemTypeId = GetNextItemTypeId(),
                    // we don't hard code PK in case DB updated/changed and the primary keys of these items have changed
                    unitOfMeasure = ReferenceData[nameof(UnitOfMeasure)].ByName<UnitOfMeasure>("Each"),
                    batteryType = ReferenceData[nameof(BatteryType)].ByName<BatteryType>("None"),
                    category = ReferenceData[nameof(ItemCategory)].ByName<ItemCategory>("Treatment"),
                    //vendor = ReferenceData[nameof(VendorDetail)].ByName<VendorDetail>("???"),
                };
                return itemType;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to get newly initialized ItemType! - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// gets next unused ItemId for Item
        /// </summary>
        /// <returns></returns>
        private int GetNextItemId()
        {
            try
            {
                return db.ExecuteScalar<int>("SELECT coalesce(MAX(itemId),0) FROM Item;") + 1; // on empty DB MAX() returns NULL so coalesce to 0
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to retrieve next free ItemId! - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// returns a new Item instance with default values, also initialized any properties that
        /// refer to existing reference items and other items that require the DB to properly initialize.
        /// optionally set parent and itemType
        /// </summary>
        /// <returns></returns>
        public Item GetInitializedItem(Item parent, ItemType itemType)
        {
            try
            {
                var item = new Item
                {
                    itemId = GetNextItemId(),
                    itemType = itemType,
                    parent = parent,
                    count = 1,
                    vehicleLocation = ReferenceData[nameof(VehicleLocation)].ByName<VehicleLocation>("Trailer"),
                };
                if (parent != null)
                {
                    item.unitType = parent.unitType;
                }
                return item;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to get newly initialized ItemType! - {e.Message}");
                throw;
            }
        }

        // what to include, OR together all wanted
        // Note: it is an internal DB consistency error if the same row is flagged as both bin and module; where Item=NOT (Bin OR Module)
        [Flags]
        public enum ItemAndContainerEnum
        {
            None = 0,
            Items = 1,
            Modules = 2,
            Bins = 4,
            Containers = Modules | Bins,
            All = Items | Containers
        }

        private ItemAndContainerEnum SearchFilterItemBoolsToEnum(SearchFilterItems searchFilter)
        {
            try
            {
                return (ItemAndContainerEnum)(
                    (searchFilter.IncludeItems ? ItemAndContainerEnum.Items : 0) |
                    (searchFilter.IncludeModules ? ItemAndContainerEnum.Modules : 0) |
                    (searchFilter.IncludeBins ? ItemAndContainerEnum.Bins : 0)
                );
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in SearchFilterItemBoolsToEnum() - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// returns tree of ItemInstance, Item, or ItemType to display
        /// Only includes limited information for display, full entity must be loaded for all details/editing
        /// </summary>
        /// <param name="searchFilter">criteria to filter search results</param>
        /// <param name="selectedItem">If not null, and search results in a single selected Item result, will be set to non null value</param>
        /// <returns></returns>
        public ObservableCollection<SearchResult> GetItemTree(QueryResultEntitySelector resultEntitySelector, SearchFilterItems searchFilter, out GenericItemResult selectedItem)
        {
            // default to no specific singular item found
            selectedItem = null;

            try
            {
                // parameters to pass for query
                var queryParamsList = new List<string>();

                #region includeItemsModulesOrBins limits to just items, modules, bins or some combination

                // limit to just items, modules, bins or some combination
                string includeItemsModulesOrBins = " AND ";
                switch (SearchFilterItemBoolsToEnum(searchFilter))
                {
                    case ItemAndContainerEnum.All:
                        includeItemsModulesOrBins = "";  // don't add additional condition
                        break;

                    case ItemAndContainerEnum.Containers:
                        includeItemsModulesOrBins += "((isBin = 1) OR (isModule = 1))";
                        break;

                    case ItemAndContainerEnum.Items:
                        includeItemsModulesOrBins += "(((isBin IS NULL) OR (IsBin != 1)) AND ((isModule IS NULL) OR (isModule != 1)))";
                        break;

                    case ItemAndContainerEnum.Modules:
                        includeItemsModulesOrBins += "(isModule = 1)";  // assumes item can not be both a module and bin at same time
                        break;

                    case ItemAndContainerEnum.Bins:
                        includeItemsModulesOrBins += "(isBin = 1)";  // assumes item can not be both a module and bin at same time
                        break;

                    case ItemAndContainerEnum.Items | ItemAndContainerEnum.Modules:
                        includeItemsModulesOrBins += "((isBin IS NULL) OR (IsBin != 1))";  // basically not a bin
                        break;

                    case ItemAndContainerEnum.Items | ItemAndContainerEnum.Bins:
                        includeItemsModulesOrBins += "((isModule IS NULL) OR (isModule != 1))";  // basically not a module
                        break;

                    default:
                        // while silly, a person may request nothing
                        // throw new ArgumentOutOfRangeException("include", "DB.GetItemList invalid selection of what to return (items|modules|bins)!");
                        includeItemsModulesOrBins += "0"; // i.e. WHERE ??? AND 0 always returns no rows
                        break;
                }

                #endregion includeItemsModulesOrBins limits to just items, modules, bins or some combination

                #region which db fields to return and their mapping to GenericItemResult

                var Fields = string.Empty;
                if (resultEntitySelector == QueryResultEntitySelector.ItemInstance)
                    Fields = "'ItemInstance' AS entityType, ItemInstance.id as id, statusId, Item.id as parentItemId, ";
                else if (resultEntitySelector == QueryResultEntitySelector.Item)
                    Fields = "'Item' AS entityType, Item.id as id, ";
                else
                    Fields = "'ItemType' AS entityType, ItemType.id as id, ItemType.itemTypeId as itemNumber, ";
                if (resultEntitySelector != QueryResultEntitySelector.ItemType)
                    Fields += "ItemType.itemTypeId || '-' || Item.itemId AS itemNumber, count AS quantity, parentId, unitTypeName, ";
                Fields += "ItemType.name AS description, isModule, isBin";

                #endregion which db fields to return and their mapping to GenericItemResult

                // define our basic (joined) tables for our result entity
                const string TablesItemType = "ItemType";
                const string TablesItem = "(Item INNER JOIN " + TablesItemType + " ON Item.ItemTypeId=ItemType.id)";
                const string TablesItemInstance = "(ItemInstance INNER JOIN " + TablesItem + " ON ItemInstance.itemId=Item.id)";
                // default to minimal tables (to avoid JOIN based filtering), but query filters determines actual resulting join
                QueryResultEntitySelector requiredTables = resultEntitySelector;
                string JoinedTables = "{0}";

                string selectedSite = "(1)"; // so remaining conditions can just use "AND ..."
                // only limit by site location if it is a viable option (Note: site location is an ItemInstance field)
                if (searchFilter.SiteLocationEnabled)
                {
                    Fields += ", siteLocationId";
                    JoinedTables = $"(SiteLocation INNER JOIN {JoinedTables} ON SiteLocation.id=ItemInstance.siteLocationId)";
                    requiredTables = QueryResultEntitySelector.ItemInstance;

                    selectedSite = "(SiteLocation.id =?)";
                    queryParamsList.Add(searchFilter.User.siteId.ToString());
                }

                // optionally do not exclude removedFromService items
                string excludeOutOfServiceItems = string.Empty;
                if (true /*!searchFilter.AllowItemsRemovedFromService*/)
                {
                    if (resultEntitySelector == QueryResultEntitySelector.ItemInstance)
                    {
                        excludeOutOfServiceItems = "AND (ItemInstance.removedServiceDate IS NULL)";
                    }
                }

                // limit by equipment unit type e.g. DMSU only, MMRU and SSU, ...
                // either empty "" or list of desired unit types with each type surrounded in single quotes and comma separated
                // e.g. "'DMSU'"  OR  "'DMSU', 'SSU'"
                string selectedUnits = string.Empty;
                if (searchFilter.SelectEquipmentUnitsEnabled)
                {
                    selectedUnits = getConditionFromList(searchFilter, searchFilter.SelectedEquipmentUnits, searchFilter.EquipmentUnits.Count, " AND (unitTypeName IN ({0}))");
                    // Note: we need INNER JOIN to exclude items when filtering, but if not filtering then any join
                    // will prevent ItemType not yet associated with an Item from being returned
                    if ((selectedUnits.Length > 0) && (requiredTables == QueryResultEntitySelector.ItemType))
                        requiredTables = QueryResultEntitySelector.Item;
                }

                #region selectedStatus limits by item instance status

                // limit by item status
                string selectedStatus = string.Empty;
                if (searchFilter.SelectItemStatusValuesEnabled)
                {
                    selectedStatus = getConditionFromList(searchFilter, searchFilter.SelectedItemStatusValues, searchFilter.ItemStatusValues.Count, " AND (ItemStatus.name IN ({0}))");
                    if (selectedStatus.Length > 0)
                    {
                        if (resultEntitySelector != QueryResultEntitySelector.ItemInstance)
                            requiredTables = QueryResultEntitySelector.ItemInstance;
                        //Fields += ", statusId";  // only available for ItemInstance queries to allow activating buttons on overview lists
                        JoinedTables = $"(ItemStatus INNER JOIN {JoinedTables} ON ItemStatus.id=ItemInstance.statusId)";
                    }
                }

                #endregion selectedStatus limits by item instance status

                #region selectedCategories limits by item category

                // limit by item category
                string selectedCategories = "";
                if (searchFilter.SelectItemCategoryValuesEnabled)
                {
                    selectedCategories = getConditionFromList(searchFilter, searchFilter.SelectedItemCategoryValues, searchFilter.ItemCategoryValues.Count, " AND (ItemCategory.name IN ({0}))");
                    if (selectedCategories.Length > 0)
                    {
                        JoinedTables = $"(ItemCategory INNER JOIN {JoinedTables} ON ItemCategory.id=ItemType.itemCategoryId)";
                    }
                }

                #endregion selectedCategories limits by item category

                string itemDesc = "";
                if (!string.IsNullOrWhiteSpace(searchFilter.SearchText))
                {
                    ItemNumberParser itemNumberParser = new ItemNumberParser(searchFilter.SearchText);
                    if (itemNumberParser.IsItemNumber())
                    {
                        // WARNING: these values are validated by regex to only contain digits and letters
                        // however to avoid SQL injection or other issues, passed as parameter

                        // site location, Note: overrides drop down selection if using OnlyExact
                        // limits to a specific item instance if used in conjunction with item id
                        if (searchFilter.SiteLocationEnabled && !string.IsNullOrEmpty(itemNumberParser.siteCode) && (searchFilter.ItemTypeMatching == SearchFilterItemMatching.OnlyExact))
                        {
                            itemDesc += "(SiteLocation.locSuffix=?)";
                            queryParamsList.Add(itemNumberParser.siteCode);
                        }

#if false
                        // equipment unit, Note: redundant as also part of item id; overrides drop down selection
                        if (searchFilter.SelectEquipmentUnitsEnabled && !string.IsNullOrEmpty(itemNumberParser.equipCode))
                        {
                            if (itemDesc.Length > 0) itemDesc += " AND ";
                            itemDesc += "(EquipmentUnitType.unitCode=?)";
                            queryParamsList.Add(itemNumberParser.equipCode);
                        }
#endif

                        // item type, search by id instead of partial description match
                        if (!string.IsNullOrEmpty(itemNumberParser.itemTypeId))
                        {
                            if (int.TryParse(itemNumberParser.itemTypeId, out int itemType))
                            {
                                if (itemDesc.Length > 0) itemDesc += " AND ";
                                itemDesc += "(ItemType.itemTypeId=?)";
                                queryParamsList.Add(Convert.ToString(itemType));
                            }
                            else
                            {
                                logger.Error($"DB.GetItemTree - bad itemNumberParser.itemTypeId, Not an integer! [{itemNumberParser.itemTypeId}]");
                            }
                        }

                        // limit to specific item only makes sense for item and itemInstance, not itemType
                        if (resultEntitySelector != QueryResultEntitySelector.ItemType)
                        {
                            // item id, limits search to specific item if "onlyExact" (i.e. not "AnySame")
                            if (!string.IsNullOrEmpty(itemNumberParser.itemTypeId) && (searchFilter.ItemTypeMatching == SearchFilterItemMatching.OnlyExact))
                            {
                                if (int.TryParse(itemNumberParser.itemId, out int itemId))
                                {
                                    if (itemDesc.Length > 0) itemDesc += " AND ";
                                    itemDesc += "(Item.itemId=?)";
                                    queryParamsList.Add(Convert.ToString(itemId));
                                }
                                else
                                {
                                    logger.Error($"DB.GetItemTree - bad itemNumberParser.itemId, Not an integer! [{itemNumberParser.itemId}]");
                                }
                            }
                        }

                        itemDesc = $" AND ({itemDesc})";
                    }
                    else
                    {
                        itemDesc = " AND (ItemType.name LIKE ?)";
                        queryParamsList.Add("%" + searchFilter.SearchText + "%"); // force wildcards so matches anywhere in name
                    }
                }

                // update JoinedTables based on what is required
                JoinedTables = string.Format(JoinedTables,
                    (requiredTables == QueryResultEntitySelector.ItemInstance) ? TablesItemInstance :
                    (requiredTables == QueryResultEntitySelector.Item) ? TablesItem :
                    TablesItemType);

                // Note: SQL aggregates (e.g. SUM()) always return at least 1 row [on NULL row] even if otherwise no rows,
                // so to avoid extraneous row being returned, use aggregates in subquery
                // Also note that using aggregate will limit rows returned to 1st of given type
                // Note: {selectedSite} must be 1st so remaining conditions can use "AND ..." without seeing if prior condition

                // get exact items, if they have them, their parents and grandparents
                var ItemAndTypeQuery = $"FROM {JoinedTables} WHERE ";
                var matchingItemsQuery =
                        $"{ItemAndTypeQuery} {selectedSite} {excludeOutOfServiceItems} {selectedUnits} {includeItemsModulesOrBins} {selectedStatus} {selectedCategories} ";
                var matchingExactItemsQuery =
                        $"{matchingItemsQuery} {itemDesc} ";
                var matchingItems =
                        $"SELECT DISTINCT {Fields} {matchingExactItemsQuery}";
                var parentItemsOf =
                        $"{ItemAndTypeQuery} ((isBin = 1) OR (isModule = 1)) AND Item.id IN (SELECT parentId ";
                var parentItemsQuery =
                        $"{parentItemsOf} {matchingExactItemsQuery}) ";
                var parentItems =
                        $"SELECT DISTINCT {Fields} {parentItemsQuery}";
                var parentOfParentItems =
                        $"SELECT DISTINCT {Fields} {parentItemsOf} {parentItemsQuery})";
                // if item queried is not a bin or module, then need sibling items
                var siblingItemsQuery =
                        $"{ItemAndTypeQuery} ((isBin <> 1) AND (isModule <> 1)) AND Item.parentId IN (SELECT Item.id {parentItemsQuery}) ";
                var siblingItems =
                        $"SELECT DISTINCT {Fields} {siblingItemsQuery}";
                // if item queried is a bin or module, then need its children
                var childItemsQuery =
                        $"{ItemAndTypeQuery} Item.parentId IN (SELECT Item.id {matchingExactItemsQuery})";
                var childItems =
                        $"SELECT DISTINCT {Fields} {childItemsQuery}";
                // add grandchildren
                var grandchildItems =
                        $"SELECT DISTINCT {Fields} " +
                        $"{ItemAndTypeQuery} Item.parentId IN (SELECT Item.id {childItemsQuery})";

                string SQL;
                if (string.IsNullOrEmpty(itemDesc) || (resultEntitySelector == QueryResultEntitySelector.ItemType))
                {
                    // not searching by any particular item so no need for UNION portions to find parents or kids of query'd items
                    SQL = $"{matchingItems} ORDER BY ItemType.name;";
                }
                else
                {
                    // each union query takes same parameters, so put into array so can ensure parameter count always matches union count
                    string[] unionQueries = { $"{matchingItems}", $"{parentItems}", $"{parentOfParentItems}", $"{siblingItems}", $"{childItems}", $"{grandchildItems}" };
                    StringBuilder sqlBuilder = new StringBuilder();
                    foreach (var s in unionQueries)
                    {
                        if (sqlBuilder.Length > 0) sqlBuilder.Append(" UNION ");
                        sqlBuilder.Append(s);
                    }
                    sqlBuilder.Append(" ORDER BY ItemType.name;");
                    SQL = sqlBuilder.ToString();
                    // we need to repeat queryParams for each union group
                    var UnionGroupCount = unionQueries.Length;
                    var tempList = new List<string>(queryParamsList.Count * UnionGroupCount);
                    for (int i = 1; i <= UnionGroupCount; i++)
                    {
                        foreach (var qParam in queryParamsList)
                        {
                            tempList.Add(qParam);
                        }
                    }
                    queryParamsList = tempList;
                }

                logger.Debug(SQL);
                logger.Debug(statePrinter.PrintObject(queryParamsList));
                var queryItems = db.QueryAsync<GenericItemResult>(SQL, queryParamsList.ToArray()).Result;

                ObservableCollection<SearchResult> results;

                if (resultEntitySelector == QueryResultEntitySelector.ItemType)
                {
                    // get a count of distinct Items of type ItemType,
                    // i.e. if there are items Y and Z of type T in unit A and item X of type T in unit B
                    // then we would return 3 if units A & B selected, or 2 if only unit A is selected.
                    // Note: this is not a count of how many (sum of counts) there are, i.e. if item Y
                    // has a count 5 (e.g. 5 gloves) and only a count of 1 for X and Z then there
                    // 7 total Items (5 + 1 + 1 = 7 gloves) for units A and B, but only 3 Items (X, Y, & Z)
                    var query = $"SELECT COUNT(Item.id) FROM Item INNER JOIN ItemType ON Item.ItemTypeId=ItemType.id WHERE ItemType.itemTypeId=? {selectedUnits};";
                    foreach (var itemResult in queryItems)
                    {
                        itemResult.quantity = db.ExecuteScalar<int>(query, itemResult.itemNumber);
                    }

                    // if returning item types then do not include any headers, no nesting either
                    results = new ObservableCollection<SearchResult>(queryItems);
                    // auto select if only 1 ItemType returned
                    if (results.Count == 1) selectedItem = results.FirstOrDefault() as GenericItemResult;
                }
                else
                {
                    // if equipment list not shown (or null) then assume returning results for all available units
                    var equipList = ((searchFilter.SelectEquipmentUnitsEnabled) ? searchFilter.SelectedEquipmentUnits : searchFilter.EquipmentUnits) ?? searchFilter.EquipmentUnits;
                    if (equipList == null)
                    {
                        results = getChildItems(searchFilter, null, null, null, queryItems, ref selectedItem);
                    }
                    else
                    {
                        var equips = new ObservableCollection<SearchResult>(new List<SearchResult>(equipList.Count));

                        SearchResult siteResult = null;
                        SiteLocation site = null;
                        if (searchFilter.SiteLocationEnabled)
                        {
                            site = searchFilter.User.availableSites.Where(x => x.id == searchFilter.User.siteId).FirstOrDefault();
                            siteResult = new GroupHeader() { id = site.id, description = site.name, children = equips };
                            results = new ObservableCollection<SearchResult> { siteResult };
                        }
                        else
                        {
                            results = equips;
                        }

                        foreach (var equipObj in equipList)
                        {
                            if (equipObj is EquipmentUnitType equip)
                            {
                                var equipResult = new EquipmentUnitResult() { name = equip.name, description = equip.description, parent = siteResult };
                                equipResult.children = getChildItems(searchFilter, site, equip, equipResult, queryItems, ref selectedItem);
                                // add equipment to results, pk is equip.name not a Guid
                                // don't include if nothing for that equipment ???
                                if (equipResult.childCount > 0)
                                    equips.Add(equipResult);
                            }
                        }
                    }
                }

                return results;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error retrieving Item Tree! - {e.Message}");
                // return an empty list
                return new ObservableCollection<SearchResult>();
            }
        }

        private string getConditionFromList(SearchFilterItems searchFilter, IList<object> selectedList, int maxSelectable, string conditionFormat)
        {
            string queryCondition = "";  // default to all units, ie no additional condition
            if (selectedList != null)
            {
                // if selected count = available count then assume all selected, so no additional condition
                // otherwise limit to results within selected set
                if (selectedList.Count != maxSelectable)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var i in selectedList)
                    {
                        // get this item's value to match with in DB
                        string name = "";
                        {
                            if (i is ReferenceData refData) name = refData.name;

                            if (i is EquipmentUnitType unit) name = unit.name;

                            if (i is SiteLocation site) name = site.name;

                            if (name == string.Empty) throw new NotImplementedException("Unexpected type in generating DB query condition from List!");
                        }

                        {
                            if (sb.Length > 0) sb.Append(", ");  // 2nd or more item in list
                            sb.Append("'");
                            sb.Append(name);
                            sb.Append("'");
                        }
                    }
                    queryCondition = string.Format(conditionFormat, sb.ToString());
                }
            }
            return queryCondition;
        }

        private ObservableCollection<SearchResult> getChildItems(SearchFilterItems searchFilter, SiteLocation site, EquipmentUnitType equip, SearchResult parent, IList<GenericItemResult> itemResults, ref GenericItemResult selectedItem)
        {
            try
            {
                // if called on an item (not module or bin) then just return immediately, assume nothing inside a non-container item
                //if ((parentItem != null) && !(parentItem.isModule || parentItem.isBin)) return null;
                var parentItem = parent as GenericItemResult; // will be null if not a item, item instance, or item type

                var bins = new ObservableCollection<SearchResult>();
                var ghBins = new BinGroupHeader() { description = "Bins", parent = parent, children = bins };

                var modules = new ObservableCollection<SearchResult>();
                var ghModules = new ModuleGroupHeader() { description = "Modules", parent = parent, children = modules };

                var items = new ObservableCollection<SearchResult>();
                var ghItems = new ItemGroupHeader() { description = "Items", parent = parent, children = items };

                foreach (var item in itemResults)
                {
                    // must be child of current parent (null for top level items), and for current site and equipment unit
                    if ((item != parentItem) && ((item.parentId == parentItem?.id) || (item.parentId == parentItem?.parentItemId) || ((parentItem == null) && (Guid.Empty == item.parentId))) && ((site == null) || (item.siteLocationId == site.id)) && ((equip == null) || (item.unitTypeName.Equals(equip.name))))
                    {
                        // bins
                        if (item.isBin)
                        {
                            bins.Add(item);
                            item.parent = ghBins;
                        }

                        // modules
                        if (item.isModule)
                        {
                            modules.Add(item);
                            item.parent = ghModules;
                        }

                        // items
                        if (!(item.isModule || item.isBin))
                        {
                            items.Add(item);
                            item.parent = ghItems;
                        }

                        // iff a single item searched for, then select it
                        ItemNumberParser itemNumberParser = new ItemNumberParser(searchFilter.SearchText);
                        if (itemNumberParser.IsItemNumber())
                        {
                            // itemNumber is ItemType.itemTypeId || '-' || Item.itemId for item instances and items
                            if (string.Equals(item.itemNumber, itemNumberParser.itemTypeId + '-' + itemNumberParser.itemId, StringComparison.InvariantCultureIgnoreCase))
                            {
                                item.IsExpanded = true;
                                item.IsSelected = true;
                                selectedItem = item;
                            }
                        }

                        // find all children (and so on) for this item
                        if (item.isBin || item.isModule)
                        {
                            item.children = getChildItems(searchFilter, site, equip, item, itemResults, ref selectedItem);
                        }
                    }
                }

                // create our child collection, always contains headers and possible list of bins, modules, and items.
                var children = new ObservableCollection<SearchResult>(new List<GroupHeader>(3));

                // bins are not contained within any other item, i.e. top level only; and there are bins
                if (/*(parentItem == null) &&*/ (bins.Count > 0))
                {
                    children.Add(ghBins);
                }
                // modules can be in bins or at the top level; and there are modules
                if (/*((parentItem == null) || (parentItem.isBin)) &&*/ (modules.Count > 0))
                {
                    children.Add(ghModules);
                }
                // items can be at any level, but only add if there items
                if (items.Count > 0)
                {
                    children.Add(ghItems);
                }

                return children;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in getChildItems - {e.Message}");
                logger.Error(e.StackTrace);
                throw;
            }
        }

        public IList<ItemInstance> GetItemInstancesWithinBinOrModuleAsync(ItemInstance itemInstance)
        {
            try
            {
                var SQL = "INNER JOIN Item on Item.id=ItemInstance.itemId WHERE (Item.parentId IN (?)) AND (ItemInstance.siteLocationId = ?);";

                // parameters to pass for query
                var queryParamsList = new List<string>
                {
                    itemInstance.item.id.ToString(),
                    itemInstance.siteLocationId.ToString()
                };

                logger.Debug("SELECT ItemInstance.* FROM ItemInstance " + SQL);
                logger.Debug(statePrinter.PrintObject(queryParamsList));

                var results = db.LoadRows<ItemInstance>(SQL, queryParamsList.ToArray());
                return results;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Exception in GetItemInstancesWithinBinOrModuleAsync - {e.Message}");
                logger.Error(e.StackTrace);
                throw;
            }
        }

        public IList<Item> AllBinsAndModules()
        {
            return db.LoadRows<Item>("INNER JOIN ItemType on Item.itemTypeId=ItemType.id WHERE (isBin=1) OR (isModule=1)");
        }

        public IList<ItemInstance> AllItemInstancesForItem(Guid itemPk)
        {
            return db.LoadRows<ItemInstance>("WHERE itemId=?", itemPk);
        }
    }
}