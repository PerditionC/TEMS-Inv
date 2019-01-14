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
using DW.WPFToolkit.Helpers;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Represents a ComboBox which shows a tree view in the drop down.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <Toolkit:TreeComboBox ItemsSource="{Binding Customers}" SelectionBoxItem="{Binding SelectedCustomer}">
    ///     <Toolkit:TreeComboBox.ItemContainerStyle>
    ///         <Style TargetType="{x:Type TreeViewItem}">
    ///             <Setter Property="IsSelected" Value="{Binding IsSelected, Mode=TwoWay}" />
    ///         </Style>
    ///     </Toolkit:TreeComboBox.ItemContainerStyle>
    ///     <Toolkit:TreeComboBox.SelectionBoxItemTemplate>
    ///         <DataTemplate>
    ///             <StackPanel Orientation="Horizontal">
    ///                 <TextBlock Text="{Binding Name}" />
    ///                 <TextBlock Text="{Binding FamilyName}" Margin="5,0,0,0" />
    ///             </StackPanel>
    ///         </DataTemplate>
    ///     </Toolkit:TreeComboBox.SelectionBoxItemTemplate>
    ///     <Toolkit:TreeComboBox.ItemTemplate>
    ///         <HierarchicalDataTemplate ItemsSource="{Binding Customers}">
    ///             <TextBlock Text="{Binding Name}" />
    ///         </HierarchicalDataTemplate>
    ///     </Toolkit:TreeComboBox.ItemTemplate>
    /// </Toolkit:TreeComboBox>
    /// ]]>
    /// </code>
    /// </example>
    public class TreeComboBox : TreeView
    {
        static TreeComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeComboBox), new FrameworkPropertyMetadata(typeof(TreeComboBox)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.TreeComboBox" /> class.
        /// </summary>
        public TreeComboBox()
        {
            var box = new ComboBox();
            MaxDropDownHeight = box.MaxDropDownHeight;
            SelectionBoxItemTemplate = box.SelectionBoxItemTemplate;
            SelectionBoxItemStringFormat = box.SelectionBoxItemStringFormat;
            SelectionBoxItem = "";
        }

        /// <summary>
        /// The template gets added to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            new PopupHandler().AutoClose(this, ClosePopup);
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Gets or sets the maximum height of the drop down popup. This is taken from the calculated height of the original <see cref="System.Windows.Controls.ComboBox.MaxDropDownHeight" />.
        /// </summary>
        [DefaultValue(0.0)]
        public double MaxDropDownHeight
        {
            get { return (double)GetValue(MaxDropDownHeightProperty); }
            set { SetValue(MaxDropDownHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.TreeComboBox.MaxDropDownHeight" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register("MaxDropDownHeight", typeof(double), typeof(TreeComboBox), new UIPropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets data template of the selected item. This is taken from the original <see cref="System.Windows.Controls.ComboBox.SelectionBoxItemTemplate" />.
        /// </summary>
        [DefaultValue(null)]
        public DataTemplate SelectionBoxItemTemplate
        {
            get { return (DataTemplate)GetValue(SelectionBoxItemTemplateProperty); }
            set { SetValue(SelectionBoxItemTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.TreeComboBox.SelectionBoxItemTemplate" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionBoxItemTemplateProperty =
            DependencyProperty.Register("SelectionBoxItemTemplate", typeof(DataTemplate), typeof(TreeComboBox), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the item shown as selected.
        /// </summary>
        [DefaultValue(null)]
        public object SelectionBoxItem
        {
            get { return (object)GetValue(SelectionBoxItemProperty); }
            set { SetValue(SelectionBoxItemProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.TreeComboBox.SelectionBoxItem" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionBoxItemProperty =
            DependencyProperty.Register("SelectionBoxItem", typeof(object), typeof(TreeComboBox), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets string format of the selected item. This is taken from the original <see cref="System.Windows.Controls.ComboBox.SelectionBoxItemStringFormat" />.
        /// </summary>
        [DefaultValue(null)]
        public string SelectionBoxItemStringFormat
        {
            get { return (string)GetValue(SelectionBoxItemStringFormatProperty); }
            set { SetValue(SelectionBoxItemStringFormatProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.TreeComboBox.SelectionBoxItemStringFormat" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionBoxItemStringFormatProperty =
            DependencyProperty.Register("SelectionBoxItemStringFormat", typeof(string), typeof(TreeComboBox), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that indicates of the drop down is opened or not.
        /// </summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.TreeComboBox.IsDropDownOpen" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(TreeComboBox));

        /// <summary>
        /// Sets the current selected item of the selection in the tree has been changed and closed the drop down.
        /// </summary>
        /// <param name="e">The parameter passed by the owner.</param>
        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            var item = e.NewValue as TreeViewItem;
            if (item != null)
                SelectionBoxItem = item.Header;
            else
                SelectionBoxItem = e.NewValue;
            ClosePopup();
            base.OnSelectedItemChanged(e);
        }

        private void ClosePopup()
        {
            if (IsDropDownOpen)
                IsDropDownOpen = false;
        }
    }
}
