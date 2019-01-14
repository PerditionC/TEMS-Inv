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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DW.WPFToolkit.Helpers;

namespace DW.WPFToolkit.Interactivity
{
    /// <summary>
    /// Brings the feature to a <see cref="System.Windows.Window" /> to bind loading and closing action or easy close with dialog result.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <Window Interactivity:WindowBehavior.ClosingCommand="{Binding ClosingCommand}">
    /// 
    ///     <Button Content="Close" Interactivity:WindowBehavior.DialogResult="True" />
    ///     
    ///     <Button Content="Try Close" Interactivity:WindowBehavior.DialogResultCommand="{Binding TryCloseCommand}" />
    /// 
    /// </Window>
    /// ]]>
    /// </code>
    /// 
    /// <code lang="csharp">
    /// <![CDATA[
    /// public class MainViewModel : ObservableObject
    /// {
    ///     public MainViewModel()
    ///     {
    ///         TryCloseCommand = new DelegateCommand<WindowClosingArgs>(TryClose);
    ///     }
    /// 
    ///     public DelegateCommand<WindowClosingArgs> TryCloseCommand { get; private set; }
    /// 
    ///     private void TryClose(WindowClosingArgs e)
    ///     {
    ///         // Ask user if really close
    ///         e.Cancel = true;
    /// 
    ///         //e.DialogResult = false;
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class WindowBehavior : DependencyObject
    {
        #region DialogResult
        /// <summary>
        /// Gets the dialog result from a button to be called on the owner window.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.WindowBehavior.DialogResult property value for the element.</returns>
        public static bool? GetDialogResult(DependencyObject obj)
        {
            return (bool?)obj.GetValue(DialogResultProperty);
        }

        /// <summary>
        /// Attaches the dialog result to a button to be called on the owner window.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.WindowBehavior.DialogResult value.</param>
        public static void SetDialogResult(DependencyObject obj, bool? value)
        {
            obj.SetValue(DialogResultProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.GetDialogResult(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.SetDialogResult(DependencyObject, bool?)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.RegisterAttached("DialogResult", typeof(bool?), typeof(WindowBehavior), new UIPropertyMetadata(OnDialogResultChanged));

        /// <summary>
        /// Gets the dialog result command from a button to get the dialog result called on the owner window.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.WindowBehavior.DialogResultCommand property value for the element.</returns>
        public static ICommand GetDialogResultCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DialogResultCommandProperty);
        }

        /// <summary>
        /// Attaches the dialog result command to a button to get the dialog result called on the owner window.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.WindowBehavior.DialogResultCommand value.</param>
        public static void SetDialogResultCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DialogResultCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.GetDialogResultCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.SetDialogResultCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty DialogResultCommandProperty =
            DependencyProperty.RegisterAttached("DialogResultCommand", typeof(ICommand), typeof(WindowBehavior), new UIPropertyMetadata(OnDialogResultChanged));

        private static void OnDialogResultChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var button = sender as ButtonBase;
            if (button == null)
                throw new InvalidOperationException("'WindowBehavior.DialogResultCommand' only can be attached to a 'ButtonBase' object");

            button.Click += DialogResultButton_Click;
        }

        private static void DialogResultButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var window = VisualTreeAssist.FindParent<Window>(button);
            if (window != null)
            {
                var resultCommand = GetDialogResultCommand(button);
                if (resultCommand != null)
                {
                    var args = new WindowClosingArgs();
                    resultCommand.Execute(args);
                    if (!args.Cancel)
                        window.DialogResult = args.DialogResult;
                }
                else
                    window.DialogResult = GetDialogResult(button);
            }
        }
        #endregion DialogResult

