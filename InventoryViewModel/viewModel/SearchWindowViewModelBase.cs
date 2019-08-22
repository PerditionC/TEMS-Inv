// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.command.action;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.userManager;


namespace TEMS_Inventory.views
{
    public abstract class SearchWindowViewModelBase : ViewModelBase
    {
        public SearchWindowViewModelBase(QueryResultEntitySelector resultEntitySelector) : this(resultEntitySelector, null) { }

        // anything that needs initializing
        public SearchWindowViewModelBase(QueryResultEntitySelector resultEntitySelector, SearchFilterOptions searchFilter) : base()
        {
            SearchFilterCommand = new SearchItemsCommand(resultEntitySelector, searchFilter);

            // initialize SearchFilter, 
            // Note: search is not triggered until SearchFilterEnabled == true, so can set values in any order
            var db = DataRepository.GetDataRepository;
            SearchFilter.User = UserManager.GetUserManager.CurrentUser();

            // default to all choices possible with all selected
            SearchFilter.ItemStatusValues = new List<object>(db.ReferenceData[nameof(ItemStatus)]);
            SearchFilter.SelectedItemStatusValues = new List<object>(SearchFilter.ItemStatusValues);

            SearchFilter.ItemCategoryValues = new List<object>(db.ReferenceData[nameof(ItemCategory)]);
            SearchFilter.SelectedItemCategoryValues = new List<object>(SearchFilter.ItemCategoryValues);

            SearchFilter.EquipmentUnits = new List<object>(db.ReferenceData[nameof(EquipmentUnitType)]);
            SearchFilter.SelectedEquipmentUnits = new List<object>(SearchFilter.EquipmentUnits);

            SearchFilter.SelectItemStatusValuesVisible = false;
            // don't include out of service items by default either

            // from this point on, trigger changes based on any changes to our SearchFilter
            SearchFilterCommand.PropertyChanged += SearchCmd_PropertyChanged;
            SearchFilterCommand.SearchFilter.PropertyChanged += SearchFilter_PropertyChanged;
        }

        private void SearchFilter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            logger.Debug($"Search Filter: Property[{e.PropertyName}]\n" + SearchFilter.ToString());

            // as long as enabled then trigger a new search on any change (ignore changes to IsChanged as this would result in double searches)
            if (!nameof(SearchFilter.IsChanged).Equals(e.PropertyName, StringComparison.InvariantCulture) && SearchFilter.SearchFilterEnabled)
            {
                if (SearchFilterCommand.CanExecute(null)) SearchFilterCommand.Execute(null);
            }
        }

        /// <summary>
        /// Exposes search criteria and results
        /// </summary>
        public SearchFilterOptions SearchFilter { get { return SearchFilterCommand.SearchFilter; } }

        /// <summary>
        /// Our implementation of searching for ItemInstance, Item, and ItemType objects
        /// </summary>
        public SearchItemsCommand SearchFilterCommand
        {
            get { return _SearchItemsCommand; }
            set
            {
                SetProperty(ref _SearchItemsCommand, value, nameof(SearchItemsCommand));
                RaisePropertyChanged(nameof(SearchFilter));
            }
        }
        private SearchItemsCommand _SearchItemsCommand = null;

        /// <summary>
        /// Called to notify of changes to search results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchCmd_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            logger.Debug($"Search Command: Property[{e.PropertyName}]\n" + SearchFilter.ToString());
            var db = DataRepository.GetDataRepository;

