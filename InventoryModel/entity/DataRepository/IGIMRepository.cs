// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using System.Collections.ObjectModel;

using TEMS.InventoryModel.entity.db.query;

namespace TEMS.InventoryModel.entity.db
{
    public enum QueryResultEntitySelector
    {
        ItemInstance = 2,
        Item = 1,
        ItemType = 0
    }

    /// <summary>
    /// Interface to database entities - all queries, updates, etc. handled via an instance of this class.
    /// Uses Database class to perform actual interaction with back-end database.
    /// </summary>
    public interface IGEMRepository  //  GIM item lookups
    {
        /// <summary>
        /// returns a new ItemType instance with default values, also initialized any properties that
        /// refer to existing reference items and other items that require the DB to properly initialize.
        /// </summary>
        /// <returns></returns>
        ItemType GetInitializedItemType();

        /// <summary>
        /// returns tree of ItemInstance, Item, or ItemType to display
        /// Only includes limited information for display, full entity must be loaded for all details/editing
        /// </summary>
        /// <param name="searchFilter">criteria to filter search results</param>
        /// <param name="selectedItem">If not null, and search results in a single selected Item result, will be set to non null value</param>
        /// <returns></returns>
        ObservableCollection<SearchResult> GetItemTree(QueryResultEntitySelector resultEntitySelector, SearchFilterItems searchFilter, out GenericItemResult selectedItem);

        /// <summary>
        ///
        /// </summary>
        /// <param name="itemInstance"></param>
        /// <returns></returns>
        IList<ItemInstance> GetItemInstancesWithinBinOrModuleAsync(ItemInstance itemInstance);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IList<Item> AllBinsAndModules();
    }
}