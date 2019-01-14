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

namespace DW.WPFToolkit.Helpers
{
    /// <summary>
    /// Brings the possibility to append one or more EventToCommand behaviors onto a control.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <StackPanel>
    ///     <Button Content="Demo Single">
    ///         <Helpers:EventToCommandAdapter.EventToCommand>
    ///             <Helpers:EventToCommand Event="MouseLeave" Command="{Binding MouseLeaveCommand}" />
    ///         </Helpers:EventToCommandAdapter.EventToCommand>
    ///     </Button>
    ///     
    ///     <Button Content="Demo Multiple">
    ///         <Helpers:EventToCommandAdapter.EventsToCommands>
    ///             <Helpers:EventToCommandCollection>
    ///                 <Helpers:EventToCommand Event="Click" Command="{Binding ClickCommand}" />
    ///                 <Helpers:EventToCommand Event="MouseEnter" Command="{Binding MouseEnterCommand}" />
    ///                 <Helpers:EventToCommand Event="MouseLeave" Command="{Binding MouseLeaveCommand}" />
    ///             </Helpers:EventToCommandCollection>
    ///         </Helpers:EventToCommandAdapter.EventsToCommands>
    ///     </Button>
    /// </StackPanel>
    /// ]]>
    /// </code>
    /// </example>
    public class EventToCommandAdapter : DependencyObject
    {
        private readonly FrameworkElement _owner;

        private EventToCommandAdapter(FrameworkElement owner)
        {
            _owner = owner;

            _owner.DataContextChanged += HandleDataContextChanged;
            _owner.Loaded += HandleLoaded;
        }

        #region EventToCommandAdapter
        private static EventToCommandAdapter GetEventToCommandAdapter(DependencyObject obj)
        {
            return (EventToCommandAdapter)obj.GetValue(EventToCommandAdapterProperty);
        }

        private static void SetEventToCommandAdapter(DependencyObject obj, EventToCommandAdapter value)
        {
            obj.SetValue(EventToCommandAdapterProperty, value);
        }

        private static readonly DependencyProperty EventToCommandAdapterProperty =
            DependencyProperty.RegisterAttached("EventToCommandAdapter", typeof(EventToCommandAdapter), typeof(EventToCommandAdapter), new PropertyMetadata(null));
        #endregion EventToCommandAdapter

        #region EventToCommand
        /// <summary>
        /// Gets the attached <see cref="DW.WPFToolkit.Helpers.EventToCommand" /> behavior.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Helpers.EventToCommandAdapter.EventToCommand property value for the element.</returns>
        public static EventToCommand GetEventToCommand(DependencyObject obj)
        {
            return (EventToCommand)obj.GetValue(EventToCommandProperty);
        }

        /// <summary>
        /// Sets the attached <see cref="DW.WPFToolkit.Helpers.EventToCommand" /> behavior.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Helpers.EventToCommandAdapter.EventToCommand value.</param>
        public static void SetEventToCommand(DependencyObject obj, EventToCommand value)
        {
            obj.SetValue(EventToCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.EventToCommandAdapter.GetEventToCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Helpers.EventToCommandAdapter.SetEventToCommand(DependencyObject, EventToCommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty EventToCommandProperty =
            DependencyProperty.RegisterAttached("EventToCommand", typeof(EventToCommand), typeof(EventToCommandAdapter), new PropertyMetadata(null, OnEventToCommandChanged));

        private static void OnEventToCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = GetEventToCommandAdapter(d);
            if (container != null)
                return;

            container = new EventToCommandAdapter((FrameworkElement)d);
            SetEventToCommandAdapter(d, container);
        }

        private void UpdateEventToCommand()
        {
            var connection = GetEventToCommand(_owner);
            if (connection == null)
                return;

            connection.Dispose();
            connection.Initialize(_owner);
        }
        #endregion EventToCommand

        #region EventsToCommands
        /// <summary>
        /// Gets the attached <see cref="DW.WPFToolkit.Helpers.EventToCommandCollection" /> behavior.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Helpers.EventToCommandAdapter.EventsToCommands property value for the element.</returns>
        public static EventToCommandCollection GetEventsToCommands(DependencyObject obj)
        {
            var extension = (EventToCommandCollection)obj.GetValue(EventsToCommandsProperty);
            if (extension == null)
            {
                var extensionsCollection = new EventToCommandCollection();
                obj.SetValue(EventsToCommandsProperty, extensionsCollection);
            }
            return extension;
        }

        /// <summary>
        /// Sets the attached <see cref="DW.WPFToolkit.Helpers.EventToCommandCollection" /> behavior.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Helpers.EventToCommandAdapter.EventsToCommands value.</param>
        public static void SetEventsToCommands(DependencyObject obj, EventToCommandCollection value)
        {
            obj.SetValue(EventsToCommandsProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.EventToCommandAdapter.GetEventsToCommands(DependencyObject)" /> <see cref="DW.WPFToolkit.Helpers.EventToCommandAdapter.SetEventsToCommands(DependencyObject, EventToCommandCollection)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty EventsToCommandsProperty =
            DependencyProperty.RegisterAttached("EventsToCommands", typeof(EventToCommandCollection), typeof(EventToCommandAdapter), new PropertyMetadata(null, OnEventsToCommandsChanged));

        private static void OnEventsToCommandsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var container = GetEventToCommandAdapter(d);
            if (container != null)
                return;

            container = new EventToCommandAdapter((FrameworkElement)d);
            SetEventToCommandAdapter(d, container);
        }

        private void UpdateEventsToCommands()
        {
            var connections = GetEventsToCommands(_owner);
            if (connections == null)
                return;

            foreach (var connection in connections)
            {
                connection.Dispose();
                connection.Initialize(_owner);
            }
        }
        #endregion EventsToCommands

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            UpdateEventToCommand();
            UpdateEventsToCommands();
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            UpdateEventToCommand();
            UpdateEventsToCommands();
        }
    }
}