            if (string.IsNullOrEmpty(e.PropertyName) || nameof(SearchFilterCommand.selectedItem).Equals(e.PropertyName, StringComparison.InvariantCulture))
            {
                // if only selected item changed, only trigger change to SelectedItem
                RaisePropertyChanged(nameof(SelectedItem));
            }
            else if (nameof(SearchFilterCommand.searchResults).Equals(e.PropertyName, StringComparison.InvariantCulture))
            {
                // if items changed then need to also update selected item
                RaisePropertyChanged(nameof(Items));
                RaisePropertyChanged(nameof(SelectedItem));
                // update status with results of search
                var count = 0;
                foreach (var topItem in Items)
                {
                    count += topItem.resultTotal;
                }
                StatusMessage = $"Search returned {count} results.";
            }
        }

        /// <summary>
        /// maintains results from last Search (or empty list initially)
        /// </summary>
        public ObservableCollection<SearchResult> Items { get { return SearchFilterCommand.searchResults; } }

        /// <summary>
        /// maintains currently selected item from last search (or null if nothing currently selected)
        /// Note: this value may be set by update to search results, updated from binding to user list and new item
        /// selected by user, or set explicitly (e.g. new item created and marked as selected)
        /// </summary>
        public SearchResult SelectedItem
        {
            get { return SearchFilterCommand.selectedItem; }
            set
            {
                // if currently selected item, update not IsSelected
                if (SearchFilterCommand.selectedItem != null) SearchFilterCommand.selectedItem.IsSelected = false;
                // mark newly selected item is selected and expanded to be visible
                if (value != null)
                {
                    value.IsSelected = true;
                    value.IsExpanded = true;
                }
                SearchFilterCommand.selectedItem = value;
                RaisePropertyChanged(nameof(SelectedItem));
                RaisePropertyChanged(nameof(IsSelectedItem));
                // clear status on new selection
                StatusMessage = string.Empty;
            }
        }

        /// <summary>
        /// Is there a currently selected item
        /// </summary>
        /// <returns></returns>
        public bool IsSelectedItem
        {
            get { return SelectedItem != null; }
        }


        #region Expand/Collapse tree

        /// <summary>
        /// information displayed next to ExpandCollapse check box
        /// </summary>
        public string ExpandCollapseCommandText
        {
            get { return _ExpandCollapseCommandText; }
            set { SetProperty(ref _ExpandCollapseCommandText, value, nameof(ExpandCollapseCommandText)); }
        }
        private string _ExpandCollapseCommandText = "Expand All";

        /// <summary>
        /// command to execute when user requests to expand/collapse tree
        /// </summary>
        public ICommand ExpandCollapseCommand
        {
            get { return InitializeCommand(ref _ExpandCollapseCommand, new Action<object>(DoExpandCollapse), param => { return (Items != null) && (Items.Count > 0); }); }
        }
        private ICommand _ExpandCollapseCommand;

        /// <summary>
        /// Command action to perform tree expansion/collapse
        /// </summary>
        /// <param name="arg">where to expand (true) or collapse(false) - does nothing if not true or false</param>
        private void DoExpandCollapse(object arg)
        {
            logger.Debug("Expanding items - DoExpandCollapse:\n" + arg?.ToString());

            if (arg != null)
            {
                var isChecked = (bool)arg;
                recursiveExpandCollapse(Items, isChecked);
                if (isChecked)
                    ExpandCollapseCommandText = "Collapse All";
                else
                    ExpandCollapseCommandText = "Expand All";
            }
        }

        /// <summary>
        /// Recursively calls self to set items children to expanded/collapsed (which also causes self to be expanded if needed)
        /// </summary>
        /// <param name="items">the items to expand/collapse</param>
        /// <param name="IsExpanded">true to expand, false to collapse</param>
        private void recursiveExpandCollapse(ObservableCollection<SearchResult> items, bool IsExpanded)
        {
            if (items != null)
            {
                foreach (var child in items)
                {
                    recursiveExpandCollapse(child.children, IsExpanded);
                    // Note: setting IsExpanded true will automatically expand parents, but doesn't collapse them automatically when set false
                    // don't collapse header
                    if (!(child is GroupHeader)) child.IsExpanded = IsExpanded;
                }
            }
        }

        #endregion // Expand/Collapse tree

    }
}
