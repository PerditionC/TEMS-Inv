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
using System.Windows;
using System.Windows.Input;
using DW.WPFToolkit.Helpers;

namespace DW.WPFToolkit.Interactivity
{
    /// <summary>
    /// Brings the feature to set the focus to a specific element or on window launch.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <Window Interactivity:FocusBehavior.ApplicationGotFocusCommand="{Binding SwitchedToApplicationCommand}"
    ///         Interactivity:FocusBehavior.ApplicationLostFocusCommand="{Binding SwitchedOutFromApplicationCommand}">
    /// </Window>
    /// 
    /// <Button Interactivity:FocusBehavior.GotFocusCommand="{Binding ButtonGotFocusCommand}"
    ///         Interactivity:FocusBehavior.GotFocusCommandParameter="Example" />
    /// 
    /// <Button Interactivity:FocusBehavior.LostFocusCommand="{Binding ButtonGotFocusCommand}"
    ///         Interactivity:FocusBehavior.LostFocusCommandParameter="Example" />
    ///  
    /// <Button Interactivity:FocusBehavior.HasFocus="{Binding IsButtonFocused}" />
    /// ]]>
    /// </code>
    /// </example>
    public class FocusBehavior : DependencyObject
    {
        #region StartFocusedControl
        /// <summary>
        /// Gets the control which has to get the focus when its loaded.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.FocusBehavior.StartFocusedControl property value for the element.</returns>
        public static UIElement GetStartFocusedControl(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(StartFocusedControlProperty);
        }

        /// <summary>
        /// Attaches the control which has to get the focus when its loaded.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.FocusBehavior.StartFocusedControl value.</param>
        public static void SetStartFocusedControl(DependencyObject obj, UIElement value)
        {
            obj.SetValue(StartFocusedControlProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.GetStartFocusedControl(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.SetStartFocusedControl(DependencyObject, UIElement)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty StartFocusedControlProperty =
            DependencyProperty.RegisterAttached("StartFocusedControl", typeof(UIElement), typeof(FocusBehavior), new UIPropertyMetadata(OnStartFocusedControlChanged));

        private static void OnStartFocusedControlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                throw new InvalidOperationException("The FocusBehavior.StartFocusedControl only can be attached to an FrameworkElement");

            element.Loaded += Element_Loaded;
        }

        private static void Element_Loaded(object sender, RoutedEventArgs e)
        {
            var target = GetStartFocusedControl((DependencyObject)sender);
            ControlFocus.GiveFocus(target);
        }
        #endregion StartFocusedControl

        #region HasFocus
        /// <summary>
        /// Gets a value that indicates the state if the element has the focus or not.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.FocusBehavior.HasFocus property value for the element.</returns>
        public static bool GetHasFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(HasFocusProperty);
        }

        /// <summary>
        /// Attaches a value that indicates the state if the element has the focus or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.FocusBehavior.HasFocus value.</param>
        public static void SetHasFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(HasFocusProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.GetHasFocus(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.SetHasFocus(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty HasFocusProperty =
            DependencyProperty.RegisterAttached("HasFocus", typeof(bool), typeof(FocusBehavior), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnHasFocusChanged));

        private static void OnHasFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as FrameworkElement;
            if (element == null)
                throw new InvalidOperationException("The FocusBehavior.HasFocus only can be attached to an FrameworkElement");

            if ((bool)e.NewValue)
            {
                ControlFocus.GiveFocus(element);
                element.LostFocus += Element_LostFocus;
            }
        }

        private static void Element_LostFocus(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            element.LostFocus -= Element_LostFocus;
            SetHasFocus(element, false);
        }
        #endregion HasFocus

        #region LostFocusCommand
        /// <summary>
        /// Gets the command to be executed when the element lost its focus.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.FocusBehavior.LostFocusCommand property value for the element.</returns>
        public static ICommand GetLostFocusCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LostFocusCommandProperty);
        }

        /// <summary>
        /// Attaches the command to be executed when the control lost its focus.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.FocusBehavior.LostFocusCommand value.</param>
        public static void SetLostFocusCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LostFocusCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.GetLostFocusCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.SetLostFocusCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty LostFocusCommandProperty =
            DependencyProperty.RegisterAttached("LostFocusCommand", typeof(ICommand), typeof(FocusBehavior), new PropertyMetadata(LostFocusCommandChanged));

        /// <summary>
        /// Gets the parameter to be passed with the DW.WPFToolkit.Interactivity.FocusBehavior.LostFocusCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.FocusBehavior.LostFocusCommandParameter property value for the element.</returns>
        public static object GetLostFocusCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(LostFocusCommandParameterProperty);
        }

