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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DW.WPFToolkit.Helpers;

namespace DW.WPFToolkit.Interactivity
{
    /// <summary>
    /// Brings the feature to controls with a <see cref="System.Windows.Controls.GridViewColumnHeader" /> to have columns with a dynamic width.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <ListView Interactivity:ColumnWidthBehavior.AutoSize="ByContent" />
    /// 
    /// <ListView Interactivity:ColumnWidthBehavior.AutoSize="Proportional">
    ///     <ListView.View>
    ///         <GridView>
    ///             <GridViewColumn Header="Name" DisplayMemberBinding="{Binding Name}" Interactivity:ColumnWidthBehavior.ProportionalWidth="60" />
    ///             <GridViewColumn Header="Size" DisplayMemberBinding="{Binding Size}" Interactivity:ColumnWidthBehavior.ProportionalWidth="30" />
    ///             <GridViewColumn Header="Date" DisplayMemberBinding="{Binding Date}" Interactivity:ColumnWidthBehavior.ProportionalWidth="10" />
    ///         </GridView>
    ///     </ListView.View>
    /// </ListView>
    /// ]]>
    /// </code>
    /// </example>
    public class ColumnWidthBehavior : DependencyObject
    {
        /// <summary>
        /// Gets resize kind for a column.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnWidthBehavior.AutoSize property value for the element.</returns>
        public static ColumnResizeKind GetAutoSize(DependencyObject obj)
        {
            return (ColumnResizeKind)obj.GetValue(AutoSizeProperty);
        }

