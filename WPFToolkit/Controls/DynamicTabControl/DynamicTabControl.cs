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
using System.Windows.Input;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Enhances the <see cref="System.Windows.Controls.TabControl" /> with buttons for add new tab item and close buttons of existing tab items.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <Toolkit:DynamicTabControl ShowAddButton="True"
    ///                            TabItemAddingCommand="{Binding AddItemCommand}"
    ///                            TabItemClosingCommand="{Binding RemoveItemCommand}" />
    /// ]]>
    /// </code>
    /// </example>
    public class DynamicTabControl : TabControl
    {
        static DynamicTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicTabControl), new FrameworkPropertyMetadata(typeof(DynamicTabControl)));
        }

        /// <summary>
        /// Generates a new child item container to hold in the <see cref="DW.WPFToolkit.Controls.DynamicTabControl" />.
        /// </summary>
        /// <returns>The generated child item container</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DynamicTabItem();
        }

        /// <summary>
        /// Checks if the item is already the correct item container. If not the <see cref="DW.WPFToolkit.Controls.DynamicTabControl.GetContainerForItemOverride" /> will be used to generate the right container.
        /// </summary>
        /// <param name="item">The item to shown in the <see cref="DW.WPFToolkit.Controls.DynamicTabControl" />.</param>
        /// <returns>True if the item is the correct item container already.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DynamicTabItem;
        }

        /// <summary>
        /// Gets or sets a value which indicates if the close buttons are shown on the tab items header.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowCloseButtons
        {
            get { return (bool)GetValue(ShowCloseButtonsProperty); }
            set { SetValue(ShowCloseButtonsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DynamicTabControl.ShowCloseButtons" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowCloseButtonsProperty =
            DependencyProperty.Register("ShowCloseButtons", typeof(bool), typeof(DynamicTabControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets the value which indicates if the add new tab item button is shown.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowAddButton
        {
            get { return (bool)GetValue(ShowAddButtonProperty); }
            set { SetValue(ShowAddButtonProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DynamicTabControl.ShowAddButton" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowAddButtonProperty =
            DependencyProperty.Register("ShowAddButton", typeof(bool), typeof(DynamicTabControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets the command which gets called when the close on the tab item header is clicked. The tab DataContext is forwarded as the command parameter.
        /// </summary>
        [DefaultValue(null)]
        public ICommand TabItemClosingCommand
        {
            get { return (ICommand)GetValue(TabItemClosingCommandProperty); }
            set { SetValue(TabItemClosingCommandProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DynamicTabControl.TabItemClosingCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabItemClosingCommandProperty =
            DependencyProperty.Register("TabItemClosingCommand", typeof(ICommand), typeof(DynamicTabControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter which is passed with the <see cref="DW.WPFToolkit.Controls.DynamicTabControl.TabItemAddingCommand" /> command.
        /// </summary>
        [DefaultValue(null)]
        public object TabItemAddingCommandParameter
        {
            get { return (object)GetValue(TabItemAddingCommandParameterProperty); }
            set { SetValue(TabItemAddingCommandParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DynamicTabControl.TabItemAddingCommandParameter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabItemAddingCommandParameterProperty =
            DependencyProperty.Register("TabItemAddingCommandParameter", typeof(object), typeof(DynamicTabControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command which gets called when the add new tab item button is pressed.
        /// </summary>
        [DefaultValue(null)]
        public ICommand TabItemAddingCommand
        {
            get { return (ICommand)GetValue(TabItemAddingCommandProperty); }
            set { SetValue(TabItemAddingCommandProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DynamicTabControl.TabItemAddingCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TabItemAddingCommandProperty =
            DependencyProperty.Register("TabItemAddingCommand", typeof(ICommand), typeof(DynamicTabControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the margin of the add new tab item button.
        /// </summary>
        public Thickness AddButtonMargin
        {
            get { return (Thickness)GetValue(AddButtonMarginProperty); }
            set { SetValue(AddButtonMarginProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DynamicTabControl.AddButtonMargin" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AddButtonMarginProperty =
            DependencyProperty.Register("AddButtonMargin", typeof(Thickness), typeof(DynamicTabControl), new UIPropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Gets or sets the width of the add new tab item button.
        /// </summary>
        [DefaultValue(14.0)]
        public double AddButtonWidth
        {
            get { return (double)GetValue(AddButtonWidthProperty); }
            set { SetValue(AddButtonWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DynamicTabControl.AddButtonWidth" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AddButtonWidthProperty =
            DependencyProperty.Register("AddButtonWidth", typeof(double), typeof(DynamicTabControl), new UIPropertyMetadata(14.0));

        /// <summary>
        /// Gets or sets the height of the add new tab item button.
        /// </summary>
        [DefaultValue(14.0)]
        public double AddButtonHeight
        {
            get { return (double)GetValue(AddButtonHeightProperty); }
            set { SetValue(AddButtonHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DynamicTabControl.AddButtonHeight" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AddButtonHeightProperty =
            DependencyProperty.Register("AddButtonHeight", typeof(double), typeof(DynamicTabControl), new UIPropertyMetadata(14.0));

        /// <summary>
        /// Gets or sets the value which indicates where the add new tab item button has to be placed in the header.
        /// </summary>
        [DefaultValue(Dock.Right)]
        public Dock AddButtonPosition
        {
            get { return (Dock)GetValue(AddButtonPositionProperty); }
            set { SetValue(AddButtonPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.DynamicTabControl.AddButtonPosition" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AddButtonPositionProperty =
            DependencyProperty.Register("AddButtonPosition", typeof(Dock), typeof(DynamicTabControl), new UIPropertyMetadata(Dock.Right));
    }
}