        /// <summary>
        /// Sets the parameter to be passed with the DW.WPFToolkit.Interactivity.FocusBehavior.LostFocusCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.FocusBehavior.LostFocusCommandParameter value.</param>
        public static void SetLostFocusCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(LostFocusCommandParameterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.GetLostFocusCommandParameter(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.SetLostFocusCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty LostFocusCommandParameterProperty =
            DependencyProperty.RegisterAttached("LostFocusCommandParameter", typeof(object), typeof(FocusBehavior), new PropertyMetadata(null));

        private static void LostFocusCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as UIElement;
            if (control == null)
                throw new InvalidOperationException("The FocusBehavior.LostFocusCommand only can be attached to an UIElement");

            if (e.OldValue != null)
                control.LostFocus -= HandleLostFocus;
            if (e.NewValue != null)
                control.LostFocus += HandleLostFocus;
        }

        private static void HandleLostFocus(object sender, RoutedEventArgs e)
        {
            var command = GetLostFocusCommand((DependencyObject)sender);
            var commandParameter = GetLostFocusCommandParameter((DependencyObject)sender);
            if (command != null && command.CanExecute(commandParameter))
                command.Execute(commandParameter);
        }

        #endregion LostFocusCommand

        #region GotFocusCommand
        /// <summary>
        /// Gets the command to be executed when the element got the focus.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.FocusBehavior.GotFocusCommand property value for the element.</returns>
        public static ICommand GetGotFocusCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(GotFocusCommandProperty);
        }

        /// <summary>
        /// Attaches the command to be executed when the control got the focus.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.FocusBehavior.GotFocusCommand value.</param>
        public static void SetGotFocusCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(GotFocusCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.GetGotFocusCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.SetGotFocusCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty GotFocusCommandProperty =
            DependencyProperty.RegisterAttached("GotFocusCommand", typeof(ICommand), typeof(FocusBehavior), new PropertyMetadata(OnGotFocusCommandChanged));

        /// <summary>
        /// Gets the parameter to be passed with the DW.WPFToolkit.Interactivity.FocusBehavior.GotFocusCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.FocusBehavior.GotFocusCommandParameter property value for the element.</returns>
        public static object GetGotFocusCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(GotFocusCommandParameterProperty);
        }

