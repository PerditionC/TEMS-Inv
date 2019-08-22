// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TEMS.InventoryModel.entity.db.user;
using TEMS.InventoryModel.userManager;
using TEMS.InventoryModel.util;

namespace TEMS.InventoryModel.entity.db.query
{
    /// <summary>
    /// See SearchFilterOptions.ItemTypeMatching, who search criteria is applied, any of same type of item(s) or exactly specified item(s)
    /// </summary>
    public enum SearchFilterItemMatching
    {
        AnySame = 0,
        OnlyExact = 1
    }

    /// <summary>
    /// maintains complete search/filter criteria for items to display/modify in corresponding item list/tree
    /// </summary>
    public class SearchFilterOptions : NotifyPropertyChanged //, ICloneable
    {
        /// <summary>
        /// initialize defaults for search/filter criteria for items to display/modify in corresponding item list/tree
        /// specific Windows should initialize used search criteria
        /// </summary>
        public SearchFilterOptions() : base()
        {
            TitleFunc = new Func<object, string>((o) =>
            {
                if (o is ReferenceData refData) return refData.name;

                if (o is EquipmentUnitType unit) return unit.name;

                if (o is ItemBase item) return item.PrimaryKey.ToString();

                logger.Error("SearchFilterItem.TitleFunc pass unknown object instance: ItemsSource collection contains unknown items!  " + o.ToString());
                return "?" + o.ToString();
            });
        }

        /// <summary>
        /// initialize to match same search criteria as an existing search filter
        /// </summary>
        /// <param name="searchFilterItems"></param>
        public void InitializeAs(SearchFilterOptions searchFilterItems)
        {
            IncludeBins = searchFilterItems.IncludeBins;
            IncludeModules = searchFilterItems.IncludeModules;
            IncludeItems = searchFilterItems.IncludeItems;
            SearchText = searchFilterItems.SearchText;
            ItemTypeMatching = searchFilterItems.ItemTypeMatching;
            SelectEquipmentUnitsEnabled = searchFilterItems.SelectEquipmentUnitsEnabled;
            SelectEquipmentUnitsVisible = searchFilterItems.SelectEquipmentUnitsVisible;
            SelectedEquipmentUnits = searchFilterItems.SelectedEquipmentUnits;
            SelectItemCategoryValuesEnabled = searchFilterItems.SelectItemCategoryValuesEnabled;
            SelectItemCategoryValuesVisible = searchFilterItems.SelectItemCategoryValuesVisible;
            SelectedItemCategoryValues = searchFilterItems.SelectedItemCategoryValues;
            SelectItemStatusValuesEnabled = searchFilterItems.SelectItemStatusValuesEnabled;
            SelectItemStatusValuesVisible = searchFilterItems.SelectItemStatusValuesVisible;
            SelectedItemStatusValues = searchFilterItems.SelectedItemStatusValues;

            SearchFilterVisible = searchFilterItems.SearchFilterVisible;
        }

        /// <summary>
        /// initialize to default settings
        /// </summary>
        public void Initialize()
        {
            // initialize SearchFilter, 
            // Note: search is not triggered until SearchFilterEnabled == true, so can set values in any order
            var db = DataRepository.GetDataRepository;
            User = UserManager.GetUserManager.CurrentUser();

            // default to all choices possible with all selected
            ItemStatusValues = new List<object>(db.ReferenceData[nameof(ItemStatus)]);
            SelectedItemStatusValues = new List<object>(ItemStatusValues);

            ItemCategoryValues = new List<object>(db.ReferenceData[nameof(ItemCategory)]);
            SelectedItemCategoryValues = new List<object>(ItemCategoryValues);

            // load only trailers at currently selected user location
            //EquipmentUnits = new List<object>(db.ReferenceData[nameof(EquipmentUnitType)]);
            EquipmentUnits = new ObservableCollection<Object>(User.currentSite.equipmentUnitTypesAvailable);
            SelectedEquipmentUnits = new List<object>(EquipmentUnits);

            SelectItemStatusValuesVisible = false;
            // don't include out of service items by default either
        }


        /// <summary>
        /// maps our objects to strings to display in MultiSelectComboBox dropdown (or other list selection dialog),
        /// instead of using ToString()
        /// </summary>
        public Func<object, string> TitleFunc
        {
            get { return _TitleFunc; }
            set { SetProperty<Func<object, string>>(ref _TitleFunc, value, nameof(TitleFunc)); }
        }