        #region ClosingCommand
        /// <summary>
        /// Gets the command from a window which get called when the window closes. A WindowClosingArgs is passed as a parameter to change the dialog result and cancel the close.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.WindowBehavior.ClosingCommand property value for the element.</returns>
        public static ICommand GetClosingCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ClosingCommandProperty);
        }

        /// <summary>
        /// Attaches the command to a window which get called when the window closes. A WindowClosingArgs is passed as a parameter to change the dialog result and cancel the close.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.WindowBehavior.ClosingCommand value.</param>
        public static void SetClosingCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ClosingCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.GetClosingCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.SetClosingCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ClosingCommandProperty =
            DependencyProperty.RegisterAttached("ClosingCommand", typeof(ICommand), typeof(WindowBehavior), new UIPropertyMetadata(OnClosingCommandChanged));

        private static void OnClosingCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var window = sender as Window;
            if (window == null)
                throw new InvalidOperationException("'WindowBehavior.ClosingCommand' only can be attached to a 'Window' object");

            window.Closing += Window_Closing;
        }

        private static void Window_Closing(object sender, CancelEventArgs e)
        {
            var command = GetClosingCommand((DependencyObject)sender);
            if (command != null &&
                command.CanExecute(null))
            {
                var args = new WindowClosingArgs();
                command.Execute(args);
                e.Cancel = args.Cancel;
            }
        }
        #endregion ClosingCommand

        #region ClosedCommand
        /// <summary>
        /// Gets the command from a window which get called when the window has been closed. A WindowClosingArgs is passed as a parameter to change the dialog result and cancel the close.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.WindowBehavior.ClosedCommand property value for the element.</returns>
        public static ICommand GetClosedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ClosedCommandProperty);
        }

        /// <summary>
        /// Attaches the command to a window which get called when the window closes.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.WindowBehavior.ClosingCommand value.</param>
        public static void SetClosedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ClosedCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.GetClosedCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.SetClosedCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ClosedCommandProperty =
            DependencyProperty.RegisterAttached("ClosedCommand", typeof(ICommand), typeof(WindowBehavior), new UIPropertyMetadata(OnClosedCommandChanged));

        private static void OnClosedCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var window = sender as Window;
            if (window == null)
                throw new InvalidOperationException("'WindowBehavior.ClosedCommand' only can be attached to a 'Window' object");

            window.Closed += Window_Closed;
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            var command = GetClosedCommand((DependencyObject)sender);
            if (command != null &&
                command.CanExecute(null))
                command.Execute(null);
        }
        #endregion ClosedCommand

        #region LoadedCommand
        /// <summary>
        /// Gets the command from a window which get called when the window is loaded.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.WindowBehavior.LoadedCommand property value for the element.</returns>
        public static ICommand GetLoadedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LoadedCommandProperty);
        }

        /// <summary>
        /// Attaches the command to a window which get called when the window is loaded.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.WindowBehavior.LoadedCommand value.</param>
        public static void SetLoadedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadedCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.GetLoadedCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.SetLoadedCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty LoadedCommandProperty =
            DependencyProperty.RegisterAttached("LoadedCommand", typeof(ICommand), typeof(WindowBehavior), new UIPropertyMetadata(OnLoadedCommandChanged));

        /// <summary>
        /// Gets the command parameter from a window which is passed by the DW.WPFToolkit.Interactivity.WindowBehavior.LoadedCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.WindowBehavior.LoadedCommandParameter property value for the element.</returns>
        public static object GetLoadedCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(LoadedCommandParameterProperty);
        }

        /// <summary>
        /// Attaches the command parameter from a window which is passed by the DW.WPFToolkit.Interactivity.WindowBehavior.LoadedCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.WindowBehavior.LoadedCommandParameter value.</param>
        public static void SetLoadedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(LoadedCommandParameterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.GetLoadedCommandParameter(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.SetLoadedCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty LoadedCommandParameterProperty =
            DependencyProperty.RegisterAttached("LoadedCommandParameter", typeof(object), typeof(WindowBehavior), new UIPropertyMetadata(null));

        private static void OnLoadedCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var window = sender as FrameworkElement;
            if (window == null)
                throw new InvalidOperationException("'WindowBehavior.LoadedCommand' only can be attached to a 'FrameworkElement' object");

            window.Loaded += Window_Loaded;
        }

        private static void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var command = GetLoadedCommand((DependencyObject)sender);
            var parameter = GetLoadedCommandParameter((DependencyObject)sender);
            if (command != null &&
                command.CanExecute(parameter))
                command.Execute(parameter);
        }
        #endregion LoadedCommand

        #region IsClose
        /// <summary>
        /// Gets a value from a button that indicates that the window have to be closed when the button is pressed without using the dialog result.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.WindowBehavior.IsClose property value for the element.</returns>
        public static bool GetIsClose(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCloseProperty);
        }

        /// <summary>
        /// Attaches a value from a button that indicates that the window have to be closed when the button is pressed without using the dialog result.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.WindowBehavior.IsClose value.</param>
        public static void SetIsClose(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCloseProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.GetIsClose(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.SetIsClose(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty IsCloseProperty =
            DependencyProperty.RegisterAttached("IsClose", typeof(bool), typeof(WindowBehavior), new UIPropertyMetadata(OnIsCloseChanged));

        private static void OnIsCloseChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var button = sender as ButtonBase;
            if (button == null)
                throw new InvalidOperationException("'WindowBehavior.IsClose' only can be attached to a 'ButtonBase' object");

            button.Click += CloseButton_Click;
        }

        private static void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (GetIsClose((DependencyObject)sender))
            {
                var button = sender as Button;
                var window = VisualTreeAssist.FindParent<Window>(button);
                if (window != null)
                    window.Close();
            }
        }
        #endregion IsClose

        #region WinApiMessages
        /// <summary>
        /// Gets a list of hex values of a WinAPI messages to listen and forwarded to the DW.WPFToolkit.Interactivity.WindowBehavior.WinApiCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.WindowBehavior.WinApiMessages property value for the element.</returns>
        public static string GetWinApiMessages(DependencyObject obj)
        {
            return (string)obj.GetValue(WinApiMessagesProperty);
        }

        /// <summary>
        /// Attaches a list of hex values of a WinAPI messages to listen and forwarded to the DW.WPFToolkit.Interactivity.WindowBehavior.WinApiCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.WindowBehavior.WinApiMessages value.</param>
        public static void SetWinApiMessages(DependencyObject obj, string value)
        {
            obj.SetValue(WinApiMessagesProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.GetWinApiMessages(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.SetWinApiMessages(DependencyObject, string)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty WinApiMessagesProperty =
            DependencyProperty.RegisterAttached("WinApiMessages", typeof(string), typeof(WindowBehavior), new UIPropertyMetadata(OnWinApiMessagesChanged));

        private static void OnWinApiMessagesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var messages = e.NewValue as string;
            if (string.IsNullOrWhiteSpace(messages))
                return;

            var observer = GetOrCreateObsever(sender);
            if (observer == null)
                return;

            observer.ClearCallbacks();

            if (messages.ToLower().Trim() == "all")
                observer.AddCallback(EventNotified);
            else
                foreach (var id in StringToIntegerList(messages))
                    observer.AddCallbackFor(id, EventNotified);
        }

        private static void EventNotified(NotifyEventArgs e)
        {
            var command = GetWinApiCommand(e.ObservedWindow);
            if (command != null &&
                command.CanExecute(e))
                command.Execute(e);
        }

        private static IEnumerable<int> StringToIntegerList(string messages)
        {
            var idTexts = messages.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var ids = new List<int>();
            foreach (var idText in idTexts)
            {
                try
                {
                    int id;
                    ids.Add(int.TryParse(idText, NumberStyles.HexNumber, new CultureInfo(1033), out id)
                                        ? id
                                        : Convert.ToInt32(idText, 16));
                }
                catch
                {
                    throw new ArgumentException("The attached WinApiMessages cannot be parsed to a list of integers. Supported are just integer numbers separated by a semicolon, e.g. '3;42' or hex values (base of 16) like '0x03;0x2A'. See message values in the 'DW.SharpTools\\DW.SharpTools\\WindowObserver\\WindowMessages.cs' or in the WinUser.h (Windows SDK; C:\\Program Files (x86)\\Microsoft SDKs\\Windows\\v7.0A\\Include\\WinUser.h)");
                }
            }
            return ids;
        }
        #endregion WinApiMessages

        #region WinApiCommand
        /// <summary>
        /// Gets a command which get called if one of the message attached by the DW.WPFToolkit.Interactivity.WindowBehavior.WinApiMessages occurs.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.WindowBehavior.WinApiCommand property value for the element.</returns>
        public static ICommand GetWinApiCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(WinApiCommandProperty);
        }

        /// <summary>
        /// Attaches a command which get called if one of the message attached by the DW.WPFToolkit.Interactivity.WindowBehavior.WinApiMessages occurs.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.WindowBehavior.WinApiCommand value.</param>
        public static void SetWinApiCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(WinApiCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.GetWinApiCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.WindowBehavior.SetWinApiCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty WinApiCommandProperty =
            DependencyProperty.RegisterAttached("WinApiCommand", typeof(ICommand), typeof(WindowBehavior), new UIPropertyMetadata(OnWinApiCommandChanged));

        private static void OnWinApiCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                GetOrCreateObsever(sender);
        }

        private static WindowObserver GetOrCreateObsever(DependencyObject sender)
        {
            var observer = GetObserver(sender);
            if (observer == null)
            {
                var window = sender as Window;
                if (window != null)
                {
                    observer = new WindowObserver(window);
                    SetObserver(sender, observer);
                }
            }

            return observer;
        }
        #endregion WinApiCommand

        #region Observer
        private static WindowObserver GetObserver(DependencyObject obj)
        {
            return (WindowObserver)obj.GetValue(ObserverProperty);
        }

        private static void SetObserver(DependencyObject obj, WindowObserver value)
        {
            obj.SetValue(ObserverProperty, value);
        }

        private static readonly DependencyProperty ObserverProperty =
            DependencyProperty.RegisterAttached("Observer", typeof(WindowObserver), typeof(WindowBehavior), new UIPropertyMetadata(null));
        #endregion Observer
    }
}
