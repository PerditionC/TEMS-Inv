// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.ObjectModel;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util;
using TEMS_Inventory.views;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// Initiates a search for items based on SearchFilterOptions
    /// </summary>
    public class SearchItemsCommand : RelayCommand
    {
        private QueryResultEntitySelector resultEntitySelector = QueryResultEntitySelector.ItemInstance;
        private SearchResultViewModel searchResultViewModel = null;

        public SearchItemsCommand(QueryResultEntitySelector resultEntitySelector, SearchResultViewModel searchResultViewModel) : base()
        {
            _canExecute = IsValidParameters;
            _execute = QuerySearchCriteria;
            this.resultEntitySelector = resultEntitySelector;
            this.searchResultViewModel = searchResultViewModel;
        }

        /// <summary>
        /// Check that a SearchFilterOptions criteria object is provided
        /// </summary>
        /// <param name="searchFilterOptions">the search criteria</param>
        /// <returns>true if valid search criteria specified and can perform search</returns>
        private bool IsValidParameters(object searchFilterOptions)
        {
            if (searchFilterOptions is SearchFilterOptions criteria)
                return criteria.User != null;
            return false;
        }

        /// <summary>
        /// perform the search based on supplied criteria and return corresponding items
        /// </summary>
        /// <param name="searchFilterOptions">unused</param>
        private void QuerySearchCriteria(object searchFilterOptions)
        {
            if (searchFilterOptions is SearchFilterOptions criteria)
            {
                var db = DataRepository.GetDataRepository;
                searchResultViewModel.Items = db.GetItemTree(resultEntitySelector, criteria, out GenericItemResult item);
                searchResultViewModel.SelectedItem = item;
            }
        }
    }
}