        /// <summary>
        /// Sets the parameter to be passed with the DW.WPFToolkit.Interactivity.FocusBehavior.GotFocusCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.FocusBehavior.GotFocusCommandParameter value.</param>
        public static void SetGotFocusCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(GotFocusCommandParameterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.GetGotFocusCommandParameter(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.SetGotFocusCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty GotFocusCommandParameterProperty =
            DependencyProperty.RegisterAttached("GotFocusCommandParameter", typeof(object), typeof(FocusBehavior), new PropertyMetadata(null));

        private static void OnGotFocusCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as UIElement;
            if (control == null)
                throw new InvalidOperationException("The FocusBehavior.LostFocusCommand only can be attached to an UIElement");

            if (e.OldValue != null)
                control.GotFocus -= HandleGotFocus;
            if (e.NewValue != null)
                control.GotFocus += HandleGotFocus;
        }

        private static void HandleGotFocus(object sender, RoutedEventArgs e)
        {
            var command = GetGotFocusCommand((DependencyObject)sender);
            var commandParameter = GetGotFocusCommandParameter((DependencyObject)sender);
            if (command != null && command.CanExecute(commandParameter))
                command.Execute(commandParameter);
        }
        #endregion GotFocusCommand

        #region ApplicationLostFocusCommand
        /// <summary>
        /// Gets the command to be executed when the application is not the foreground application anymore.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.FocusBehavior.ApplicationLostFocusCommand property value for the element.</returns>
        public static ICommand GetApplicationLostFocusCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ApplicationLostFocusCommandProperty);
        }

        /// <summary>
        /// Attaches the command to be executed when the application is not the foreground application anymore.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.FocusBehavior.ApplicationLostFocusCommand value.</param>
        public static void SetApplicationLostFocusCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ApplicationLostFocusCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.GetApplicationLostFocusCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.SetApplicationLostFocusCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ApplicationLostFocusCommandProperty =
            DependencyProperty.RegisterAttached("ApplicationLostFocusCommand", typeof(ICommand), typeof(FocusBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Gets the parameter to be passed with the DW.WPFToolkit.Interactivity.FocusBehavior.ApplicationLostFocusCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.FocusBehavior.ApplicationLostFocusCommandParameter property value for the element.</returns>
        public static object GetApplicationLostFocusCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(ApplicationLostFocusCommandParameterProperty);
        }

        /// <summary>
        /// Sets the parameter to be passed with the DW.WPFToolkit.Interactivity.FocusBehavior.ApplicationLostFocusCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.FocusBehavior.ApplicationLostFocusCommandParameter value.</param>
        public static void SetApplicationLostFocusCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ApplicationLostFocusCommandParameterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.GetApplicationLostFocusCommandParameter(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.SetApplicationLostFocusCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ApplicationLostFocusCommandParameterProperty =
            DependencyProperty.RegisterAttached("ApplicationLostFocusCommandParameter", typeof(object), typeof(FocusBehavior), new PropertyMetadata(OnApplicationFocusCommandChanged));
        #endregion ApplicationLostFocusCommand

        #region ApplicationGotFocusCommand
        /// <summary>
        /// Gets the command to be executed when the application become the foreground application.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.FocusBehavior.ApplicationGotFocusCommand property value for the element.</returns>
        public static ICommand GetApplicationGotFocusCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ApplicationGotFocusCommandProperty);
        }

        /// <summary>
        /// Attaches the command to be executed when the application become the foreground application.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.FocusBehavior.ApplicationGotFocusCommand value.</param>
        public static void SetApplicationGotFocusCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ApplicationGotFocusCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.GetApplicationGotFocusCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.SetApplicationGotFocusCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ApplicationGotFocusCommandProperty =
            DependencyProperty.RegisterAttached("ApplicationGotFocusCommand", typeof(ICommand), typeof(FocusBehavior), new PropertyMetadata(OnApplicationFocusCommandChanged));

        /// <summary>
        /// Gets the parameter to be passed with the DW.WPFToolkit.Interactivity.FocusBehavior.ApplicationGotFocusCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.FocusBehavior.ApplicationGotFocusCommandParameter property value for the element.</returns>
        public static object GetApplicationGotFocusCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(ApplicationGotFocusCommandParameterProperty);
        }

        /// <summary>
        /// Sets the parameter to be passed with the DW.WPFToolkit.Interactivity.FocusBehavior.ApplicationGotFocusCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.FocusBehavior.ApplicationGotFocusCommandParameter value.</param>
        public static void SetApplicationGotFocusCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ApplicationGotFocusCommandParameterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.GetApplicationGotFocusCommandParameter(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.FocusBehavior.SetApplicationGotFocusCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ApplicationGotFocusCommandParameterProperty =
            DependencyProperty.RegisterAttached("ApplicationGotFocusCommandParameter", typeof(object), typeof(FocusBehavior), new PropertyMetadata(null));
        #endregion ApplicationGotFocusCommand

        #region FocusEventWatching
        private static void OnApplicationFocusCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var watcher = GetGlobalEventWatcher(d);
            if (watcher != null)
                watcher.Dispose();

            if (e.NewValue != null)
            {
                watcher = new GlobalEventWatcher(d);
                SetGlobalEventWatcher(d, watcher);
            }
        }

        private static GlobalEventWatcher GetGlobalEventWatcher(DependencyObject obj)
        {
            return (GlobalEventWatcher)obj.GetValue(GlobalEventWatcherProperty);
        }

        private static void SetGlobalEventWatcher(DependencyObject obj, GlobalEventWatcher value)
        {
            obj.SetValue(GlobalEventWatcherProperty, value);
        }

        private static readonly DependencyProperty GlobalEventWatcherProperty =
            DependencyProperty.RegisterAttached("GlobalEventWatcher", typeof(GlobalEventWatcher), typeof(FocusBehavior));

        private class GlobalEventWatcher
        {
            private DependencyObject _owner;

            public GlobalEventWatcher(DependencyObject owner)
            {
                _owner = owner;
                Application.Current.Activated += HandleApplicationActivated;
                Application.Current.Deactivated += HandleApplicationDeactivated;
            }

            public void Dispose()
            {
                Application.Current.Activated -= HandleApplicationActivated;
                Application.Current.Deactivated -= HandleApplicationDeactivated;
                _owner = null;
            }

            private void HandleApplicationActivated(object sender, EventArgs e)
            {
                var command = GetApplicationGotFocusCommand(_owner);
                var commandParameter = GetApplicationGotFocusCommandParameter(_owner);
                if (command != null && command.CanExecute(commandParameter))
                    command.Execute(commandParameter);
            }

            private void HandleApplicationDeactivated(object sender, EventArgs e)
            {
                var command = GetApplicationLostFocusCommand(_owner);
                var commandParameter = GetApplicationLostFocusCommandParameter(_owner);
                if (command != null && command.CanExecute(commandParameter))
                    command.Execute(commandParameter);
            }
        }
        #endregion FocusEventWatching
    }
}
