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
using System.Windows;
using System.Windows.Input;

namespace DW.WPFToolkit.Interactivity
{
    /// <summary>
    /// Brings the feature to be able to double click any UI element.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <TextBlock Text="Doubleclick Me"
    ///            Interactivity:DoubleClickBehavior.Command="{Binding ItemDoubleClicked}"
    ///            Interactivity:DoubleClickBehavior.CommandParameter="Parameter" />
    /// ]]>
    /// </code>
    /// </example>
    public class DoubleClickBehavior :  DependencyObject
    {
        /// <summary>
        /// Gets the command to be called when the element gets double clicked.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.DoubleClickBehavior.Command property value for the element.</returns>
        public static ICommand GetCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        /// <summary>
        /// Attaches the command to be called when the element gets double clicked.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.DoubleClickBehavior.Command value.</param>
        public static void SetCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.DoubleClickBehavior.GetCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.DoubleClickBehavior.SetCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(DoubleClickBehavior), new UIPropertyMetadata(OnCommandChanged));

        /// <summary>
        /// Gets the command parameter to be passed when the called when DW.WPFToolkit.Interactivity.DoubleClickBehavior.Command gets called.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.DoubleClickBehavior.CommandParameter property value for the element.</returns>
        public static object GetCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Attaches the command parameter to be passed when the called when DW.WPFToolkit.Interactivity.DoubleClickBehavior.Command gets called.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.DoubleClickBehavior.CommandParameter value.</param>
        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.DoubleClickBehavior.GetCommandParameter(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.DoubleClickBehavior.SetCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(DoubleClickBehavior), new UIPropertyMetadata(null));

        private static void OnCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as FrameworkElement;
            if (control == null)
                throw new InvalidOperationException("The DoubleClickBehavior can only attached to an FrameworkElement");

            control.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MouseButtonDown);
        }

        private static void MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var command = GetCommand((DependencyObject)sender);
                var parameter = GetCommandParameter((DependencyObject)sender);
                if (parameter == null)
                {
                    var owner = sender as FrameworkElement;
                    parameter = owner.DataContext;
                }
                if (command != null &&
                    command.CanExecute(parameter))
                    command.Execute(parameter);
            }
        }
    }
}
