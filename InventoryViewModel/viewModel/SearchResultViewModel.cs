// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.ObjectModel;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.command.action;
using TEMS.InventoryModel.entity.db.query;


/** in Window, need to init
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
 */

namespace TEMS_Inventory.views
{
    public class SearchResultViewModel : ViewModelBase
    {
        private OnSelectionChangedCommand onSelectionChangedCommand;

        /// <summary>
        /// anything that needs initializing
        /// </summary>
        /// <param name="onSelectionChangedCommand"></param>
        public SearchResultViewModel(OnSelectionChangedCommand onSelectionChangedCommand) : base()
        {
            if (onSelectionChangedCommand is null) throw new ArgumentNullException(nameof(onSelectionChangedCommand), "Action to perform on selection change cannot be null!");
            this.onSelectionChangedCommand = onSelectionChangedCommand;
        }


        /// <summary>
        /// maintains results from last Search (or empty list initially)
        /// </summary>
        public ObservableCollection<SearchResult> Items
        {
            get { return _Items; }
            set { SetProperty(ref _Items, value, nameof(Items)); }
        }
        private ObservableCollection<SearchResult> _Items = new ObservableCollection<SearchResult>();

        /// <summary>
        /// maintains currently selected item from last search (or null if nothing currently selected)
        /// Note: this value may be set by update to search results, updated from binding to user list and new item
        /// selected by user, or set explicitly (e.g. new item created and marked as selected)
        /// </summary>
        public SearchResult SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                // clear status on new selection
                StatusMessage = string.Empty;

                // mark newly selected item is selected and expanded to be visible
                if (value != null)
                {
                    value.IsSelected = true;
                    value.IsExpanded = true;
                }

                SetProperty(ref _SelectedItem, value, nameof(SelectedItem));
                RaisePropertyChanged(nameof(IsSelectedItem));

                // invoke command to update detail pane with new selection
                if (onSelectionChangedCommand.CanExecute(_SelectedItem)) onSelectionChangedCommand.Execute(_SelectedItem);
            }
        }
        private SearchResult _SelectedItem;

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
            get { return InitializeCommand(ref _ExpandCollapseCommand, new Action<object>(DoExpandCollapse), _ => { return (Items != null) && (Items.Count > 0); }); }
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
                var doExpand = (bool)arg;
                recursiveExpandCollapse(Items, doExpand);
                if (doExpand)
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
                    // so don't expand header, but do need to collapse them
                    if (IsExpanded)
                    {
                        if (!(child is GroupHeader))
                            child.IsExpanded = true;
                    }
                    else
                    {
                        child.IsExpanded = false;
                    }
                }
            }
        }

        #endregion // Expand/Collapse tree

    }
}