        private Func<object, string> _TitleFunc = null;

        #region User information

        public UserDetail User
        {
            get { return _User; }
            set { SetProperty<UserDetail>(ref _User, value, nameof(User)); }
        }

        private UserDetail _User;

        #endregion User information

        #region Is site information visible? enabled?

        /// <summary>
        /// If Site Location section should be shown?
        /// </summary>
        public bool SiteLocationVisible
        {
            get { return _SiteLocationVisible; }
            set { SetProperty<bool>(ref _SiteLocationVisible, value, nameof(SiteLocationVisible)); }
        }

        private bool _SiteLocationVisible = true;

        /// <summary>
        /// Does Site Location effect the search results?
        /// </summary>
        public bool SiteLocationEnabled
        {
            get { return _SiteLocationEnabled; }
            set { SetProperty<bool>(ref _SiteLocationEnabled, value, nameof(SiteLocationEnabled)); }
        }

        private bool _SiteLocationEnabled = true;

        #endregion Is site information visible? enabled?

        #region Is Search / Filter section visible? enabled?

        /// <summary>
        /// If search/filter options should be shown to user, Visible, Collapsed or Hidden
        /// </summary>
        public bool SearchFilterVisible
        {
            get { return _SearchFilterVisible; }
            set { SetProperty<bool>(ref _SearchFilterVisible, value, nameof(SearchFilterVisible)); }
        }

        private bool _SearchFilterVisible = true;

        /// <summary>
        /// Overall toggle to enable search.  Initially false, should be set to true to after all values initialized.
        /// Used to prevent searching until all criteria is specified.  
        /// ViewModel should listen for this to be toggled to enabled and trigger an initial search and new search after 
        /// each update until disabled (set to false).
        /// </summary>
        public bool SearchFilterEnabled
        {
            get { return _SearchFilterEnabled; }
            set { SetProperty<bool>(ref _SearchFilterEnabled, value, nameof(SearchFilterEnabled)); }
        }

        private bool _SearchFilterEnabled = false;

        #endregion Is Search / Filter section visible? enabled?

        #region Search / Filter text

        /// <summary>
        /// Search text - either [partial] external item # or [partial] description text
        /// </summary>
        public string SearchText
        {
            get { return _SearchText; }
            set { SetProperty(ref _SearchText, value, nameof(SearchText)); }
        }

        private string _SearchText = null;

        /// <summary>
        /// Search includes only items that explicitly match rest of the search criteria
        /// or all items of the same item type for any matching items; i.e. all bandaids at all sites in a all places
        /// vs just the bandaids in a unit A at location X [possibly limited further by item# to site Y]
        /// </summary>
        public SearchFilterItemMatching ItemTypeMatching
        {
            get { return _ItemTypeMatching; }
            set { SetProperty(ref _ItemTypeMatching, value, nameof(ItemTypeMatching)); }
        }

        private SearchFilterItemMatching _ItemTypeMatching = SearchFilterItemMatching.AnySame;

        #endregion Search / Filter text

        #region Include items, modules, bins

        /// <summary>
        /// include Items
        /// </summary>
        public bool IncludeItems
        {
            get { return _IncludeItems; }
            set { SetProperty<bool>(ref _IncludeItems, value, nameof(IncludeItems)); }
        }

        private bool _IncludeItems = true;

        /// <summary>
        /// include Modules
        /// </summary>
        public bool IncludeModules
        {
            get { return _IncludeModules; }
            set { SetProperty<bool>(ref _IncludeModules, value, nameof(IncludeModules)); }
        }

        private bool _IncludeModules = true;

        /// <summary>
        /// include Bins
        /// </summary>
        public bool IncludeBins
        {
            get { return _IncludeBins; }
            set { SetProperty<bool>(ref _IncludeBins, value, nameof(IncludeBins)); }
        }

        private bool _IncludeBins = true;

        #endregion Include items, modules, bins

        #region Equipment Units

        /// <summary>
        /// If should be shown to user
        /// </summary>
        public bool SelectEquipmentUnitsVisible
        {
            get { return _SelectEquipmentUnitsVisible; }
            set { SetProperty<bool>(ref _SelectEquipmentUnitsVisible, value, nameof(SelectEquipmentUnitsVisible)); }
        }

        private bool _SelectEquipmentUnitsVisible = true;

