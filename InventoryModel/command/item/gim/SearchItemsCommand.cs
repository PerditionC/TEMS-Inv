// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.ObjectModel;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// Initiates a search for items based on SearchFilterItems
    /// </summary>
    public class SearchItemsCommand : RelayCommand
    {
        /// <summary>
        /// Exposes search criteria
        /// </summary>
        public SearchFilterItems SearchFilter { get; /*private*/ set; }

        public SearchItemsCommand(QueryResultEntitySelector resultEntitySelector) : this(resultEntitySelector, null) { }

        public SearchItemsCommand(QueryResultEntitySelector resultEntitySelector, SearchFilterItems SearchFilter) : base()
        {
            _canExecute = IsValidParameters;
            _execute = UpdateSearchResults;
            this.resultEntitySelector = resultEntitySelector;
            this.SearchFilter = SearchFilter ?? new SearchFilterItems();
        }

        /// <summary>
        /// what Type this search command returns, ItemType, Item, or ItemInstance?
        /// </summary>
        public QueryResultEntitySelector resultEntitySelector { get; private set; }

        /// <summary>
        /// public property with the results of last search (Execute() invocation)
        /// will contain an empty list if no results found (or no search yet done)
        /// </summary>
        public ObservableCollection<SearchResult> searchResults { get { return _searchResults; } set { SetProperty(ref _searchResults, value, nameof(searchResults)); } }

        private ObservableCollection<SearchResult> _searchResults = new ObservableCollection<SearchResult>(); // default to empty list instead of null

        /// <summary>
        /// may be null; if search criteria indicates a single item, then return it or other default selected value
        /// Note: this may be an item returned (GenericItemResult) or a header (GroupHeader)
        /// </summary>
        public SearchResult selectedItem { get { return _selectedItem; } set { SetProperty(ref _selectedItem, value, nameof(selectedItem)); } }

        private SearchResult _selectedItem = null;

        /// <summary>
        /// Check that a SearchFilterItems criteria object is provided
        /// </summary>
        /// <param name="parameters">unused</param>
        /// <returns></returns>
        private bool IsValidParameters(object parameters = null)
        {
            return SearchFilter.User != null;
        }

        /// <summary>
        /// perform the search based on supplied criteria and return corresponding items
        /// </summary>
        /// <param name="parameters">unused</param>
        private void UpdateSearchResults(object parameters = null)
        {
            var db = DataRepository.GetDataRepository;
            searchResults = db.GetItemTree(resultEntitySelector, SearchFilter, out GenericItemResult item);
            selectedItem = item;
        }
    }
}