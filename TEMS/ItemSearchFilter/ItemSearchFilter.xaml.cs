// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NLog;
using TEMS.InventoryModel.command.action;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.entity.db.user;
using TEMS.InventoryModel.util;


namespace TEMS_Inventory.UserControls
{
    /// <summary>
    /// Interaction logic for ItemSearchFilter.xaml
    /// </summary>
    public partial class ItemSearchFilter : UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // used to listen for various property changes
        PropertyObserver<SearchItemsCommand> _observer;
        PropertyObserver<UserDetail> _userObserver;

        public ItemSearchFilter()
        {
            InitializeComponent();
            // Note: SearchFilter is null at this time
            SearchTextCommand = new RelayCommand(param => DoSearch(), param => CanSearch());

            this.Unloaded += ItemSearchFilter_Unloaded;
        }

        /// <summary>
        /// removes any registered property change handlers when UserControl unloaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ItemSearchFilter_Unloaded(object sender, RoutedEventArgs args)
        {
            try
            {
                _userObserver?.UnregisterHandler(n => n.siteId);
                UnRegisterHandlers();
            }
            catch (Exception e)
            {
                logger.Error(e, "Dispose - error removing property change handlers");
            }
        }



        #region SearchFilter

        /// <summary>
        /// maintains information about active search/filter criteria
        /// </summary>
        public SearchItemsCommand SearchFilterCommand
        {
            get { return (SearchItemsCommand)GetValue(SearchFilterCommandProperty); }
            set { SetValue(SearchFilterCommandProperty, value); }
        }

