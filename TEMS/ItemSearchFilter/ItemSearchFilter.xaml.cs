// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows;
using System.Windows.Controls;
using NLog;
using TEMS.InventoryModel.command.action;
using TEMS_Inventory.views;


namespace TEMS_Inventory.UserControls
{
    /// <summary>
    /// Interaction logic for ItemSearchFilter.xaml
    /// </summary>
    public partial class ItemSearchFilter : UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ItemSearchFilter()
        {
            ViewModel = new SearchFilterOptionsViewModel();
            this.DataContext = ViewModel;

            InitializeComponent();
        }


        public SearchFilterOptionsViewModel ViewModel
        {
            get { return (SearchFilterOptionsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(SearchFilterOptionsViewModel), typeof(ItemSearchFilter));


        /// <summary>
        /// does actual search based on filter criteria
        /// </summary>
        public SearchItemsCommand SearchFilterCommand
        {
            get { return (SearchItemsCommand)GetValue(SearchFilterCommandProperty); }
            //set { SetValue(SearchFilterCommandProperty, value); ViewModel.SearchFilterCommand = value; }
        }

        public static readonly DependencyProperty SearchFilterCommandProperty =
            DependencyProperty.Register("SearchFilterCommand", typeof(SearchItemsCommand), typeof(ItemSearchFilter));
    }
}