        /// <summary>
        /// Does it effect search results?
        /// </summary>
        public bool SelectEquipmentUnitsEnabled
        {
            get { return _SelectEquipmentUnitsEnabled; }
            set { SetProperty<bool>(ref _SelectEquipmentUnitsEnabled, value, nameof(SelectEquipmentUnitsEnabled)); }
        }

        private bool _SelectEquipmentUnitsEnabled = true;

        /// <summary>
        /// a collection of selected equipment units
        /// null if option not visible so none ever selected
        /// </summary>
        public IList<object> SelectedEquipmentUnits
        {
            get { return _SelectedEquipmentUnits; }
            set { SetProperty<IList<object>>(ref _SelectedEquipmentUnits, value, nameof(SelectedEquipmentUnits)); }
        }

        private IList<object> _SelectedEquipmentUnits = null;

        /// <summary>
        /// a collection of available equipment units
        /// </summary>
        public IList<Object> EquipmentUnits
        {
            get { return _EquipmentUnits; }
            set { SetProperty<IList<Object>>(ref _EquipmentUnits, value, nameof(EquipmentUnits)); }
        }

        private IList<Object> _EquipmentUnits;

        #endregion Equipment Units

        #region Item Status values

        /// <summary>
        /// If should be shown to user
        /// </summary>
        public bool SelectItemStatusValuesVisible
        {
            get { return _SelectedItemStatusValuesVisible; }
            set { SetProperty<bool>(ref _SelectedItemStatusValuesVisible, value, nameof(SelectItemStatusValuesVisible)); }
        }

        private bool _SelectedItemStatusValuesVisible = true;

        /// <summary>
        /// Does it effect search results?
        /// </summary>
        public bool SelectItemStatusValuesEnabled
        {
            get { return _SelectedItemStatusValuesEnabled; }
            set { SetProperty<bool>(ref _SelectedItemStatusValuesEnabled, value, nameof(SelectItemStatusValuesEnabled)); }
        }

        private bool _SelectedItemStatusValuesEnabled = true;

        /// <summary>
        /// a collection of selected Item Status values
        /// null if option not visible so none ever selected
        /// </summary>
        public IList<object> SelectedItemStatusValues
        {
            get { return _SelectedItemStatusValues; }
            set { SetProperty<IList<object>>(ref _SelectedItemStatusValues, value, nameof(SelectedItemStatusValues)); }
        }

        private IList<object> _SelectedItemStatusValues = null;

        /// <summary>
        /// a collection of available Item Status values
        /// </summary>
        public IList<object> ItemStatusValues
        {
            get { return _ItemStatusValues; }
            set { SetProperty<IList<object>>(ref _ItemStatusValues, value, nameof(ItemStatusValues)); }
        }

        private IList<object> _ItemStatusValues = null;

        #endregion Item Status values

        #region Item Category values

        /// <summary>
        /// If should be shown to user
        /// </summary>
        public bool SelectItemCategoryValuesVisible
        {
            get { return _SelectItemCategoryValuesVisible; }
            set { SetProperty<bool>(ref _SelectItemCategoryValuesVisible, value, nameof(SelectItemCategoryValuesVisible)); }
        }

        private bool _SelectItemCategoryValuesVisible = true;

        /// <summary>
        /// Does it effect search results?
        /// </summary>
        public bool SelectItemCategoryValuesEnabled
        {
            get { return _SelectItemCategoryValuesEnabled; }
            set { SetProperty<bool>(ref _SelectItemCategoryValuesEnabled, value, nameof(SelectItemCategoryValuesEnabled)); }
        }

        private bool _SelectItemCategoryValuesEnabled = true;

        /// <summary>
        /// a collection of selected Item Category values
        /// null if option not visible so none ever selected
        /// </summary>
        public IList<object> SelectedItemCategoryValues
        {
            get { return _SelectedItemCategoryValues; }
            set { SetProperty<IList<object>>(ref _SelectedItemCategoryValues, value, nameof(SelectedItemCategoryValues)); }
        }

        private IList<object> _SelectedItemCategoryValues = null;

        /// <summary>
        /// a collection of available Item Category values
        /// </summary>
        public IList<object> ItemCategoryValues
        {
            get { return _ItemCategoryValues; }
            set { SetProperty<IList<object>>(ref _ItemCategoryValues, value, nameof(ItemCategoryValues)); }
        }

        private IList<object> _ItemCategoryValues = null;

        #endregion Item Category values
    }
}