        public static readonly DependencyProperty SearchFilterCommandProperty =
            DependencyProperty.Register("SearchFilterCommand", typeof(SearchItemsCommand), typeof(ItemSearchFilter),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ItemSearchFilter.OnSearchFilterChanged)));

        private static void OnSearchFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            logger.Debug("SearchFilter dependency property changed.");
            var me = d as ItemSearchFilter;
            // if currently running, remove existing handlers, possibly avoid memory leak
            me?.UnRegisterHandlers();
            // [re]instate our handlers for when any search criteria other than site changes
            me?.RegisterHandlers();
        }

        /// <summary>
        /// watch for changes to User property, specifically the user's selected site
        /// </summary>
        /// <param name="searchFilter"></param>
        void UserChangedHandler(SearchItemsCommand searchFilterCommand)
        {            
            try
            {
                _userObserver?.UnregisterHandler(n => n.siteId);
                _userObserver = new PropertyObserver<UserDetail>(searchFilterCommand.SearchFilter.User).RegisterHandler(n => n.siteId, updateSite);
            }
            catch (Exception e)
            {
                logger.Error(e, "UserChangeHander");
            }

            // force initial update to site specific information
            updateSite(searchFilterCommand.SearchFilter.User);
        }


        /// <summary>
        /// stop listening for any changes to our search/filter options
        /// </summary>
        private void UnRegisterHandlers()
        {
            try
            {
                if (_observer != null)
                {
                    _observer
                        .UnregisterHandler(n => n.SearchFilter.SearchFilterEnabled)
                        .UnregisterHandler(n => n.SearchFilter.User)
                        .UnregisterHandler(n => n.SearchFilter.ItemCategoryValues)
                        .UnregisterHandler(n => n.SearchFilter.ItemStatusValues)
                        .UnregisterHandler(n => n.SearchFilter.SelectedEquipmentUnits)
                        .UnregisterHandler(n => n.SearchFilter.SelectedItemCategoryValues)
                        .UnregisterHandler(n => n.SearchFilter.SelectedItemStatusValues)
                        .UnregisterHandler(n => n.SearchFilter.IncludeItems)
                        .UnregisterHandler(n => n.SearchFilter.IncludeModules)
                        .UnregisterHandler(n => n.SearchFilter.IncludeBins)
                        .UnregisterHandler(n => n.SearchFilter.ItemTypeMatching)
                        .UnregisterHandler(n => n.SearchFilter.SearchText)
                    ;
                }
            }
            catch (Exception e)
            {
                logger.Warn(e, "Item Search Filter - unregister handlers.");
            }

        }

        /// <summary>
        /// start listening for any changes to our search/filter options and trigger criteria changed accordingly
        /// </summary>
        private void RegisterHandlers()
        {
            try
            {
                _observer = new PropertyObserver<SearchItemsCommand>(SearchFilterCommand)
                    .RegisterHandler(n => n.SearchFilter.SearchFilterEnabled, SearchCriteriaChanged)  // trigger search when enabled
                    // watch for User changed so can track active site
                    .RegisterHandler(n => n.SearchFilter.User, UserChangedHandler)
                    // default to all choices selected (so automatically select all when initialized)
                    // EquipmentUnits purposely not registered here, see updateSite()
                    // Note: we create new List() objects for selected values from initial set, otherwise a user may .Clear() selection which will also empty full List
                    .RegisterHandler(n => n.SearchFilter.ItemCategoryValues, sF => sF.SearchFilter.SelectedItemCategoryValues = new List<object>(sF.SearchFilter.ItemCategoryValues))
                    .RegisterHandler(n => n.SearchFilter.ItemStatusValues, sF => sF.SearchFilter.SelectedItemStatusValues = new List<object>(sF.SearchFilter.ItemStatusValues))
                    // when user changes selection, trigger a search criteria has changed so reload list,etc.
                    .RegisterHandler(n => n.SearchFilter.SelectedEquipmentUnits, SearchCriteriaChanged)
                    .RegisterHandler(n => n.SearchFilter.SelectedItemCategoryValues, SearchCriteriaChanged)
                    .RegisterHandler(n => n.SearchFilter.SelectedItemStatusValues, SearchCriteriaChanged)
                    .RegisterHandler(n => n.SearchFilter.IncludeItems, SearchCriteriaChanged)
                    .RegisterHandler(n => n.SearchFilter.IncludeModules, SearchCriteriaChanged)
                    .RegisterHandler(n => n.SearchFilter.IncludeBins, SearchCriteriaChanged)
                    .RegisterHandler(n => n.SearchFilter.ItemTypeMatching, SearchCriteriaChanged)
                    .RegisterHandler(n => n.SearchFilter.SearchText, SearchTextChanged)
                ;
            }
            catch (Exception e)
            {
                logger.Warn(e, "Item Search Filter - register handlers.");
            }
        }

        /// <summary>
        /// when site changed/initialize, load values into dependent lists (equip for the site)
        /// </summary>
        /// <param name="user"></param>
        public void updateSite(UserDetail user)
        {
            logger.Debug("Updating site specific information based on user's [new] site.");

            // get the user's current site (whatever it was changed/initialized to)
            var site = user.currentSite; 
            // load list of equipment available for current site
            // however, this causes SelectedEquipmentUnits to be set to null which triggers
            // SearchCriteriaChanged via RegisterHandler call, so we temp unregister then re-register
            // thus avoiding an unnecessary call to SearchCriteriaChanged and ultimately possibly long DB load
            _observer?.UnregisterHandler(n => n.SearchFilter.SelectedEquipmentUnits);
            SearchFilterCommand.SearchFilter.EquipmentUnits = new ObservableCollection<Object>(site.equipmentUnitTypesAvailable);
            _observer?.RegisterHandler(n => n.SearchFilter.SelectedEquipmentUnits, SearchCriteriaChanged);

            // Note: this will trigger normal criteria changed actions via SearchCriteriaChanged()
            SearchFilterCommand.SearchFilter.SelectedEquipmentUnits = new List<object>(SearchFilterCommand.SearchFilter.EquipmentUnits);
        }

        /// <summary>
        /// Invoke supplied user command to indicate search criteria has changed
        /// </summary>
        /// <param name="searchItemsCommand"></param>
        private void SearchCriteriaChanged(SearchItemsCommand searchItemsCommand)
        {
            logger.Debug("Search criteria changed!");
            if (searchItemsCommand != null)
            {
                if (searchItemsCommand.SearchFilter.SearchFilterEnabled && searchItemsCommand.CanExecute(null)) searchItemsCommand.Execute(null);
            }
        }

        #endregion // SearchFilter


        #region SearchTextCommand

        /// <summary>
        /// command to execute when search button (or Enter) is pressed
        /// </summary>
        public ICommand SearchTextCommand { get; set; }

        public bool SearchTextAvailable
        {
            get { return (bool)GetValue(SearchTextAvailableProperty); }
            set { SetValue(SearchTextAvailableProperty, value); }
        }
        public static readonly DependencyProperty SearchTextAvailableProperty =
            DependencyProperty.Register("SearchTextAvailable", typeof(bool), typeof(ItemSearchFilter),
            new FrameworkPropertyMetadata(false));

        private void SearchTextChanged(SearchItemsCommand searchFilterCommand)
        {
            SearchTextAvailable = (searchFilterCommand?.SearchFilter?.SearchText?.Length ?? 0) > 0;
        }


        // need to also be able to search when search text is cleared,
        // but without enabling search if nothing previously searched
        private bool lastSearchTextNotEmpty = false;

        private bool CanSearch()
        {
            // if user input anything then allow searching
            return SearchTextAvailable || lastSearchTextNotEmpty;
        }

        private void DoSearch()
        {
            logger.Debug("Searching! " + SearchFilterCommand.SearchFilter.SearchText);
            // store for next call to determine if okay to search on clearing text
            lastSearchTextNotEmpty = SearchTextAvailable;
            // trigger the search
            SearchCriteriaChanged(SearchFilterCommand);
        }

        #endregion // SearchTextCommand
    }
}
