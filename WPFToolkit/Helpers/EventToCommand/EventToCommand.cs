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
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace DW.WPFToolkit.Helpers
{
    /// <summary>
    /// Brings the possibility to connect any event from a UI control with an ICommand.
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
    public class EventToCommand : FrameworkElement, IDisposable
    {
        private bool _connected;
        private FrameworkElement _owner;
        private EventInfo _event;
        private Delegate _eventHandler;

        /// <summary>
        /// Gets or sets the event name to listen to.
        /// </summary>
        public string Event
        {
            get { return (string)GetValue(EventProperty); }
            set { SetValue(EventProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.EventToCommand.Event" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.Register("Event", typeof(string), typeof(EventToCommand), new PropertyMetadata(null, OnEventChanged));

        /// <summary>
        /// Gets or sets the command to be executed when the event (given by <see cref="DW.WPFToolkit.Helpers.EventToCommand.Event" />) got raised.
        /// </summary>
        /// <remarks>If <see cref="DW.WPFToolkit.Helpers.EventToCommand.CommandParameter" /> is null, the command parameter will be the EventArgs from the event.</remarks>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.EventToCommand.Command" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EventToCommand), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command parameter to sent with the <see cref="DW.WPFToolkit.Helpers.EventToCommand.Event" />.
        /// </summary>
        /// <remarks>If its null, the command parameter will be the EventArgs from the event.</remarks>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Helpers.EventToCommand.CommandParameter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventToCommand), new PropertyMetadata(null));

        /// <summary>
        /// Initializes <see cref="DW.WPFToolkit.Helpers.EventToCommand" /> object with the control which event should be catched.
        /// </summary>
        /// <param name="owner">The control which event should be catched.</param>
        public void Initialize(FrameworkElement owner)
        {
            if (owner == null)
                return;

            _owner = owner;
            DataContext = _owner.DataContext;
            ConnectToEvent();
        }

        /// <summary>
        /// Disposes the <see cref="DW.WPFToolkit.Helpers.EventToCommand.CommandParameter" /> to free it from its owner. <see cref="DW.WPFToolkit.Helpers.EventToCommand.Initialize(FrameworkElement)" /> can be called afterwards again.
        /// </summary>
        public void Dispose()
        {
            if (_connected)
                DisconnectEvent();
            _owner = null;
        }

        private static void OnEventChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var connector = (EventToCommand)sender;
            if (e.OldValue != null)
                connector.DisconnectEvent();
            if (e.NewValue != null)
                connector.ConnectToEvent();
        }

        private void ConnectToEvent()
        {
            if (_connected)
                return;

            if (_owner == null || string.IsNullOrWhiteSpace(Event))
                return;

            try
            {
                var type = _owner.GetType();
                _event = type.GetEvent(Event, BindingFlags.Instance | BindingFlags.Public);

                var handlerType = _event.EventHandlerType;
                var callbackMethod = typeof(EventToCommand).GetMethod("EventRaised", BindingFlags.NonPublic | BindingFlags.Instance);
                _eventHandler = Delegate.CreateDelegate(handlerType, this, callbackMethod);
                _event.AddEventHandler(_owner, _eventHandler);

                _connected = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0}: Cannot read or abonize the given event by its name. {1}", GetType(), ex.Message);
            }
        }

        private void EventRaised(object sender, EventArgs e)
        {
            var parameter = CommandParameter;
            if (parameter == null)
                parameter = e;

            if (Command != null && Command.CanExecute(parameter))
                Command.Execute(parameter);
        }

        private void DisconnectEvent()
        {
            if (!_connected)
                return;

            if (_owner == null || string.IsNullOrWhiteSpace(Event))
                return;

            try
            {
                _event.RemoveEventHandler(_owner, _eventHandler);

                _connected = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0}: Cannot read or deabonize the given event by its name. {1}", GetType(), ex.Message);
            }

            _connected = false;
        }
    }
}
