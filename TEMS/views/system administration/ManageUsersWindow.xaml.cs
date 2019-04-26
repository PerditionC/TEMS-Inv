// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.user;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for ManageUsersWindow.xaml
    /// </summary>
    public partial class ManageUsersWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ManageUsersWindow()
        {
            // delegate (local function) used to create new instances from Add command
            ItemBase GetNewItem() { return (ItemBase)Activator.CreateInstance(typeof(UserDetail)); }
            ViewModel = new ManageUsersViewModel(GetNewItem);
            this.DataContext = ViewModel;

            InitializeComponent();
        }


        public ManageUsersViewModel ViewModel
        {
            get { return (ManageUsersViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ManageUsersViewModel), typeof(ManageUsersWindow));
    }
}
