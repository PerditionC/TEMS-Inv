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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Shows a button which can be clicked but also expanded to call commands from child elements.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <WPFToolkit:SplitButton Content="Main" Command="{Binding MainCommand}">
    ///     <WPFToolkit:SplitButtonItem Content="Sub 1" Command="{Binding Sub1Command}" />
    ///     <WPFToolkit:SplitButtonItem Content="Sub 2" Command="{Binding Sub2Command}" />
    ///     <WPFToolkit:SplitButtonItem Content="Sub 3" Command="{Binding Sub3Command}" />
    ///     <WPFToolkit:SplitButtonItem Content="Sub 4" Command="{Binding Sub4Command}" />
    /// </WPFToolkit:SplitButton>
    /// ]]>
    /// </code>
    /// </example>
    [TemplatePart(Name = "PART_ContentButton", Type = typeof(SplitToggleButton))]
    public class SplitButton : ComboBox
    {
        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        /// <summary>
        /// The template gets added to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var content = GetTemplateChild("PART_ContentButton") as SplitToggleButton;
            if (content != null)
                content.Click += new RoutedEventHandler(Content_Click);
        }

        private void Content_Click(object sender, RoutedEventArgs e)
        {
            CallClickEvent();
            CallCommand();
        }

        /// <summary>
        /// Gets or sets the command to be called when the button itself is clicked.
        /// </summary>
        [DefaultValue(null)]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SplitButton.Command" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitButton), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter to be passed with the <see cref="DW.WPFToolkit.Controls.SplitButton.Command" />.
        /// </summary>
        [DefaultValue(null)]
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SplitButton.CommandParameter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SplitButton), new UIPropertyMetadata(null));

        /// <summary>
        /// Occurs when the main button is clicked.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SplitButton.Click" /> routed event.
        /// </summary>
        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SplitButton));

        private void CallClickEvent()
        {
            var newEventArgs = new RoutedEventArgs(SplitButton.ClickEvent);
            RaiseEvent(newEventArgs);
        }

        private void CallCommand()
        {
            if (Command != null &&
                Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }

        /// <summary>
        /// Gets or sets the main content of the button.
        /// </summary>
        [DefaultValue(null)]
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SplitButton.Content" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(SplitButton), new UIPropertyMetadata(null));

        /// <summary>
        /// Generates a new child item container to hold in the <see cref="DW.WPFToolkit.Controls.SplitButton" />.
        /// </summary>
        /// <returns>The generated child item container</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SplitButtonItem();
        }

        /// <summary>
        /// Checks if the item is already the correct item container. If not the <see cref="DW.WPFToolkit.Controls.SplitButton.GetContainerForItemOverride" /> will be used to generate the right container.
        /// </summary>
        /// <param name="item">The item to shown in the <see cref="DW.WPFToolkit.Controls.SplitButton" />.</param>
        /// <returns>True if the item is the correct item container already.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SplitButtonItem;
        }
    }
}
