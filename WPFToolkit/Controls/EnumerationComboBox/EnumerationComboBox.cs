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

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Represents a ComboBox which takes an enumeration value and shows all possible states inside the dropdown menu for let choosing a value.
    /// </summary>
    /// <example>
    /// In the example the data shown in C# is the base the UI is binding to. See the XAML tab for the control usages.
    /// <code lang="csharp">
    /// <![CDATA[
    /// public enum Number
    /// {
    ///     [Description("The Number One")]
    ///     One,
    /// 
    ///     [Description("The Number Two")]
    ///     Two,
    /// 
    ///     [Description("The Number Three")]
    ///     Three
    /// }
    /// 
    /// public class MainViewModel : ObservableObject
    /// {
    ///     public MainViewModel()
    ///     {
    ///         Number = Number.One;
    ///     }
    /// 
    ///     public Number Number
    ///     {
    ///         get { return _number; }
    ///         set
    ///         {
    ///             _number = value;
    ///             NotifyPropertyChanged("Number");
    ///         }
    ///     }
    ///     private Number _number;
    /// }
    /// ]]>
    /// </code>
    /// <code lang="xaml">
    /// <![CDATA[
    /// <!-- The items will be shown like "Name: One; Description: The Number One". -->
    /// <!-- (The DW.WPFToolkit.Controls.EnumDescriptionConverter will return the description value unchanged) -->
    /// <!-- DisplayKind is not set so the default will be taken which is EnumDisplayKind.Custom -->
    /// <Controls:EnumerationComboBox EnumType="{x:Type Demo:Number}" SelectedItem="{Binding Number}">
    ///     <Controls:EnumerationComboBox.ItemTemplate>
    ///         <DataTemplate>
    ///             <StackPanel Orientation="Horizontal">
    ///                 <TextBlock Text="Name: " />
    ///                 <TextBlock Text="{Binding }" />
    ///                 <TextBlock Text="; Description: " />
    ///                 <TextBlock Text="{Binding Converter={StaticResource EnumDescriptionConverter}}" />
    ///             </StackPanel>
    ///         </DataTemplate>
    ///     </Controls:EnumerationComboBox.ItemTemplate>
    /// </Controls:EnumerationComboBox>
    ///     
    /// <!-- The items will be shown like "The Number One". -->
    /// <Controls:EnumerationComboBox EnumType="{x:Type Demo:Number}" SelectedItem="{Binding Number}" DisplayKind="Description" />
    /// 
    /// <!-- The items will be shown like "One". -->    
    /// <Controls:EnumerationComboBox EnumType="{x:Type Demo:Number}" SelectedItem="{Binding Number}" DisplayKind="ToString" />
    ///     
    /// <!-- The items will be shown how you defined in the EnumToStringConverter. -->
    /// <Controls:EnumerationComboBox EnumType="{x:Type Demo:Number}" SelectedItem="{Binding Number}" DisplayKind="Converter" ItemConverter="{StaticResource EnumToStringConverter}" />
    /// ]]>
    /// </code>
    /// </example>
    public class EnumerationComboBox : ComboBox
    {
        static EnumerationComboBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumerationComboBox), new FrameworkPropertyMetadata(typeof(EnumerationComboBox)));
            SelectedItemProperty.OverrideMetadata(typeof(EnumerationComboBox), new FrameworkPropertyMetadata(OnSelectedItemChanged, OnSelectedItemChanging));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.EnumerationComboBox" /> class.
        /// </summary>
        public EnumerationComboBox()
        {
            Loaded += HandleLoaded;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            if (EnumType != null)
                return;

            var enumTypeBinding = GetBindingExpression(EnumTypeProperty);
            if (enumTypeBinding != null)
                return;

            var selectedItemBinding = GetBindingExpression(SelectedItemProperty);
            if (selectedItemBinding != null)
            {
                var type = GetBoundType(selectedItemBinding);
                if (type != null)
                    EnumType = type;
            }
            else if (SelectedItem != null)
                EnumType = SelectedItem.GetType();
        }

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private static object OnSelectedItemChanging(DependencyObject sender, object basevalue)
        {
            if (basevalue == null)
                return null;

            var control = (EnumerationComboBox)sender;
            if (control.EnumType != null)
                return basevalue;

            control.EnumType = basevalue.GetType();
            return basevalue;
        }

        /// <summary>
        /// Checks if the item is already the correct item container. If not the <see cref="DW.WPFToolkit.Controls.EnumerationComboBox.GetContainerForItemOverride" /> will be used to generate the right container.
        /// </summary>
        /// <param name="item">The item to shown in the <see cref="DW.WPFToolkit.Controls.EnumerationComboBox" />.</param>
        /// <returns>True if the item is the correct item container already.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is EnumerationComboBoxItem;
        }

        /// <summary>
        /// Generates a new child item container to hold in the <see cref="DW.WPFToolkit.Controls.EnumerationComboBox" />.
        /// </summary>
        /// <returns>The generated child item container.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new EnumerationComboBoxItem();
        }

        /// <summary>
        /// Gets or sets the type of the enum which named can be selected from.
        /// </summary>
        public Type EnumType
        {
            get { return (Type)GetValue(EnumTypeProperty); }
            set { SetValue(EnumTypeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnumerationComboBox.EnumType" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register("EnumType", typeof(Type), typeof(EnumerationComboBox), new PropertyMetadata(OnEnumTypeChanged));

        /// <summary>
        /// Gets or sets the way hot to display the items in the drop down or in the selection box itself.
        /// </summary>
        /// <remarks>The default is <see cref="DW.WPFToolkit.Controls.EnumDisplayKind.Custom" /> which means you have to define the EnumerationComboBox.ItemTemplate by yourself.</remarks>
        /// <remarks>Note: When <see cref="DW.WPFToolkit.Controls.EnumDisplayKind.Custom" /> is set you need to set the <see cref="DW.WPFToolkit.Controls.EnumerationComboBox.ItemConverter" /> as well; otehrwise <see cref="DW.WPFToolkit.Controls.EnumDisplayKind.ToString" /> will be used as a default.</remarks>
        [DefaultValue(EnumDisplayKind.Custom)]
        public EnumDisplayKind DisplayKind
        {
            get { return (EnumDisplayKind)GetValue(DisplayKindProperty); }
            set { SetValue(DisplayKindProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnumerationComboBox.DisplayKind" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayKindProperty =
            DependencyProperty.Register("DisplayKind", typeof(EnumDisplayKind), typeof(EnumerationComboBox), new PropertyMetadata(EnumDisplayKind.Custom, OnItemConverterChanged));

        /// <summary>
        /// Gets or sets the converter to use when <see cref="DW.WPFToolkit.Controls.EnumDisplayKind.Custom" /> is set as the <see cref="DW.WPFToolkit.Controls.EnumerationComboBox.DisplayKind" />.
        /// </summary>
        public IValueConverter ItemConverter
        {
            get { return (IValueConverter)GetValue(ItemConverterProperty); }
            set { SetValue(ItemConverterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnumerationComboBox.ItemConverter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemConverterProperty =
            DependencyProperty.Register("ItemConverter", typeof(IValueConverter), typeof(EnumerationComboBox), new PropertyMetadata(OnItemConverterChanged));

        private static void OnItemConverterChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (EnumerationComboBox)sender;
            if (control.DisplayKind != EnumDisplayKind.Converter || control.ItemConverter == null)
                return;

            var textBinding = new Binding { Converter = control.ItemConverter };
            var template = new DataTemplate();

            var textBlockControl = new FrameworkElementFactory(typeof(TextBlock));
            textBlockControl.SetBinding(TextBlock.TextProperty, textBinding);
            template.VisualTree = textBlockControl;

            control.ItemTemplate = template;
        }

        private static void OnEnumTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (EnumerationComboBox)sender;

            if (control.ItemsSource != null)
                return;

            TakeValues(control);
        }

        private static void TakeValues(EnumerationComboBox control)
        {
            control.Items.Clear();
            foreach (var value in Enum.GetValues(control.EnumType))
                control.Items.Add(value);
        }

        private static Type GetBoundType(BindingExpression bindingExpression)
        {
            var split = bindingExpression.ParentBinding.Path.Path.Split('.').LastOrDefault();
            if (split == null)
                return null;
            var dataItem = bindingExpression.DataItem;
            if (dataItem == null)
                return null;
            var type = dataItem.GetType();
            var propertyInfo = type.GetProperty(split);
            if (propertyInfo == null)
                return null;
            return propertyInfo.PropertyType;
        }
    }
}
