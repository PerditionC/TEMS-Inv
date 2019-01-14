#region License
/*
The MIT License (MIT)

Copyright (c) 2009-2016 David Wendland

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE
*/
#endregion License

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DW.WPFToolkit.Helpers;

namespace DW.WPFToolkit.Interactivity
{
    /// <summary>
    /// Brings the feature to UI elements with a <see cref="System.Windows.Controls.GridViewColumnHeader" /> to have a sorting by clicking the corresponding header.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <DataTemplate x:Key="HeaderArrowUp">
    ///     <Grid>
    ///         <TextBlock Text="{Binding}" VerticalAlignment="Center" HorizontalAlignment="Center" />
    ///         <Path StrokeThickness="0" SnapsToDevicePixels="True" Data="M 0,4 L 4,0 L 8,4 L 0,4"
    ///               Margin="0,-2,0,0" VerticalAlignment="Top" HorizontalAlignment="Center">
    ///             <Path.Fill>
    ///                 <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
    ///                     <GradientStop Color="#FF3C5E72" Offset="0"/>
    ///                     <GradientStop Color="#FFC3E4F5" Offset="1"/>
    ///                 </LinearGradientBrush>
    ///             </Path.Fill>
    ///         </Path>
    ///     </Grid>
    /// </DataTemplate>
    /// <DataTemplate x:Key="HeaderArrowDown">
    ///     <Grid>
    ///         <TextBlock Text="{Binding}" VerticalAlignment="Center" HorizontalAlignment="Center" />
    ///         <Path StrokeThickness="0" SnapsToDevicePixels="True" Data="M 0,0 L 4,4 L 8,0 L 0,0"
    ///               Margin="0,-2,0,0" VerticalAlignment="Top" HorizontalAlignment="Center">
    ///             <Path.Fill>
    ///                 <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
    ///                     <GradientStop Color="#FF3C5E72" Offset="0"/>
    ///                     <GradientStop Color="#FFC4E3F4" Offset="1"/>
    ///                 </LinearGradientBrush>
    ///             </Path.Fill>
    ///         </Path>
    ///     </Grid>
    /// </DataTemplate>
    /// <DataTemplate x:Key="HeaderTransparent">
    ///     <Grid>
    ///         <TextBlock Text="{Binding}" VerticalAlignment="Center" HorizontalAlignment="Center" />
    ///     </Grid>
    /// </DataTemplate>
    /// 
    /// <ListView Interactivity:ColumnSortBehavior.AllowColumnSortings="True"
    ///           Interactivity:ColumnSortBehavior.AscendingSortHeaderTemplate="{StaticResource HeaderArrowUp}"
    ///           Interactivity:ColumnSortBehavior.DescendingSortHeaderTemplate="{StaticResource HeaderArrowDown}"
    ///           Interactivity:ColumnSortBehavior.NeutralHeaderTemplate="{StaticResource HeaderTransparent}">
    ///     <ListView.View>
    ///         <GridView>
    ///             <GridViewColumn Header="Name" Interactivity:ColumnSortBehavior.IsDefaultSortColumn="True" Interactivity:ColumnSortBehavior.SortPropertyName="Name" />
    ///             <GridViewColumn Header="Size" Interactivity:ColumnSortBehavior.SortPropertyName="Size" />
    ///             <GridViewColumn Header="Date" Interactivity:ColumnSortBehavior.SortPropertyName="Date" />
    ///         </GridView>
    ///     </ListView.View>
    /// </ListView>
    /// ]]>
    /// </code>
    /// </example>
    public class ColumnSortBehavior : DependencyObject
    {
        /// <summary>
        /// Gets the header template to be used for sorting if the column is not used for sort actually.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnSortBehavior.NeutralHeaderTemplate property value for the element.</returns>
        public static DataTemplate GetNeutralHeaderTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(NeutralHeaderTemplateProperty);
        }

