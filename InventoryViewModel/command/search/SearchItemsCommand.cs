// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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
        public enum SearchStatus
        {
            NoSearch,
            Pending,
            Complete
        }

        /// <summary>
        /// state of search, none started, till pending, or results available
        /// </summary>
        public SearchStatus searchStatus
        {
            get { return _searchStatus; }
            set { SetProperty(ref _searchStatus, value, nameof(searchStatus)); }
        }
        private SearchStatus _searchStatus = SearchStatus.NoSearch;

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
        /// helper routine for testing, should not be called by normal applications
        /// will cause the current thread to sleep, periodically checking for the query to complete
        /// Only returns once query has completed.
        /// </summary>
        public void WaitForSearchToComplete()
        {
            while (searchStatus != SearchItemsCommand.SearchStatus.Complete)
            {
                System.Threading.Thread.Sleep(100); // give async query time to complete
            }
        }


        async private void GetItemTreeAsync(SearchFilterOptions criteria)
        {
            logger.Info("GetItemTreeAsync");
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    logger.Info("GetItemTreeAsync query");
                    var db = DataRepository.GetDataRepository;
                    var items = db.GetItemTree(resultEntitySelector, criteria, out GenericItemResult item);

                    // update the view, but do so in WPF dispatcher thread
                    //Dispatcher.BeginInvoke((Action)(() => { searchResultViewModel.Items = items; }));
                    logger.Info($"GetItemTreeAsync:returned {items.Count} items.");
                    searchResultViewModel.Items = items;

                    // set selected item if one was returned to be selected
                    if (item == null)
                        logger.Info("GetItemTreeAsync: No item auto-selected.");
                    else
                        logger.Info($"GetItemTreeAsync: Auto-selected {item.ToString()}");
                    searchResultViewModel.SelectedItem = item;

                    // updated returned count
                    if (items.Count > 0)
                    {
                        searchResultViewModel.StatusMessage = $"Found {items.First().resultTotal} results.";
                    }
                    else
                    {
                        searchResultViewModel.StatusMessage = "No matches found.";
                    }
                }
                catch (Exception e)
                {
                    logger?.Error(e, $"Failed to load item tree! criteria:{criteria}");
                    return;
                }
                finally
                {
                    searchStatus = SearchStatus.Complete;
                }
            });
            logger.Info("GetItemTreeAsync end");
        }

        /// <summary>
        /// perform the search based on supplied criteria and return corresponding items
        /// </summary>
        /// <param name="searchFilterOptions">unused</param>
        private void QuerySearchCriteria(object searchFilterOptions)
        {
            searchResultViewModel.StatusMessage = null;
            searchStatus = SearchStatus.NoSearch;

            if (searchFilterOptions is SearchFilterOptions criteria)
            {
                logger.Info($"QuerySearchCriteria - {criteria}");
                searchStatus = SearchStatus.Pending;
                searchResultViewModel.StatusMessage = "Querying database ...";
                GetItemTreeAsync(criteria);
                /*
                var db = DataRepository.GetDataRepository;
                searchResultViewModel.Items = db.GetItemTree(resultEntitySelector, criteria, out GenericItemResult item);

                if (item == null)
                    logger.Info("No item auto-selected.");
                else
                    logger.Info($"Auto-selected {item.ToString()}");
                searchResultViewModel.SelectedItem = item;
                */
            }
        }
    }
}