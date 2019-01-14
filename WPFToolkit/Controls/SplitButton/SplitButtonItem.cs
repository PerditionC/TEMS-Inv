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
    /// Represents a single entry in the drop down of the <see cref="DW.WPFToolkit.Controls.SplitButton" />.
    /// </summary>
    public class SplitButtonItem : ComboBoxItem
    {
        static SplitButtonItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButtonItem), new FrameworkPropertyMetadata(typeof(SplitButtonItem)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.SplitButtonItem" /> class.
        /// </summary>
        public SplitButtonItem()
        {
            PreviewMouseLeftButtonUp += SplitButtonItem_PreviewMouseLeftButtonUp;
        }

        private void SplitButtonItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CallClickEvent();
            CallCommand();
        }

        /// <summary>
        /// Gets or sets the command to be executed when the item get clicked.
        /// </summary>
        [DefaultValue(null)]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SplitButtonItem.Command" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitButtonItem), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command parameter passed with the <see cref="DW.WPFToolkit.Controls.SplitButtonItem.Command" />.
        /// </summary>
        [DefaultValue(null)]
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SplitButtonItem.CommandParameter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SplitButtonItem), new UIPropertyMetadata(null));

        /// <summary>
        /// Occurs when the element is clicked.
        /// </summary>
        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SplitButtonItem.Click" /> routed event.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SplitButtonItem));

        private void CallClickEvent()
        {
            var newEventArgs = new RoutedEventArgs(SplitButtonItem.ClickEvent);
            RaiseEvent(newEventArgs);
        }

        private void CallCommand()
        {
            if (Command != null &&
                Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }
    }
}
