// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif
using TEMS.InventoryModel.command.action;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.userManager;

namespace TEMS_Inventory.views // InventoryViewModel.viewModel
{
    /// <summary>
    /// performs search filtering (search pane VM) to get a list of items (for result pane VM)
    /// </summary>
    public class ItemSearchFilterViewModel : ViewModelBase
    {
        public ItemSearchFilterViewModel(SearchFilterOptions searchFilterOptions, QueryResultEntitySelector resultEntitySelector, SearchResultViewModel searchResultViewModel) : base()
        {
            this.SearchFilter = searchFilterOptions;
            SearchFilterCommand = new SearchItemsCommand(resultEntitySelector, searchResultViewModel);
        }

        /// <summary>
        /// does actual search based on filter criteria
        /// contains 'SearchFilterOptions SearchFilter' which maintains all search options, e.g. text to search for, locations, etc.
        /// </summary>
        public SearchFilterOptions SearchFilter
        {
            get { return _searchFilterOptions; }
            private set
            {
                if (_searchFilterOptions != null) UnRegisterHandlers();
                SetProperty(ref _searchFilterOptions, value, nameof(SearchFilter));
                if (value != null)
                {
                    RegisterHandlers();
                    // notify any listeners that we have valid values now
                    RaisePropertyChanged("User.availableSites");
                    RaisePropertyChanged("EquipmentUnits");
                    RaisePropertyChanged("ItemStatusValues");
                    RaisePropertyChanged("ItemCategoryValues");
                }
            }
        }
        private SearchFilterOptions _searchFilterOptions = null;


        /// <summary>
        /// ICommand that performs actual query based on query criteria
        /// </summary>
        public SearchItemsCommand SearchFilterCommand { get; private set; }

        /// <summary>
        /// initialization
        /// </summary>
        public ItemSearchFilterViewModel()
        {
            RegisterHandlers();
        }

        /// <summary>
        /// cleanup - specifically ensure any events we subscribed to we un-subscribe
        /// </summary>
        ~ItemSearchFilterViewModel()
        {
            UnRegisterHandlers();
        }


        /// <summary>
        /// start listening for any changes to our search/filter options and trigger criteria changed accordingly
        /// </summary>
        private void RegisterHandlers()
        {
            try
            {
                // we need to know when any SearchFilter value changes so we can trigger updated search results
                SearchFilter.PropertyChanged += SearchFilter_PropertyChanged;
            }
            catch (Exception e)
            {
                logger.Warn(e, "Item Search Filter - register handlers.");
            }
        }

        /// <summary>
        /// stop listening for any changes to our search/filter options
        /// </summary>
        private void UnRegisterHandlers()
        {
            try
            {
                if (SearchFilter != null)
                {
                    SearchFilter.PropertyChanged -= SearchFilter_PropertyChanged;
                    if (SearchFilter.User != null)
                    {
                        SearchFilter.User.PropertyChanged -= User_PropertyChanged;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Warn(e, "Item Search Filter - unregister handlers.");
            }

        }


        private void SearchFilter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SearchText":
                    RaisePropertyChanged(nameof(SearchTextAvailable));
                    break;
                case "User":
                    // track when user changes settings, e.g. switches to different locality
                    // *** we have no way to remove from old User value if changed
                    if (SearchFilter?.User != null)
                    {
                        SearchFilter.User.PropertyChanged += User_PropertyChanged;
                    }
                    break;
                default:
                    // assume any other changes should trigger a new search
                    // limit if necessary
                    //if (e.PropertyName.Substring(1, 8) == "Selected")
                    {
                        SearchCriteriaChanged();
                    }
                    break;
            }
        }

        private void User_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "currentSite")
            {
                RaisePropertyChanged("currentSite");
                updateSite();
            }
        }


        /// <summary>
        /// when site changed/initialize, load values into dependent lists (equip for the site)
        /// </summary>
        private void updateSite()
        {
            logger.Debug("Updating site specific information based on user's [new] site.");

            // get the user's current site (whatever it was changed/initialized to)
            var site = SearchFilter?.User.currentSite;
            if (site != null)
            {
                // temp disable SearchFilter so don't run search until we are done re-initializing available and selected trailers
                SearchFilter.SearchFilterEnabled = false;
                SearchFilter.EquipmentUnits = new ObservableCollection<Object>(site.equipmentUnitTypesAvailable);
                SearchFilter.SelectedEquipmentUnits = new List<object>(SearchFilter.EquipmentUnits);
                // Note: this will trigger normal criteria changed actions via SearchCriteriaChanged()
                SearchFilter.SearchFilterEnabled = true;
                //SearchCriteriaChanged();
            }
        }


        /// <summary>
        /// property which returns true if user has input search text, false if search text is empty
        /// </summary>
        public bool SearchTextAvailable
        {
            get { return (SearchFilter?.SearchText?.Length ?? 0) > 0; }
        }


        /// <summary>
        /// command to execute when search button (or Enter) is pressed
        /// </summary>
        public ICommand SearchTextCommand
        {
            get { return InitializeCommand(ref _SearchTextCommand, param => SearchCriteriaChanged(), null); } // always allow user to search, blank just returns everything
        }
        private ICommand _SearchTextCommand;

        /// <summary>
        /// Invoke supplied user command to indicate search criteria has changed
        /// </summary>
        private void SearchCriteriaChanged()
        {
            logger.Debug($"Search criteria changed!  Searching: '{SearchFilter?.SearchText}'");

            // if valid objects and enabled then
            if (SearchFilter?.SearchFilterEnabled ?? false)
            {
                if (SearchFilterCommand.CanExecute(SearchFilter)) SearchFilterCommand.Execute(SearchFilter);
            }
        }

    }
}
