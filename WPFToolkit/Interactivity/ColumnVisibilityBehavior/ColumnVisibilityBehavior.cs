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

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using DW.WPFToolkit.Helpers;

namespace DW.WPFToolkit.Interactivity
{
    /// <summary>
    /// Brings the feature to controls with a <see cref="System.Windows.Controls.GridViewColumnHeader" /> to show or hide columns dynamically.
    /// </summary>
    /// <example>
    /// <code lang="csharp">
    /// <![CDATA[
    /// public class MainViewModel : ObservableObject
    /// {
    ///     public MainViewModel()
    ///     {
    ///         VisibleColumnNames = new EnhancedObservableCollection<string> { "Name", "Size" };
    ///     }
    /// 
    ///     public EnhancedObservableCollection<string> VisibleColumnNames { get; private set; }
    /// }
    /// ]]>
    /// </code>
    /// 
    /// <code lang="XAML"> 
    /// <![CDATA[
    /// <ListView Interactivity:ColumnVisibilityBehavior.VisibleColumns="{Binding VisibleColumnNames}">
    ///     <ListView.View>
    ///         <GridView>
    ///             <GridViewColumn Header="Name" DisplayMemberBinding="{Binding Name}" Interactivity:ColumnVisibilityBehavior.Name="Name" />
    ///             <GridViewColumn Header="Size" DisplayMemberBinding="{Binding Size}" Interactivity:ColumnVisibilityBehavior.Name="Size" />
    ///             <GridViewColumn Header="Date" DisplayMemberBinding="{Binding Date}" Interactivity:ColumnVisibilityBehavior.Name="Date" />
    ///         </GridView>
    ///     </ListView.View>
    /// </ListView>
    /// ]]>
    /// </code>
    /// </example>
    public class ColumnVisibilityBehavior : DependencyObject
    {
        private ColumnVisibilityBehavior()
        {
            _filteredColumns = new List<GridViewColumn>();
        }

        /// <summary>
        /// Gets a list of visible columns by their name.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnVisibilityBehavior.VisibleColumns property value for the element.</returns>
        public static IList GetVisibleColumns(DependencyObject obj)
        {
            return (IList)obj.GetValue(VisibleColumnsProperty);
        }

        /// <summary>
        /// Attaches a list of visible columns by their name.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnVisibilityBehavior.VisibleColumns value.</param>
        public static void SetVisibleColumns(DependencyObject obj, IList value)
        {
            obj.SetValue(VisibleColumnsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnVisibilityBehavior.GetVisibleColumns(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnVisibilityBehavior.SetVisibleColumns(DependencyObject, IList)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty VisibleColumnsProperty =
            DependencyProperty.RegisterAttached("VisibleColumns", typeof(IList), typeof(ColumnVisibilityBehavior), new UIPropertyMetadata(OnVisibleColumnsChanged));

        private static void OnVisibleColumnsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            element.Loaded += Eement_Loaded;
        }

        /// <summary>
        /// Gets the name of the element.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnVisibilityBehavior.Name property value for the element.</returns>
        public static object GetName(DependencyObject obj)
        {
            return (object)obj.GetValue(NameProperty);
        }

        /// <summary>
        /// Attaches the name.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnVisibilityBehavior.Name value.</param>
        public static void SetName(DependencyObject obj, object value)
        {
            obj.SetValue(NameProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnVisibilityBehavior.GetName(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnVisibilityBehavior.SetName(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.RegisterAttached("Name", typeof(object), typeof(ColumnVisibilityBehavior), new UIPropertyMetadata(null));

        private static ColumnVisibilityBehavior GetColumnVisibilityBehavior(DependencyObject obj)
        {
            return (ColumnVisibilityBehavior)obj.GetValue(ColumnVisibilityBehaviorProperty);
        }

        private static void SetColumnVisibilityBehavior(DependencyObject obj, ColumnVisibilityBehavior value)
        {
            obj.SetValue(ColumnVisibilityBehaviorProperty, value);
        }

        private static readonly DependencyProperty ColumnVisibilityBehaviorProperty =
            DependencyProperty.RegisterAttached("ColumnVisibilityBehavior", typeof(ColumnVisibilityBehavior), typeof(ColumnVisibilityBehavior), new UIPropertyMetadata(null));

        private static ColumnVisibilityBehavior GetOrSetBehavior(DependencyObject sender)
        {
            ColumnVisibilityBehavior behavior = GetColumnVisibilityBehavior(sender);
            if (behavior == null)
            {
                behavior = new ColumnVisibilityBehavior();
                SetColumnVisibilityBehavior(sender, behavior);
            }
            return behavior;
        }

        private static int GetPosition(DependencyObject obj)
        {
            return (int)obj.GetValue(PositionProperty);
        }

        private static void SetPosition(DependencyObject obj, int value)
        {
            obj.SetValue(PositionProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnVisibilityBehavior.GetPosition(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnVisibilityBehavior.SetPosition(DependencyObject, int)" /> attached property.
        /// </summary>
        private static readonly DependencyProperty PositionProperty =
            DependencyProperty.RegisterAttached("Position", typeof(int), typeof(ColumnVisibilityBehavior), new UIPropertyMetadata(0));

        private static void Eement_Loaded(object sender, RoutedEventArgs e)
        {
            var element = (DependencyObject)sender;
            var behavior = GetOrSetBehavior(element);
            if (!behavior._isCatchedAlready)
            {
                var presenter = VisualTreeAssist.FindChild<GridViewHeaderRowPresenter>(element);
                if (presenter != null)
                    behavior.Handle(element, presenter.Columns);
            }
        }

        private bool _isCatchedAlready;
        private void Handle(DependencyObject sender, GridViewColumnCollection columns)
        {
            _isCatchedAlready = true;

            _owner = sender;
            _columns = columns;
            NumerizeColumns();

            var visibleColumns = GetVisibleColumns(sender);
            var notifyCollectionChanged = visibleColumns as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
                notifyCollectionChanged.CollectionChanged += (a, b) => Refresh();

            Refresh();
        }

        private void NumerizeColumns()
        {
            for (int i = 0; i < _columns.Count; ++i)
                SetPosition(_columns[i], i);
        }

        private DependencyObject _owner;
        private GridViewColumnCollection _columns;
        private readonly List<GridViewColumn> _filteredColumns;

        private void Refresh()
        {
            ResetOldFiltered();
            FilterOut();
        }

        private void ResetOldFiltered()
        {
            while (_columns.Count > 0)
            {
                _filteredColumns.Add(_columns[0]);
                _columns.RemoveAt(0);
            }
            _filteredColumns.Sort(SortByPosition);

            foreach (var column in _filteredColumns)
                _columns.Add(column);
            _filteredColumns.Clear();
        }

        private static int SortByPosition(GridViewColumn first, GridViewColumn second)
        {
            return GetPosition(first).CompareTo(GetPosition(second));
        }

        private void FilterOut()
        {
            var visibleColumns = GetVisibleColumns(_owner);
            for (int i = 0; i < _columns.Count; ++i)
            {
                var name = GetName(_columns[i]);
                if (name != null)
                {
                    if (!visibleColumns.Contains(name))
                    {
                        _filteredColumns.Add(_columns[i]);
                        _columns.RemoveAt(i);
                        --i;
                    }
                }
            }
        }
    }
}
