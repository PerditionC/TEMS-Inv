// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using DW.WPFToolkit.Controls;
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.userManager;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public string reportName { get; set; } = "Report";
        public WhichReport whichReport { get; set; }
        public List<ReportColumn> reportColumns { get; set; }

        public ReportWindow() : base()
        {
            InitializeComponent();
            this.DataContext = this;
            filterGroup.DataContext = DataRepository.GetDataRepository;
        }

        public ReportWindow(string reportName) : this()
        {
            this.reportName = reportName;
        }

        public EquipmentUnitType selectedEquipmentUnitType
        {
            get { return _selectedEquipmentUnitType; }
            set { _selectedEquipmentUnitType = value; }  // TODO SetProperty() to Raise change
        }
        private EquipmentUnitType _selectedEquipmentUnitType = null;

        /// <summary>
        /// Indicates if item should be shown in view if it contains filterText
        /// </summary>
        /// <param name="item">object to determine if filter applies to or not</param>
        /// <param name="filterText">string (not null or Empty) to see if is substring in one of item's string properties</param>
        /// <returns></returns>
        protected bool showItemInView(object item, string filterText) // may be overridden if different filtering required
        {
            // test if text from filter [search box] in any string column
            // as soon as found return true, otherwise continue until all columns checked.
            foreach (var column in reportColumns)
            {
                Type itemType = item.GetType();
                PropertyInfo propertyInfo = itemType.GetProperty(column.propName);
                if (typeof(string) == propertyInfo.PropertyType)
                {
                    string colVal = propertyInfo.GetValue(item, null) as string;
                    if (colVal?.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }
            }
            // if filterText is not a substring of any [string] columns then item should not be shown
            return false;
        }


        /// <summary>
        /// Indicate if item is filtered
        /// </summary>
        /// <param name="item">object (row data) to determine if shown or not</param>
        /// <returns>
        /// returns true to show item, either filterText is Empty (or null) or filterText was found in item.  
        /// returns False to omit item, filterText is provided (not Empty or null) and a substring within one of item's properties.
        /// </returns>
        private bool filterView(object item) 
        {
            if (String.IsNullOrEmpty(txtFilter.Text))
                return true;

            return showItemInView(item, txtFilter.Text);
        }

        /// <summary>
        /// Once Window has completed loading, retrieve report data from database and show
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDataReport(reportColumns);
        }

        /// <summary>
        /// Closes report Window and returns to previous (parent) Window
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// When user modifies freeform text filter, automatically update displayed data (refresh view)
        /// </summary>
        /// <param name="sender">unused</param>
        /// <param name="e">unused</param>
        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvReport.ItemsSource).Refresh();
        }


        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        private void sortByColumn(GridViewColumnHeader column, ListSortDirection? sortDirection)
        {
            // remove previous up/down arrow if previously sorted
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lvReport.Items.SortDescriptions.Clear();
            }

            // get property to sort by
            string sortBy = column?.Tag?.ToString();

            // if selected column is not sortable (no sort property to sort by is associated with it) then done
            if (sortBy == null)
            {
                listViewSortCol = null;  // not sorted
                return;
            }

            // determine whether to sort ascending or descending 
            // if explicit sort direction given then use that, otherwise base on 
            // if previously sorted or not & if so what was prior direction (ie toggle direction)
            ListSortDirection newDir;
            if (sortDirection == null)
                newDir = (listViewSortCol == column && listViewSortAdorner.Direction == ListSortDirection.Ascending) ? 
                         ListSortDirection.Descending : ListSortDirection.Ascending;
            else
                newDir = (ListSortDirection)sortDirection;

            // store column sorted by, add up/down arrow to indicate sort direction, & do the sort
            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lvReport.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private void lvColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            sortByColumn(sender as GridViewColumnHeader, null);
        }

        private void sortA2Z(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sort A-Z");
        }
        private void sortZ2A(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sort Z-A");
        }
        private void sortDefault(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sort Default");
        }

        private void print(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Print is not implemented!");
        }
        private void printPreview(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Print Preview is not implemented!");
        }
        private void printPageSetup(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Print page setup is not implemented!");
        }


        private void exportReportAsWps(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export as WPS is not implemented!");
        }
        private void exportReportAsPdf(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export as PDF is not implemented!");
        }
        private void exportReportAsCsv(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export as CSV is not implemented!");
        }
        private void exportReportAsExcel(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Export as xlsx is not implemented!");
        }


        /// <summary>
        /// Add a new labeled column and assign the property to obtain its value from
        /// </summary>
        /// <param name="gridView">the GridView to add the column to</param>
        /// <param name="header">the title label of the column</param>
        /// <param name="propName">the property to use to obtain the value for the column</param>
        /// <param name="defaultSortDirection">report sorted by this column by default (ascending/descending).
        ///                                    Use null if not initially sorted by this column.
        /// </param>
        private void AddColumnToGridView(GridView gridView, string header, string propName, ListSortDirection? defaultSortDirection)
        {
            var gridViewColumnHeader = new GridViewColumnHeader();
            var splitButton = new SplitButton();
            splitButton.Content = header;
            splitButton.Click += sortDefault;
            splitButton.Background = gridViewColumnHeader.Background;
            splitButton.BorderBrush = gridViewColumnHeader.Background; //-V3127
            splitButton.BorderThickness = (Thickness)new ThicknessConverter().ConvertFromInvariantString("0");

            var m = new SplitButtonItem();
            m.Content = "Sort A to Z";
            m.Click += sortA2Z;
            splitButton.Items.Add(m);
            m = new SplitButtonItem();
            m.Content = "Sort Z to A";
            m.Click += sortZ2A;
            splitButton.Items.Add(m);
            m = new SplitButtonItem();
            m.Content = "Filter:";
            m.Click += lvColumnHeader_Click;
            splitButton.Items.Add(m);

            splitButton.Margin = (Thickness)new ThicknessConverter().ConvertFromString("0,0,20,0");
            gridViewColumnHeader.Content = splitButton; // margin added to make room for up/down sort marker
            gridViewColumnHeader.Tag = propName;  // all columns are sortable, so store which field to use when sorting
            gridViewColumnHeader.Click += lvColumnHeader_Click;

            var gridViewColumn = new GridViewColumn();
            gridViewColumn.Header = gridViewColumnHeader;
            gridViewColumn.DisplayMemberBinding = new Binding(propName);
            gridView.Columns.Add(gridViewColumn);

            // is report sorted by this column by default?
            //if (defaultSortDirection != null) sortByColumn(gridViewColumnHeader, defaultSortDirection);
        }

        /// <summary>
        /// Load data from database and dynamically add columns to report.
        /// </summary>
        /// <param name="reportColumns"></param>
        private void LoadDataReport(List<ReportColumn> reportColumns)
        {
            // get the data to bind to report
            lvReport.ItemsSource = DataRepository.GetDataRepository.getReportItems(whichReport, siteLocationId: UserManager.GetUserManager.CurrentUser().currentSite.id.ToString(), equipment:selectedEquipmentUnitType?.name ?? "MMRS");

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvReport.ItemsSource);
            view.Filter = filterView;

            var gridView = lvReport.View as GridView;
            gridView.AllowsColumnReorder = true;

            foreach (var col in reportColumns)
                AddColumnToGridView(gridView, col.header, col.propName, col.defaultSortDirection);
        }

    }

    // draws the up/down arrow on sorted column
    // from http://www.wpf-tutorial.com/listview-control/listview-how-to-column-sorting/
    public class SortAdorner : Adorner
    {
        private static Geometry ascGeometry =
                Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

        private static Geometry descGeometry =
                Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

        public ListSortDirection Direction { get; private set; }

        public SortAdorner(UIElement element, ListSortDirection dir)
                : base(element)
        {
            this.Direction = dir;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            TranslateTransform transform = new TranslateTransform
                    (
                            AdornedElement.RenderSize.Width - 15,
                            (AdornedElement.RenderSize.Height - 5) / 2
                    );
            drawingContext.PushTransform(transform);

            Geometry geometry = ascGeometry;
            if (this.Direction == ListSortDirection.Descending)
                geometry = descGeometry;
            drawingContext.DrawGeometry(Brushes.Black, null, geometry);

            drawingContext.Pop();
        }
    }



    /// <summary>
    /// Details about a specific column in the report
    /// </summary>
    public class ReportColumn
    {
        public string header { get; set; }
        public string propName { get; set; }
        // Note: currently only sort by 1 column at a time, 
        // so only 1 column should have a defaultSortDirection, others should use null
        public ListSortDirection? defaultSortDirection { get; set; }  

        public ReportColumn() { }
        public ReportColumn(string header, string propName, ListSortDirection? defaultSortDirection = null)
        {
            this.propName = propName;
            this.header = header;
            this.defaultSortDirection = defaultSortDirection;
        }
    }

}
