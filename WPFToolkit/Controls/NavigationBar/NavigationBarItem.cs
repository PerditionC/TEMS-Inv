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

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Represents a single item hostet in the <see cref="DW.WPFToolkit.Controls.NavigationBar" />.
    /// </summary>
    public class NavigationBarItem : HeaderedContentControl
    {
        static NavigationBarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationBarItem), new FrameworkPropertyMetadata(typeof(NavigationBarItem)));
        }

        /// <summary>
        /// Gets or sets a value that indicates of the item is expanded or not.
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>IsExpanded
        /// Identifies the <see cref="DW.WPFToolkit.Controls.NavigationBarItem.IsExpanded" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register("IsExpanded", typeof(bool), typeof(NavigationBarItem), new PropertyMetadata(OnIsExpandedChanged));

        /// <summary>
        /// Gets or set the orientation of the item hosted in the <see cref="DW.WPFToolkit.Controls.NavigationBar" />.
        /// </summary>
        [DefaultValue(Orientation.Vertical)]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.NavigationBarItem.Orientation" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(NavigationBarItem), new PropertyMetadata(Orientation.Vertical));

        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NavigationBarItem)d;
            var value = (bool)e.NewValue;
            NavigationBarPanel.SetIsExpanded(control, value);

            if (value)
                control.OnExpanded();
            else
                control.OnCollapsed();
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.NavigationBarItem.Expanded" /> routed event.
        /// </summary>
        public static readonly RoutedEvent ExpandedEvent = EventManager.RegisterRoutedEvent("Expanded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavigationBarItem));

        /// <summary>
        /// Occurs when the item got expanded.
        /// </summary>
        public event RoutedEventHandler Expanded
        {
            add { AddHandler(ExpandedEvent, value); }
            remove { RemoveHandler(ExpandedEvent, value); }
        }

        private void OnExpanded()
        {
            var newEventArgs = new RoutedEventArgs(NavigationBarItem.ExpandedEvent);
            RaiseEvent(newEventArgs);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.NavigationBarItem.Collapsed" /> routed event.
        /// </summary>
        public static readonly RoutedEvent CollapsedEvent = EventManager.RegisterRoutedEvent("Collapsed", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NavigationBarItem));

        /// <summary>
        /// Occurs when the item got collapsed.
        /// </summary>
        public event RoutedEventHandler Collapsed
        {
            add { AddHandler(CollapsedEvent, value); }
            remove { RemoveHandler(CollapsedEvent, value); }
        }

        private void OnCollapsed()
        {
            var newEventArgs = new RoutedEventArgs(NavigationBarItem.CollapsedEvent);
            RaiseEvent(newEventArgs);
        }
    }
}
