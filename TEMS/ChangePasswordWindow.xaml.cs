// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows;
using TEMS.InventoryModel.userManager.extension;
using TEMS.InventoryModel.entity.db.user;
using TEMS_Inventory.views;

namespace TEMS_Inventory
{
    /// <summary>
    /// Interaction logic for ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        private ChangePasswordViewModel ViewModel;

        /// <summary>
        /// Initialize view and set user object we are changing password hash for
        /// </summary>
        /// <param name="user">user to set new password for</param>
        public ChangePasswordWindow(ChangePasswordViewModel ViewModel)
        {
            this.ViewModel = ViewModel;
            //this.DataContext = ViewModel;

            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SetPassword_Click(object sender, RoutedEventArgs e)
        {
            using (var pw = passwordBox.Text.ToSecureString())
            {
                if (ViewModel.SetPasswordCommand.CanExecute(pw))
                {
                    ViewModel.SetPasswordCommand.Execute(pw);
                }
                pw?.Clear();
            }

            this.Close();
        }
    }
}
