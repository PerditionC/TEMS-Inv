// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS_Inventory.UserControls;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for ManageVendorsWindow.xaml
    /// </summary>
    public partial class ManageVendorsWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private PropertyDetails propertyDetails;

        public ManageVendorsWindow() : this(null) { }
        public ManageVendorsWindow(ManageVendorsViewModel ViewModel)
        {
            this.ViewModel = ViewModel ?? new ManageVendorsViewModel();
            this.DataContext = this.ViewModel;

            InitializeComponent();

            // create an instance of our property details creator and pass our container object
            propertyDetails = new PropertyDetails(DetailView);
            propertyDetails.AddDetails(typeof(VendorDetail));
        }


        public ManageVendorsViewModel ViewModel
        {
            get { return (ManageVendorsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ManageVendorsViewModel), typeof(ManageVendorsWindow));
    }
}
