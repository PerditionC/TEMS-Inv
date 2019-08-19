// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.user;
using TEMS.InventoryModel.userManager;
using TEMS_Inventory.views;

namespace TEMS_Inventory
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public LogInWindow()
        {
            logger.Trace(nameof(LogInWindow));
            InitializeComponent();
            // ensure user can immediately start typing in username
            userId.Focus();
            logger.Debug("Waiting for login.");
        }

        /// <summary>
        /// Indicates if user has input information to validate for login yet or not.
        /// true if enough data entered that validation may be attempted; false if more input needed
        /// </summary>
        public Boolean hasInputCredentials { get { return !string.IsNullOrWhiteSpace(userId.Text) && !string.IsNullOrWhiteSpace(userPassword.Password); } }

        /// <summary>
        /// Enables (or disables) Login button
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void enableLoginBtn(object sender, RoutedEventArgs e)
        {
            loginBtn.IsEnabled = hasInputCredentials;
        }

        /// <summary>
        /// Validate userid and password then either proceed or display error accordingly.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            logger.Trace(nameof(LoginBtn_Click));
            var userManager = UserManager.GetUserManager;
            if (userManager.ValidateUser(userId.Text, userPassword.SecurePassword, out UserDetail user))
            {
                // consider user logged into until program exits or they logout
                userManager.LoginUser(user);

                // if user's password has expired then prompt to change it
                if ((bool)forcePasswordChange.IsChecked || user.isPasswordExpired)
                {
                    logger.Info("User's password is expired - showing dialog to set new passphrase.");
                    DoSetPassword(user);
                }
                proceedToMainWindow();
            }
            else
            {
                logger.Warn($"Unable to validate user '{userId.Text.Trim().ToLowerInvariant()}'");
                MessageBox.Show("Unable to validate user, please verify username & password then retry!", "Login Error:", MessageBoxButton.OK);
                userPassword.Password = "";
                userPassword.Focus();
            }
        }

        /// <summary>
        /// Create and show dialog if user must change password
        /// </summary>
        private void DoSetPassword(UserDetail user)
        {
            logger.Trace(nameof(DoSetPassword));
            var setPasswordWindow = new ChangePasswordWindow(new ChangePasswordViewModel(user));
            if (user.isPasswordExpired) setPasswordWindow.Title = "Current passphrase expired!";
            // don't show cancel button so forced to change [note: can still X out of window to cancel]
            setPasswordWindow.Cancel.Visibility = Visibility.Collapsed;
            setPasswordWindow.Owner = App.Current.MainWindow; /* this */
            setPasswordWindow.ShowDialog();
        }

        /// <summary>
        /// Creates and transfers control to program's MainWindow
        /// </summary>
        private void proceedToMainWindow()
        {
            logger.Trace(nameof(proceedToMainWindow));
            var newWin = new MainWindow();
            App.Current.MainWindow = newWin;
            newWin.Show();
            this.Close();
        }


        /// <summary>
        /// Close and exit program.
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            // simply close LoginWindow, no other windows should be open and we should cleanly exit
            Close();
            // but just to be sure no other threads left running, force a shutdown
            Application.Current.Shutdown(0);
        }
    }
}