        /// <summary>
        /// Attaches the resize kind for a column.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnWidthBehavior.AutoSize value.</param>
        public static void SetAutoSize(DependencyObject obj, ColumnResizeKind value)
        {
            obj.SetValue(AutoSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnWidthBehavior.GetAutoSize(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnWidthBehavior.SetAutoSize(DependencyObject, ColumnResizeKind)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty AutoSizeProperty =
            DependencyProperty.RegisterAttached("AutoSize", typeof(ColumnResizeKind), typeof(ColumnWidthBehavior), new UIPropertyMetadata(OnAutoSizeChanged));

        private static void OnAutoSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = GetOrSetBehavior(sender);
            var element = (FrameworkElement)sender;
            behavior._owner = sender;
            element.Loaded += behavior.Element_Loaded;
        }

        /// <summary>
        /// Gets proportional size in percent for a column.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnWidthBehavior.ProportionalWidth property value for the element.</returns>
        public static double GetProportionalWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(ProportionalWidthProperty);
        }

        /// <summary>
        /// Attaches the proportional size in percent for a column.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnWidthBehavior.ProportionalWidth value.</param>
        public static void SetProportionalWidth(DependencyObject obj, double value)
        {
            obj.SetValue(ProportionalWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnWidthBehavior.GetProportionalWidth(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnWidthBehavior.SetProportionalWidth(DependencyObject, double)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ProportionalWidthProperty =
            DependencyProperty.RegisterAttached("ProportionalWidth", typeof(double), typeof(ColumnWidthBehavior), new UIPropertyMetadata(double.NaN));

        /// <summary>
        /// Gets additional space left from a column by calculating the width.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ColumnWidthBehavior.TemplatePaddingWidthFix property value for the element.</returns>
        public static double GetTemplatePaddingWidthFix(DependencyObject obj)
        {
            return (double)obj.GetValue(TemplatePaddingWidthFixProperty);
        }

        /// <summary>
        /// Attaches the additional space left from a column by calculating the width.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ColumnWidthBehavior.TemplatePaddingWidthFix value.</param>
        public static void SetTemplatePaddingWidthFix(DependencyObject obj, double value)
        {
            obj.SetValue(TemplatePaddingWidthFixProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ColumnWidthBehavior.GetTemplatePaddingWidthFix(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ColumnWidthBehavior.SetTemplatePaddingWidthFix(DependencyObject, double)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty TemplatePaddingWidthFixProperty =
            DependencyProperty.RegisterAttached("TemplatePaddingWidthFix", typeof(double), typeof(ColumnWidthBehavior), new UIPropertyMetadata(10.0));

        private static bool GetOriginalWidthIsRemembered(DependencyObject obj)
        {
            return (bool)obj.GetValue(OriginalWidthIsRememberedProperty);
        }

        private static void SetOriginalWidthIsRemembered(DependencyObject obj, bool value)
        {
            obj.SetValue(OriginalWidthIsRememberedProperty, value);
        }

        private static readonly DependencyProperty OriginalWidthIsRememberedProperty =
            DependencyProperty.RegisterAttached("OriginalWidthIsRemembered", typeof(bool), typeof(ColumnWidthBehavior), new UIPropertyMetadata(false));

        private static double GetOriginalWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(OriginalWidthProperty);
        }

        private static void SetOriginalWidth(DependencyObject obj, double value)
        {
            obj.SetValue(OriginalWidthProperty, value);
        }

        private static readonly DependencyProperty OriginalWidthProperty =
            DependencyProperty.RegisterAttached("OriginalWidth", typeof(double), typeof(ColumnWidthBehavior), new UIPropertyMetadata(0.0));

        private static ColumnWidthBehavior GetColumnWidthBehavior(DependencyObject obj)
        {
            return (ColumnWidthBehavior)obj.GetValue(ColumnWidthBehaviorProperty);
        }

        private static void SetColumnWidthBehavior(DependencyObject obj, ColumnWidthBehavior value)
        {
            obj.SetValue(ColumnWidthBehaviorProperty, value);
        }

        private static readonly DependencyProperty ColumnWidthBehaviorProperty =
            DependencyProperty.RegisterAttached("ColumnWidthBehavior", typeof(ColumnWidthBehavior), typeof(ColumnWidthBehavior), new UIPropertyMetadata(null));

        private static ColumnWidthBehavior GetOrSetBehavior(DependencyObject sender)
        {
            var behavior = GetColumnWidthBehavior(sender);
            if (behavior == null)
            {
                behavior = new ColumnWidthBehavior();
                SetColumnWidthBehavior(sender, behavior);
            }
            return behavior;
        }

        private DependencyObject _owner;
        private GridViewColumnCollection _columns;
        private List<GridViewColumn> _toResizeColumns;
        private ScrollContentPresenter _scrollContentPresenter;
        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            Resize();
        }

        private void ScrollContentPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resize();
        }

        private void Resize()
        {
            var kind = GetAutoSize(_owner);
            if (kind != ColumnResizeKind.NoResize)
            {
                if (_columns == null)
                    TryReadColumns();

                if (_scrollContentPresenter == null)
                {
                    _scrollContentPresenter = FindPresenter();
                    if (_scrollContentPresenter != null)
                        _scrollContentPresenter.SizeChanged += new SizeChangedEventHandler(ScrollContentPresenter_SizeChanged);
                }

                if (_columns != null &&
                    _scrollContentPresenter != null)
                {
                    switch (kind)
                    {
                        case ColumnResizeKind.ByContent:
                            ResizeByContent();
                            break;
                        case ColumnResizeKind.ByControl:
                            ResizeByControl();
                            break;
                        case ColumnResizeKind.Proportional:
                            ResizeProportional();
                            break;
                    }
                }
            }
        }

        private void TryReadColumns()
        {
            var presenter = VisualTreeAssist.FindChild<GridViewHeaderRowPresenter>(_owner);
            if (presenter != null)
                _columns = presenter.Columns;

            if (_columns != null)
            {
                SetOriginalWidths();
                _columns.CollectionChanged += (a, b) => Reset();
            }
        }

        private void Reset()
        {
            if (_toResizeColumns != null)
            {
                foreach (var column in _toResizeColumns)
                    if (GetOriginalWidthIsRemembered(column))
                        column.Width = GetOriginalWidth(column);
                _toResizeColumns.Clear();
                _toResizeColumns = null;
            }

            Resize();
        }

        private void SetOriginalWidths()
        {
            foreach (var column in _columns)
            {
                SetOriginalWidth(column, column.Width);
                SetOriginalWidthIsRemembered(column, true);
            }
        }

        private ScrollContentPresenter FindPresenter()
        {
            var internalScrollViewer = VisualTreeAssist.FindChild<ScrollViewer>(_owner);
            return VisualTreeAssist.FindChild<ScrollContentPresenter>(internalScrollViewer);
        }

        #region ByContent
        private void ResizeByContent()
        {
            if (!_changedEventCatched)
            {
                _changedEventCatched = true;

                var itemsControl = (ItemsControl)_owner;
                var coll = CollectionViewSource.GetDefaultView(itemsControl.Items);
                if (coll != null)
                {
                    if (coll is INotifyCollectionChanged)
                        ((INotifyCollectionChanged)coll).CollectionChanged += (a, b) => { ResizeByContent(); };
                }
            }

            foreach (var column in _columns)
            {
                if (double.IsNaN(column.Width))
                {
                    column.Width = 0;
                    column.Width = double.NaN;
                }
            }
        }
        private bool _changedEventCatched;
        #endregion ByContent

        #region ByControl
        private void ResizeByControl()
        {
            var maxWidth = _scrollContentPresenter.ActualWidth;
            maxWidth -= GetTemplatePaddingWidthFix(_owner);

            double leftDistance = 0;
            if (maxWidth > 0)
            {
                leftDistance = CalculateFixedDistance(leftDistance);

                foreach (var column in _toResizeColumns)
                {
                    var newWidth = (maxWidth - leftDistance) / _toResizeColumns.Count;
                    if (newWidth >= 0)
                        column.Width = newWidth;
                }
            }
        }
        #endregion ByControl

        #region Proportional
        private void ResizeProportional()
        {
            var maxWidth = _scrollContentPresenter.ActualWidth;
            maxWidth -= GetTemplatePaddingWidthFix(_owner);

            double leftDistance = 0;
            if (maxWidth > 0)
            {
                leftDistance = CalculateFixedDistance(leftDistance);

                foreach (var column in _toResizeColumns)
                {
                    var proportion = GetProportionalWidth(column);
                    if (!double.IsNaN(proportion))
                    {
                        var newWidth = (maxWidth / 100.0) * proportion;
                        if (newWidth >= 0)
                            column.Width = newWidth;
                    }
                }
            }
        }
        #endregion Proportional

        private double CalculateFixedDistance(double leftDistance)
        {
            if (_toResizeColumns == null)
            {
                _toResizeColumns = new List<GridViewColumn>();
                foreach (var column in _columns)
                {
                    if (column.Width >= 0)
                        leftDistance += column.ActualWidth;
                    else
                        _toResizeColumns.Add(column);
                }
            }
            else
            {
                foreach (var column in _columns)
                {
                    if (!_toResizeColumns.Contains(column))
                        leftDistance += column.ActualWidth;
                }
            }
            return leftDistance;
        }
    }
}