        /// <summary>
        /// Attaches the header template to be used for sorting if the column is not used for sort actually.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnSortBehavior.NeutralHeaderTemplate value.</param>
        public static void SetNeutralHeaderTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(NeutralHeaderTemplateProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.GetNeutralHeaderTemplate(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.SetNeutralHeaderTemplate(DependencyObject, DataTemplate)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty NeutralHeaderTemplateProperty =
            DependencyProperty.RegisterAttached("NeutralHeaderTemplate", typeof(DataTemplate), typeof(ColumnSortBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the header template to be used for sorting if the column is used for sort ascending actually.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnSortBehavior.AscendingSortHeaderTemplate property value for the element.</returns>
        public static DataTemplate GetAscendingSortHeaderTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(AscendingSortHeaderTemplateProperty);
        }

        /// <summary>
        /// Attaches the header template to be used for sorting if the column is used for sort ascending actually.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnSortBehavior.AscendingSortHeaderTemplate value.</param>
        public static void SetAscendingSortHeaderTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(AscendingSortHeaderTemplateProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.GetAscendingSortHeaderTemplate(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.SetAscendingSortHeaderTemplate(DependencyObject, DataTemplate)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty AscendingSortHeaderTemplateProperty =
            DependencyProperty.RegisterAttached("AscendingSortHeaderTemplate", typeof(DataTemplate), typeof(ColumnSortBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the header template to be used for sorting if the column is used for sort descending actually.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnSortBehavior.DescendingSortHeaderTemplate property value for the element.</returns>
        public static DataTemplate GetDescendingSortHeaderTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(DescendingSortHeaderTemplateProperty);
        }

        /// <summary>
        /// Attaches the header template to be used for sorting if the column is used for sort descending actually.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnSortBehavior.DescendingSortHeaderTemplate value.</param>
        public static void SetDescendingSortHeaderTemplate(DependencyObject obj, DataTemplate value)
        {
            obj.SetValue(DescendingSortHeaderTemplateProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.GetDescendingSortHeaderTemplate(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.SetDescendingSortHeaderTemplate(DependencyObject, DataTemplate)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty DescendingSortHeaderTemplateProperty =
            DependencyProperty.RegisterAttached("DescendingSortHeaderTemplate", typeof(DataTemplate), typeof(ColumnSortBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets the value that indicates if sorting is allowed or not.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnSortBehavior.AllowColumnSortings property value for the element.</returns>
        public static bool GetAllowColumnSortings(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowColumnSortingsProperty);
        }

        /// <summary>
        /// Attaches the value if sorting is allowed or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnSortBehavior.AllowColumnSortings value.</param>
        public static void SetAllowColumnSortings(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowColumnSortingsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.GetAllowColumnSortings(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.SetAllowColumnSortings(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty AllowColumnSortingsProperty =
            DependencyProperty.RegisterAttached("AllowColumnSortings", typeof(bool), typeof(ColumnSortBehavior), new UIPropertyMetadata(OnAllowColumnSortingsChanged));

        private static void OnAllowColumnSortingsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            element.Loaded += Eement_Loaded;
        }

        /// <summary>
        /// Gets a value that indicates if a column is defined as default sort column or not.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnSortBehavior.IsDefaultSortColumn property value for the element.</returns>
        public static bool GetIsDefaultSortColumn(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDefaultSortColumnProperty);
        }

        /// <summary>
        /// Attaches a value that indicates if a column is defined as default sort column or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnSortBehavior.IsDefaultSortColumn value.</param>
        public static void SetIsDefaultSortColumn(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDefaultSortColumnProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.GetIsDefaultSortColumn(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.SetIsDefaultSortColumn(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty IsDefaultSortColumnProperty =
            DependencyProperty.RegisterAttached("IsDefaultSortColumn", typeof(bool), typeof(ColumnSortBehavior));

        /// <summary>
        /// Gets the property name to be used for sorting.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnSortBehavior.SortPropertyName property value for the element.</returns>
        public static string GetSortPropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(SortPropertyNameProperty);
        }

        /// <summary>
        /// Attaches the property name to be used for sorting.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnSortBehavior.SortPropertyName value.</param>
        public static void SetSortPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(SortPropertyNameProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.GetSortPropertyName(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.SetSortPropertyName(DependencyObject, string)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty SortPropertyNameProperty =
            DependencyProperty.RegisterAttached("SortPropertyName", typeof(string), typeof(ColumnSortBehavior), new UIPropertyMetadata(null));

        private static ColumnSortBehavior GetColumnSortingBehavior(DependencyObject obj)
        {
            return (ColumnSortBehavior)obj.GetValue(ColumnSortingBehaviorProperty);
        }

        private static void SetColumnSortingBehavior(DependencyObject obj, ColumnSortBehavior value)
        {
            obj.SetValue(ColumnSortingBehaviorProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.GetColumnSortingBehavior(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnSortBehavior.SetColumnSortingBehavior(DependencyObject, ColumnSortBehavior)" /> attached property.
        /// </summary>
        private static readonly DependencyProperty ColumnSortingBehaviorProperty =
            DependencyProperty.RegisterAttached("ColumnSortingBehavior", typeof(ColumnSortBehavior), typeof(ColumnSortBehavior), new UIPropertyMetadata(null));

        private static ColumnSortBehavior GetOrSetBehavior(DependencyObject sender)
        {
            var behavior = GetColumnSortingBehavior(sender);
            if (behavior == null)
            {
                behavior = new ColumnSortBehavior();
                SetColumnSortingBehavior(sender, behavior);
            }
            return behavior;
        }

        private static void Eement_Loaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            if (GetAllowColumnSortings(element))
            {
                if (GetColumnSortingBehavior(element) != null)
                    return;

                var behavior = GetOrSetBehavior(element);
                element.AddHandler(GridViewColumnHeader.ClickEvent, new RoutedEventHandler(behavior.GridViewColumnHeaderClickedHandler));
                behavior._owner = (ItemsControl)element;
                behavior.GetColumns(element);
                behavior.RunDefaultSort();
            }
        }

        private void GetColumns(DependencyObject sender)
        {
            var behavior = GetOrSetBehavior(sender);
            var presenter = VisualTreeAssist.FindChild<GridViewHeaderRowPresenter>(_owner);
            if (presenter != null)
                _columns = presenter.Columns;
            
            if (_columns != null)
            {
                foreach (var column in _columns)
                    column.HeaderTemplate = GetNeutralHeaderTemplate(sender);
            }
        }

        private bool _isSorting;
        private ItemsControl _owner;
        private GridViewColumnCollection _columns;
        private GridViewColumn _lastSortedColumn;
        private ListSortDirection _lastDirection;

        private void RunDefaultSort()
        {
            if (_columns != null)
            {
                GridViewColumn sortableGridViewColumn = null;
                foreach (var column in _columns)
                {
                    if (GetIsDefaultSortColumn(column))
                    {
                        sortableGridViewColumn = column;
                        break;
                    }
                }
                if (sortableGridViewColumn != null)
                {
                    _lastSortedColumn = sortableGridViewColumn;
                    Sort(GetSortPropertyName(sortableGridViewColumn), ListSortDirection.Ascending);
                    sortableGridViewColumn.HeaderTemplate = GetAscendingSortHeaderTemplate(_owner);
                }
            }
        }

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked != null &&
                headerClicked.Role != GridViewColumnHeaderRole.Padding)
            {
                var clickedColumn = headerClicked.Column;
                string toSortPropertyName = GetSortPropertyName(clickedColumn);
                if (clickedColumn != null &&
                    !String.IsNullOrEmpty(toSortPropertyName))
                {
                    var direction = ListSortDirection.Ascending;
                    bool isNewSortColumn = IsNewSortColumn(toSortPropertyName);
                    if (!isNewSortColumn)
                        direction = _lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

                    Sort(toSortPropertyName, direction);

                    clickedColumn.HeaderTemplate = direction == ListSortDirection.Ascending ? GetAscendingSortHeaderTemplate(_owner) : GetDescendingSortHeaderTemplate(_owner);

                    ResetLastTemplate(isNewSortColumn);
                    _lastSortedColumn = clickedColumn;
                }
            }
        }

        private bool IsNewSortColumn(string toSortPropertyName)
        {
            if (_lastSortedColumn == null)
                return true;
            var lastSortPropertyName = GetSortPropertyName(_lastSortedColumn);
            return string.IsNullOrEmpty(toSortPropertyName) ||
                   !string.Equals(lastSortPropertyName, toSortPropertyName, StringComparison.InvariantCultureIgnoreCase);
        }

        private void ResetLastTemplate(bool isNewSortColumn)
        {
            if (isNewSortColumn &&
                _lastSortedColumn != null)
                _lastSortedColumn.HeaderTemplate = GetNeutralHeaderTemplate(_owner);
        }

        private void Resort()
        {
            if (!_isSorting)
            {
                _isSorting = true;
                if (_lastSortedColumn != null &&
                    !string.IsNullOrEmpty(GetSortPropertyName(_lastSortedColumn)))
                    Sort(GetSortPropertyName(_lastSortedColumn), _lastDirection);
                _isSorting = false;
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            _lastDirection = direction;
            var dataView = CollectionViewSource.GetDefaultView(_owner.Items);
            if (dataView != null)
            {
                dataView.SortDescriptions.Clear();
                dataView.SortDescriptions.Add(new SortDescription(sortBy, direction));
                dataView.Refresh();
            }
        }
    }
}
