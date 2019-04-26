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

using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DW.WPFToolkit.Interactivity;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Enhances the <see cref="System.Windows.Controls.ListView" /> with additional features like column sorting, dynamic column width and dynamic shown column.
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
    /// <WPFToolkit:EnhancedListView AllowColumnSortings="True"
    ///                              AscendingSortHeaderTemplate="{StaticResource HeaderArrowUp}"
    ///                              DescendingSortHeaderTemplate="{StaticResource HeaderArrowDown}"
    ///                              NeutralHeaderTemplate="{StaticResource HeaderTransparent}"
    ///                              
    ///                              VisibleColumns="{Binding VisibleColumnNames}"
    ///                              
    ///                              AutoSize="Proportional"
    ///                              >
    ///     <ListView.View>
    ///         <GridView>
    ///             <WPFToolkit:EnhancedGridViewColumn SortPropertyName="Name" IsDefaultSortColumn="True" Name="Name" ProportionalWidth="60" />
    ///             <WPFToolkit:EnhancedGridViewColumn SortPropertyName="Size" Name="Size" ProportionalWidth="30" />
    ///             <WPFToolkit:EnhancedGridViewColumn SortPropertyName="Date" Name="Date" ProportionalWidth="10" />
    ///         </GridView>
    ///     </ListView.View>
    /// </WPFToolkit:EnhancedListView>
    /// ]]>
    /// </code>
    /// </example>
    public class EnhancedListView : ListView
    {
        /// <summary>
        /// Gets or sets a value which indicates how the columns should be resized. Columns which width is set will not modified.
        /// </summary>
        [DefaultValue(ColumnResizeKind.NoResize)]
        public ColumnResizeKind AutoSize
        {
            get { return (ColumnResizeKind)GetValue(AutoSizeProperty); }
            set { SetValue(AutoSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedListView.AutoSize" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoSizeProperty =
            DependencyProperty.Register("AutoSize", typeof(ColumnResizeKind), typeof(EnhancedListView), new UIPropertyMetadata(ColumnResizeKind.NoResize, OnAutoSizeChanged));

        private static void OnAutoSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColumnWidthBehavior.SetAutoSize(sender, (ColumnResizeKind)e.NewValue);
        }

        /// <summary>
        /// Gets or sets additional distance on the right for the sum of all column width if the width is calculated.
        /// </summary>
        [DefaultValue(10.0)]
        public double TemplatePaddingWidthFix
        {
            get { return (double)GetValue(TemplatePaddingWidthFixProperty); }
            set { SetValue(TemplatePaddingWidthFixProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedListView.TemplatePaddingWidthFix" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TemplatePaddingWidthFixProperty =
            DependencyProperty.Register("TemplatePaddingWidthFix", typeof(double), typeof(EnhancedListView), new UIPropertyMetadata(10.0, OnTemplatePaddingWidthFixChanged));

        private static void OnTemplatePaddingWidthFixChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColumnWidthBehavior.SetTemplatePaddingWidthFix(sender, (double)e.NewValue);
        }

        /// <summary>
        /// Gets or sets a collection of columns which should be visible by their name.
        /// </summary>
        public IList VisibleColumns
        {
            get { return (IList)GetValue(VisibleColumnsProperty); }
            set { SetValue(VisibleColumnsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedListView.VisibleColumns" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty VisibleColumnsProperty =
            DependencyProperty.Register("VisibleColumns", typeof(IList), typeof(EnhancedListView), new UIPropertyMetadata(OnVisibleColumnsChanged));

        private static void OnVisibleColumnsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColumnVisibilityBehavior.SetVisibleColumns(sender, (IList)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the header template to be used if <see cref="DW.WPFToolkit.Controls.EnhancedListView.AllowColumnSortings" /> is set to true and the items are not sorted by a column.
        /// </summary>
        [DefaultValue(null)]
        public DataTemplate NeutralHeaderTemplate
        {
            get { return (DataTemplate)GetValue(NeutralHeaderTemplateProperty); }
            set { SetValue(NeutralHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedListView.NeutralHeaderTemplate" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty NeutralHeaderTemplateProperty =
            DependencyProperty.Register("NeutralHeaderTemplate", typeof(DataTemplate), typeof(EnhancedListView), new UIPropertyMetadata(null, OnNeutralHeaderTemplateChanged));

        private static void OnNeutralHeaderTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColumnSortBehavior.SetNeutralHeaderTemplate(sender, (DataTemplate)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the header template to be used if <see cref="DW.WPFToolkit.Controls.EnhancedListView.AllowColumnSortings" /> is set to true and the items are sorted by a column in an ascending order.
        /// </summary>
        [DefaultValue(null)]
        public DataTemplate AscendingSortHeaderTemplate
        {
            get { return (DataTemplate)GetValue(AscendingSortHeaderTemplateProperty); }
            set { SetValue(AscendingSortHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedListView.AscendingSortHeaderTemplate" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AscendingSortHeaderTemplateProperty =
            DependencyProperty.Register("AscendingSortHeaderTemplate", typeof(DataTemplate), typeof(EnhancedListView), new UIPropertyMetadata(null, OnAscendingSortHeaderTemplateChanged));

        private static void OnAscendingSortHeaderTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColumnSortBehavior.SetAscendingSortHeaderTemplate(sender, (DataTemplate)e.NewValue);
        }

        /// <summary>
        /// Gets or sets the header template to be used if <see cref="DW.WPFToolkit.Controls.EnhancedListView.AllowColumnSortings" /> is set to true and the items are sorted by a column in an descending order.
        /// </summary>
        [DefaultValue(null)]
        public DataTemplate DescendingSortHeaderTemplate
        {
            get { return (DataTemplate)GetValue(DescendingSortHeaderTemplateProperty); }
            set { SetValue(DescendingSortHeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedListView.DescendingSortHeaderTemplate" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DescendingSortHeaderTemplateProperty =
            DependencyProperty.Register("DescendingSortHeaderTemplate", typeof(DataTemplate), typeof(EnhancedListView), new UIPropertyMetadata(null, OnDescendingSortHeaderTemplateChanged));

        private static void OnDescendingSortHeaderTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColumnSortBehavior.SetDescendingSortHeaderTemplate(sender, (DataTemplate)e.NewValue);
        }

        /// <summary>
        /// Gets or sets a value which indicates of sorting by clicking on the header is enabled or not. To have this functionality the <see cref="DW.WPFToolkit.Controls.EnhancedGridViewColumn.SortPropertyName" /> has to be set in the columns. For display arrows to indicate the sort direction consider setting header templates.
        /// </summary>
        [DefaultValue(false)]
        public bool AllowColumnSortings
        {
            get { return (bool)GetValue(AllowColumnSortingsProperty); }
            set { SetValue(AllowColumnSortingsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedListView.AllowColumnSortings" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowColumnSortingsProperty =
            DependencyProperty.Register("AllowColumnSortings", typeof(bool), typeof(EnhancedListView), new UIPropertyMetadata(false, OnAllowColumnSortingsChanged));

        private static void OnAllowColumnSortingsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ColumnSortBehavior.SetAllowColumnSortings(sender, (bool)e.NewValue);
        }
    }
}
