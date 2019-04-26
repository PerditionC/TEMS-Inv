// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;
using System.ComponentModel;

using System.Reflection;
using NLog;
using TEMS.InventoryModel.util;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.userManager;
using TEMS.InventoryModel.entity.db.user;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();

            // show currently logged in user and site
            LoggedInUserInfo.DataContext = UserManager.GetUserManager.CurrentUser();
            this.DataContext = this;

            // register to handle Mediator callbacks, i.e. open new Windows and Dialogs etc.
            RegisterCallbacks();
        }

        public Boolean isAdmin
        {
            get { var user = UserManager.GetUserManager?.CurrentUser(); return (user != null) && user.isAdmin; }
        }


        /// <summary>
        /// Register our handling of the Mediator callbacks to open new windows and dialogs
        /// </summary>
        private void RegisterCallbacks()
        {
            Mediator.Register(nameof(ShowWindowMessage), (args) => ShowDialogWindow(args));
            Mediator.Register(nameof(YesNoDialogMessage), (args) => YesNowDialog(args));
        }

        public void ShowDialogWindow(object args)
        {
            if (args is ShowWindowMessage msg)
            {
                if (msg.viewModel is ChangePasswordViewModel)
                {
                    ShowChangePasswordDialog(msg.viewModel as ChangePasswordViewModel);
                }
                else
                {
                    logger.Trace("Showing " + msg.viewModel.GetType().ToString());
                    Window window = null;
                    var vmType = msg.viewModel.GetType().Name.ToString();
                    switch (vmType)
                    {
                        case nameof(ManageVendorsViewModel):
                            window = new ManageVendorsWindow(msg.viewModel as ManageVendorsViewModel);
                            break;
                        case nameof(ItemTypeManagementViewModel):
                            window = new ItemTypeManagementWindow(msg.viewModel as ItemTypeManagementViewModel);
                            break;
                        case nameof(ItemManagementWindow):
                            window = new ItemManagementWindow(msg.viewModel as ItemManagementViewModel);
                            break;
                    }
                    if (window != null)
                    {
                        window.Owner = App.Current.MainWindow; /* this */
                        if (msg.modal)
                            window.ShowDialog();
                        else
                            window.Show();
                    }
                }
            }
        }

        public void YesNowDialog(object args)
        {
            if (args is YesNoDialogMessage msg)
            {
                var result = MessageBox.Show(owner: App.Current.MainWindow, messageBoxText: msg.message, caption: msg.caption, button: MessageBoxButton.YesNo, icon: MessageBoxImage.Question, defaultResult: MessageBoxResult.No, options: MessageBoxOptions.None);
                if (result == MessageBoxResult.Yes)
                    msg.YesAction(msg);
                else
                    msg.NoAction(msg);
            }
        }

        /// <summary>
        /// Create and show dialog allowing user to change password
        /// </summary>
        public void ShowChangePasswordDialog(ChangePasswordViewModel ViewModel)
        {
            logger.Trace(nameof(ShowChangePasswordDialog));
            var setPasswordWindow = new ChangePasswordWindow(ViewModel)
            {
                Owner = App.Current.MainWindow /* this */
            };
            setPasswordWindow.ShowDialog();
        }


        /// <summary>
        /// Logs current user out, closes MainWindow and returns to LogIn window
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void LogOutBtn() /*_Click(object sender, RoutedEventArgs e) */
        {
            //setSelection(ref LogOutBtn, ref LogOutLbl, "AssetManagementItems");

            // log out user
            UserManager.GetUserManager.LogoutUser("Logout from MainWindow.");

            // logging out and in is not expected to be too common, so simply recreate LogInWindow and transfer control to it.
            var newWin = new LogInWindow();
            App.Current.MainWindow = newWin;
            newWin.Show();
            this.Close();
        }

        /*
        public ICommand MainMenuActionCommand
        {
            get
            {
                if (_MainMenuActionCommand == null)
                {
                    _MainMenuActionCommand = new RelayCommand(param => MainMenuAction(param), null);
                }
                return _MainMenuActionCommand;
            }
        }
        private ICommand _MainMenuActionCommand;
        */

        /* Note: we use KeyUp and MouseLeftButtonUp to indicate selection 
         * so we can reselect same item with mouse or keyboard
         */
        private void ListView_SelectItem(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                MainMenuAction((e.Source as ListView).SelectedItem);
            }
        }
        private void ListView_SelectItem(object sender, MouseButtonEventArgs e)
        {
            MainMenuAction((e.Source as ListView).SelectedItem);
        }

        /// <summary>
        /// Route to appropriate window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainMenuAction(object param)
        {
            Window newWin = null;
            SimpleTreeItem selectedItem = param as SimpleTreeItem;
            logger.Info($"MainMenuAction - invoking: { selectedItem?.tag ?? "null" }");
            if (selectedItem?.label != null)
            {
                StatusLabel.Content = selectedItem.label;
            }
            else
            {
                StatusLabel.Content = "";
            }

            switch (selectedItem?.tag)
            {
                // expandable section or other control with no action associated with it
                case null:
                    return; // exit early and don't show internal error message

                // log off
                case "LogOut":
                    LogOutBtn();
                    return;

                // asset management
                case "GenInvMngt":
                    newWin = new GeneralInventoryManagementWindow();
                    break;
                case "DeployRecover":
                    newWin = new DeployRecoverHistoryWindow();
                    break;
                case "DamagedMissing":
                    newWin = new DamagedMissingHistoryWindow();
                    break;
                case "Service":
                    newWin = new ServiceHistoryWindow();
                    break;
                case "Expiration":
                    newWin = new ExpirationWindow();
                    break;

                // System Administration
                case "ManageUsers":
                    newWin = new ManageUsersWindow();
                    break;
                case "Replication":
                    newWin = new ReplicationWindow();
                    break;
                case "ManageVendors":
                    newWin = new ManageVendorsWindow();
                    break;
                case "SiteToEquipMapping":
                    newWin = new SiteToEquipmentUnitMappingWindow();
                    break;
                case "ManageItems":
                    newWin = new ItemManagementWindow();
                    break;
                case "ManageItemTypes":
                    newWin = new ItemTypeManagementWindow();
                    break;
                case "EditReferenceData":
                    newWin = new ReferenceDataWindow();
                    break;

                // reports
                case "ReportSummary":
                    newWin = new ReportWindow("Inventory Report");
                    initializeInventorySummaryReport((ReportWindow)newWin);
                    break;
                case "ReportItemStatus":
                    newWin = new ReportWindow("Item Status Report");
                    initializeItemStatusReport((ReportWindow)newWin);
                    break;
                case "ReportVendorCost":
                    newWin = new ReportWindow("Vendor Cost Report");
                    initializeVendorCostReport((ReportWindow)newWin);
                    break;
                case "ReportWeight":
                    newWin = new ReportWindow("Weight Report");
                    initializeWeightReport((ReportWindow)newWin);
                    break;
                case "ReportExpiration":
                    newWin = new ReportWindow("Expiration Report");
                    initializeExpirationReport((ReportWindow)newWin);
                    break;
                case "ReportService":
                    newWin = new ReportWindow("Service Report");
                    initializeServiceReport((ReportWindow)newWin);
                    break;
                case "ReportDeployment":
                    newWin = new ReportWindow("Deployment Report");
                    initializeDeploymentReport((ReportWindow)newWin);
                    break;
                case "ReportDamaged":
                    newWin = new ReportWindow("Missing/Damaged Report");
                    initializeDamagedOrMissingReport((ReportWindow)newWin);
                    break;

                // view/print labels
                case "Labels":
                    newWin = new ViewPrintLabelsWindow();
                    break;

                    // no default required
            }

            if (newWin == null)
            {
                MessageBox.Show("Internal Error selecting item", "Internal Error:", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                try
                {
                    App.Current.MainWindow = newWin;
                    newWin.Owner = this;
                    newWin.ShowDialog();
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Internal error - {e.Message}");
                    throw;
                }
            }
        }


        /// <summary>
        /// Initialize information for ReportWindow to display report for Inventory Summary
        /// </summary>
        /// <param name="reportWindow"></param>
        private void initializeInventorySummaryReport(ReportWindow reportWindow)
        {
            //reportWindow.reportName = "My Report's name";
            reportWindow.whichReport = WhichReport.InventorySummary;
            reportWindow.reportColumns = new List<ReportColumn>() {
                new ReportColumn("Item Description", "description", ListSortDirection.Ascending),
                new ReportColumn("Quantity", "quantity"),
                new ReportColumn("Location", "location"),
                new ReportColumn("Bin / Module Container", "binOrModule"),
                new ReportColumn("Expiration", "expirationDate"),
            };
        }

        /// <summary>
        /// Initialize information for ReportWindow to display report for Item Status
        /// </summary>
        /// <param name="reportWindow"></param>
        private void initializeItemStatusReport(ReportWindow reportWindow)
        {
            reportWindow.whichReport = WhichReport.ItemStatus;
            reportWindow.reportColumns = new List<ReportColumn>() {
                new ReportColumn("Item Status", "status"),
                new ReportColumn("Item Description", "description", ListSortDirection.Ascending),
                new ReportColumn("Quantity", "quantity"),
                new ReportColumn("Bin / Module Container", "binOrModule"),
                new ReportColumn("Item Number", "itemNumber"),
            };
        }

        /// <summary>
        /// Initialize information for ReportWindow to display report for Vendor cost
        /// </summary>
        /// <param name="reportWindow"></param>
        private void initializeVendorCostReport(ReportWindow reportWindow)
        {
            reportWindow.whichReport = WhichReport.VendorCost;
            reportWindow.reportColumns = new List<ReportColumn>() {
                new ReportColumn("Vendor", "col1"),
                new ReportColumn("Item Name", "description", ListSortDirection.Ascending),
                new ReportColumn("Unit Cost", "weight"),
                new ReportColumn("Quantity", "quantity"),
                new ReportColumn("Total Cost", "weight"),
            };
        }

        /// <summary>
        /// Initialize information for ReportWindow to display report for Item weights
        /// </summary>
        /// <param name="reportWindow"></param>
        private void initializeWeightReport(ReportWindow reportWindow)
        {
            reportWindow.whichReport = WhichReport.Weight;
            reportWindow.reportColumns = new List<ReportColumn>() {
                new ReportColumn("Location", "location"),
                new ReportColumn("Bin", "col1"),
                new ReportColumn("Module", "col2"),
                new ReportColumn("Standalone Item", "description", ListSortDirection.Ascending),
                new ReportColumn("Weight", "weight"),
            };
        }

        /// <summary>
        /// Initialize information for ReportWindow to display report for Item expirations
        /// </summary>
        /// <param name="reportWindow"></param>
        private void initializeExpirationReport(ReportWindow reportWindow)
        {
            reportWindow.whichReport = WhichReport.Expiration;
            reportWindow.reportColumns = new List<ReportColumn>() {
                new ReportColumn("Item Description", "description", ListSortDirection.Ascending),
                new ReportColumn("Quantity", "quantity"),
                new ReportColumn("Location", "location"),
                new ReportColumn("Expiration", "expirationDate"),
            };
        }

        /// <summary>
        /// Initialize information for ReportWindow to display report for Item service
        /// </summary>
        /// <param name="reportWindow"></param>
        private void initializeServiceReport(ReportWindow reportWindow)
        {
            reportWindow.whichReport = WhichReport.Service;
            reportWindow.reportColumns = new List<ReportColumn>() {
                new ReportColumn("Vendor", "col1"),
                new ReportColumn("Item Number", "itemNumber"),
                new ReportColumn("Item Name", "description", ListSortDirection.Ascending),
                new ReportColumn("Service Due", "col1"),
                new ReportColumn("Service Done", "col2"),
            };
        }

        /// <summary>
        /// Initialize information for ReportWindow to display report for Item deployments
        /// </summary>
        /// <param name="reportWindow"></param>
        private void initializeDeploymentReport(ReportWindow reportWindow)
        {
            reportWindow.whichReport = WhichReport.Deployment;
            reportWindow.reportColumns = new List<ReportColumn>() {
                new ReportColumn("Item Number", "itemNumber"),
                new ReportColumn("Item Name", "description", ListSortDirection.Ascending),
                new ReportColumn("Site", "col1"),
                new ReportColumn("Deployed By", "col2"),
                new ReportColumn("Date", "col1"),
                new ReportColumn("Returned By", "col2"),
                new ReportColumn("Date", "col1"),
            };
        }

        /// <summary>
        /// Initialize information for ReportWindow to display report for Item damage or missing
        /// </summary>
        /// <param name="reportWindow"></param>
        private void initializeDamagedOrMissingReport(ReportWindow reportWindow)
        {
            reportWindow.whichReport = WhichReport.DamagedOrMissing;
            reportWindow.reportColumns = new List<ReportColumn>() {
                new ReportColumn("Site Location", "col1"),
                new ReportColumn("Status", "status"),
                new ReportColumn("Discovered", "col2"),
                new ReportColumn("Vendor Name", "col1"),
                new ReportColumn("Bin", "col1"),
                new ReportColumn("Module", "col2"),
                new ReportColumn("Item Number", "itemNumber"),
                new ReportColumn("Name", "description", ListSortDirection.Ascending),
                new ReportColumn("Requested Action", "col1"),
                new ReportColumn("Damage", "col1"),
                new ReportColumn("Event Details", "col1"),
                new ReportColumn("Reported By", "col1"),
                new ReportColumn("Contact Info", "col1"),
            };
        }

    }


    /* used currently for menu */
    /// <summary>
    /// Simple object to hold values required to display choice in TreeView
    /// </summary>
    public class SimpleTreeItem : NotifyPropertyChanged
    {
        public string tag { get; set; }
        public string label { get; set; }
        public string image { get; set; }
        public SimpleTreeItem[] children { get; set; }
        public bool IsSelected { get; set; }
    }


}
