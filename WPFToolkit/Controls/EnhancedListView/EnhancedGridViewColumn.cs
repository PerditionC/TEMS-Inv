// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

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

using System.Windows;
using System.Windows.Controls;
using DW.WPFToolkit.Interactivity;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Enhances the <see cref="System.Windows.Controls.GridViewColumn" /> by sort information and name.
    /// </summary>
    public class EnhancedGridViewColumn : GridViewColumn
    {
        /// <summary>
        /// Gets or sets the name of the current column. This will be used by the <see cref="DW.WPFToolkit.Controls.EnhancedListView.VisibleColumns" /> collection.
        /// </summary>
        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedGridViewColumn.Name" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(EnhancedGridViewColumn), new UIPropertyMetadata(OnNameChanged));

        private static void OnNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColumnVisibilityBehavior.SetName(sender, (string)e.NewValue);
        }

        /// <summary>
        /// Gets or sets a value which indicates if the current column is the default sort when the <see cref="DW.WPFToolkit.Controls.EnhancedListView" /> is loaded.
        /// </summary>
        public bool IsDefaultSortColumn
        {
            get { return (bool)GetValue(IsDefaultSortColumnProperty); }
            set { SetValue(IsDefaultSortColumnProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedGridViewColumn.IsDefaultSortColumn" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDefaultSortColumnProperty =
            DependencyProperty.Register("IsDefaultSortColumn", typeof(bool), typeof(EnhancedGridViewColumn), new UIPropertyMetadata(OnIsDefaultSortColumnChanged));

        private static void OnIsDefaultSortColumnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColumnSortBehavior.SetIsDefaultSortColumn(sender, (bool)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the property name which will be used for sorting by the current column.
        /// </summary>
        public string SortPropertyName
        {
            get { return (string)GetValue(SortPropertyNameProperty); }
            set { SetValue(SortPropertyNameProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedGridViewColumn.SortPropertyName" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SortPropertyNameProperty =
            DependencyProperty.Register("SortPropertyName", typeof(string), typeof(EnhancedGridViewColumn), new UIPropertyMetadata(OnSortPropertyNameChanged));

        private static void OnSortPropertyNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColumnSortBehavior.SetSortPropertyName(sender, (string)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the proportional width in percent to be used for this column if <see cref="DW.WPFToolkit.Controls.EnhancedListView.AutoSize" /> is set to <see cref="DW.WPFToolkit.Interactivity.ColumnResizeKind.Proportional" />. If the width is set this property has no effect.
        /// </summary>
        public double ProportionalWidth
        {
            get { return (double)GetValue(ProportionalWidthProperty); }
            set { SetValue(ProportionalWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedGridViewColumn.ProportionalWidth" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ProportionalWidthProperty =
            DependencyProperty.Register("ProportionalWidth", typeof(double), typeof(EnhancedGridViewColumn), new UIPropertyMetadata(OnProportionalWidthChanged));

        private static void OnProportionalWidthChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColumnWidthBehavior.SetProportionalWidth(sender, (double)e.NewValue);
        }
    }
}
