// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif
using NLog;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// basic 3 pane window to allow searching and viewing details of results
    /// 1 is the search options pane (SearchFilterOptionsViewModel or descendant) - area with search filters
    /// *additional options need not be adjacent to main search options visually in view presentation
    /// 1 is the results pane  (SearchResultViewModel) - area with tree or list view of search with options applied
    /// 1 is the detail pane (DetailsViewModelBase descendant) - area with information about element selected from search results
    /// </summary>
    public class SearchDetailWindowViewModel : ViewModelBase
    {
        public SearchDetailWindowViewModel(SearchFilterOptionsViewModel optionsVM,
                                           SearchResultViewModel resultVM,
                                           DetailsViewModelBase detailsPaneVM) : base()
        {
            this.SearchFilterOptions = optionsVM;
            this.SearchResult = resultVM;
            this.Details = detailsPaneVM;
        }


        public SearchFilterOptionsViewModel SearchFilterOptions
        {
            get { return _SearchFilterOptions; }
            set { SetProperty(ref _SearchFilterOptions, value, nameof(SearchFilterOptions)); }
        }
        private SearchFilterOptionsViewModel _SearchFilterOptions;

        public SearchResultViewModel SearchResult
        {
            get { return _SearchResult; }
            set { SetProperty(ref _SearchResult, value, nameof(SearchResult)); }
        }
        private SearchResultViewModel _SearchResult;

        public DetailsViewModelBase Details
        {
            get { return _Details; }
            set { SetProperty(ref _Details, value, nameof(Details)); }
        }
        private DetailsViewModelBase _Details;
    }
}
