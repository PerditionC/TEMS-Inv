using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using NLog;
using TEMS.InventoryModel.command.action;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS_Inventory.views;

namespace TEMS_Inventory
{
    public static class ViewModelToWindowMapper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static Dictionary<string, Window> views;

        public static Window GetWindow(string view)
        {
            // needed view models for Search Detail Windows, i.e. windows with search pane, results tree pane, and details pane
            DetailsViewModelBase detailsPaneVM;
            OnSelectionChangedCommand onSelectionChangedCommand;
            SearchResultViewModel searchResultViewModel;
            SearchFilterOptions searchFilter;
            SearchFilterOptionsViewModel searchFilterOptionsViewModel;
            SearchDetailWindowViewModel winVM;

            Window win = null;
            switch (view)
            {
                // asset management
                case "GenInvMngt":
                    searchFilter = new SearchFilterOptions();
                    searchFilter.Initialize();

                    detailsPaneVM = new GeneralInventoryManagementViewModel();
                    onSelectionChangedCommand = new UpdateDetailsGeneralInventoryManagementCommand(detailsPaneVM);
                    searchResultViewModel = new SearchResultViewModel(onSelectionChangedCommand);
                    searchFilterOptionsViewModel = new SearchFilterOptionsViewModel(searchFilter, QueryResultEntitySelector.ItemInstance, searchResultViewModel);
                    winVM = new SearchDetailWindowViewModel(searchFilterOptionsViewModel, searchResultViewModel, detailsPaneVM);

                    win = new GeneralInventoryManagementWindow(winVM);
                    break;
                case "DeployRecoverDetails":
                    win = new DeployRecoverDetailsWindow(new DetailsDeployRecoverViewModel(null));
                    break;
                case "DeployRecover":
                    searchFilter = new SearchFilterOptions();
                    searchFilter.Initialize();

                    detailsPaneVM = new HistoryDeployRecoverViewModel();
                    onSelectionChangedCommand = new UpdateDetailsHistoryDeployRecoverCommand(detailsPaneVM);
                    searchResultViewModel = new SearchResultViewModel(onSelectionChangedCommand);
                    searchFilterOptionsViewModel = new SearchFilterOptionsViewModel(searchFilter, QueryResultEntitySelector.ItemInstance, searchResultViewModel);
                    winVM = new SearchDetailWindowViewModel(searchFilterOptionsViewModel, searchResultViewModel, detailsPaneVM);

                    win = new DeployRecoverHistoryWindow(winVM);
                    break;
                case "DamagedMissingDetails":
                    win = new DamagedMissingDetailsWindow(new DetailsDamagedMissingViewModel(null));
                    break;
                case "DamagedMissing":
                    searchFilter = new SearchFilterOptions();
                    searchFilter.Initialize();

                    detailsPaneVM = new HistoryDamagedMissingViewModel();
                    onSelectionChangedCommand = new UpdateDetailsHistoryDamagedMissingCommand(detailsPaneVM);
                    searchResultViewModel = new SearchResultViewModel(onSelectionChangedCommand);
                    searchFilterOptionsViewModel = new SearchFilterOptionsViewModel(searchFilter, QueryResultEntitySelector.ItemInstance, searchResultViewModel);
                    winVM = new SearchDetailWindowViewModel(searchFilterOptionsViewModel, searchResultViewModel, detailsPaneVM);

                    win = new DamagedMissingHistoryWindow(winVM);
                    break;
                case "ServiceDetails":
                    win = new ServiceDetailsWindow(new DetailsServiceViewModel(null));
                    break;
                case "Service":
                    searchFilter = new SearchFilterOptions();
                    searchFilter.Initialize();

                    detailsPaneVM = new HistoryServiceViewModel();
                    onSelectionChangedCommand = new UpdateDetailsHistoryServiceCommand(detailsPaneVM);
                    searchResultViewModel = new SearchResultViewModel(onSelectionChangedCommand);
                    searchFilterOptionsViewModel = new SearchFilterOptionsViewModel(searchFilter, QueryResultEntitySelector.ItemInstance, searchResultViewModel);
                    winVM = new SearchDetailWindowViewModel(searchFilterOptionsViewModel, searchResultViewModel, detailsPaneVM);

                    win = new ServiceHistoryWindow(winVM);
                    break;
                case "ExpirationDetails":
                    win = new ExpirationWindow(new DetailsExpirationReplaceViewModel(null));
                    break;
                case "Expiration":
                    searchFilter = new SearchFilterOptions();
                    searchFilter.Initialize();

                    detailsPaneVM = new HistoryExpirationReplaceViewModel();
                    onSelectionChangedCommand = new UpdateDetailsHistoryExpirationReplaceCommand(detailsPaneVM);
                    searchResultViewModel = new SearchResultViewModel(onSelectionChangedCommand);
                    searchFilterOptionsViewModel = new SearchFilterOptionsViewModel(searchFilter, QueryResultEntitySelector.ItemInstance, searchResultViewModel);
                    winVM = new SearchDetailWindowViewModel(searchFilterOptionsViewModel, searchResultViewModel, detailsPaneVM);

                    win = new ExpirationHistoryWindow(winVM);
                    break;

                // System Administration
                case "ManageUsers":
                    win = new ManageUsersWindow();
                    break;
                case "Replication":
                    win = new ReplicationWindow();
                    break;
                case "ManageVendors":
                    win = new ManageVendorsWindow(null as ManageVendorsViewModel);
                    break;
                case "SiteToEquipMapping":
                    win = new SiteToEquipmentUnitMappingWindow();
                    break;
                case "ManageItemInstances":
                    searchFilter = new SearchFilterOptions();
                    searchFilter.Initialize();

                    detailsPaneVM = new ItemInstanceManagementViewModel();
                    onSelectionChangedCommand = new UpdateDetailsItemManagementCommand(detailsPaneVM, typeof(ItemInstance));
                    searchResultViewModel = new SearchResultViewModel(onSelectionChangedCommand);
                    searchFilterOptionsViewModel = new SearchFilterOptionsViewModel(searchFilter, QueryResultEntitySelector.ItemInstance, searchResultViewModel);
                    winVM = new SearchDetailWindowViewModel(searchFilterOptionsViewModel, searchResultViewModel, detailsPaneVM);

                    win = new ItemManagementWindow(winVM);
                    break;
                case "ManageItems":
                    searchFilter = new SearchFilterOptions();
                    searchFilter.Initialize();

                    detailsPaneVM = new ItemManagementViewModel();
                    onSelectionChangedCommand = new UpdateDetailsItemManagementCommand(detailsPaneVM, typeof(Item));
                    searchResultViewModel = new SearchResultViewModel(onSelectionChangedCommand);
                    searchFilterOptionsViewModel = new SearchFilterOptionsViewModel(searchFilter, QueryResultEntitySelector.Item, searchResultViewModel);
                    winVM = new SearchDetailWindowViewModel(searchFilterOptionsViewModel, searchResultViewModel, detailsPaneVM);

                    win = new ItemManagementWindow(winVM);
                    break;
                case "ManageItemTypes":
                    searchFilter = new SearchFilterOptions();
                    searchFilter.Initialize();

                    detailsPaneVM = new ItemTypeManagementViewModel();
                    onSelectionChangedCommand = new UpdateDetailsItemManagementCommand(detailsPaneVM, typeof(ItemType));
                    searchResultViewModel = new SearchResultViewModel(onSelectionChangedCommand);
                    searchFilterOptionsViewModel = new SearchFilterOptionsViewModel(searchFilter, QueryResultEntitySelector.ItemType, searchResultViewModel);
                    winVM = new SearchDetailWindowViewModel(searchFilterOptionsViewModel, searchResultViewModel, detailsPaneVM);

                    win = new ItemTypeManagementWindow(winVM);
                    break;
                case "EditReferenceData":
                    win = new ReferenceDataWindow();
                    break;

                // reports - see MainWindow.xaml.cs
                case "ReportSummary":
                    break;
                case "ReportItemStatus":
                    break;
                case "ReportVendorCost":
                    break;
                case "ReportWeight":
                    break;
                case "ReportExpiration":
                    break;
                case "ReportService":
                    break;
                case "ReportDeployment":
                    break;
                case "ReportDamaged":
                    break;

                // view/print labels
                case "Labels":
                    win = new ViewPrintLabelsWindow();
                    break;

                default:
                    // nothing to do
                    break;
            }

            logger.Debug($"Invoking window {view}");
            return win;
        }
    }
}
