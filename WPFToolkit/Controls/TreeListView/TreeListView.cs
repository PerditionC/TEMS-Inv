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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Shows the <see cref="DW.WPFToolkit.Controls.EnhancedTreeView" /> with the possibity to expand or collapse child elements shown in a GridView. The expander can be placed in every column cell template.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <WPFToolkit:TreeListView ItemsSource="{Binding Customer}">
    ///     <WPFToolkit:TreeListView.Resources>
    ///         <HierarchicalDataTemplate DataType="{x:Type Data:Customer}" ItemsSource="{Binding Customer}" />
    ///     </WPFToolkit:TreeListView.Resources>
    ///     <WPFToolkit:TreeListView.View>
    ///         <GridView>
    ///             <GridViewColumn Header="Name">
    ///                 <GridViewColumn.CellTemplate>
    ///                     <DataTemplate>
    ///                         <DockPanel>
    ///                             <WPFToolkit:TreeListViewExpander DockPanel.Dock="Left" />
    ///                             <TextBlock Text="{Binding Name}" Margin="5,0,0,0" />
    ///                         </DockPanel>
    ///                     </DataTemplate>
    ///                 </GridViewColumn.CellTemplate>
    ///             </GridViewColumn>
    ///             <GridViewColumn Header="Family Name" DisplayMemberBinding="{Binding FamilyName}" />
    ///         </GridView>
    ///     </WPFToolkit:TreeListView.View>
    /// </WPFToolkit:TreeListView>
    /// ]]>
    /// </code>
    /// </example>
    public class TreeListView : EnhancedTreeView
    {
        static TreeListView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeListView), new FrameworkPropertyMetadata(typeof(TreeListView)));
        }

        /// <summary>
        /// Generates a new child item container to hold in the <see cref="DW.WPFToolkit.Controls.TreeListView" />.
        /// </summary>
        /// <returns>The generated child item container</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TreeListViewItem();
        }

        /// <summary>
        /// Checks if the item is already the correct item container. If not the <see cref="DW.WPFToolkit.Controls.TreeListView.GetContainerForItemOverride" /> will be used to generate the right container.
        /// </summary>
        /// <param name="item">The item to shown in the <see cref="DW.WPFToolkit.Controls.TreeListView" />.</param>
        /// <returns>True if the item is the correct item container already.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TreeListViewItem;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Windows.Controls.GridView"/> shown in the control.
        /// </summary>
        [DefaultValue(null)]
        public GridView View
        {
            get { return (GridView)GetValue(ViewProperty); }
            set { SetValue(ViewProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.TreeListView.View" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewProperty =
            DependencyProperty.Register("View", typeof(GridView), typeof(TreeListView), new UIPropertyMetadata(null));
    }
}
