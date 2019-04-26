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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DW.WPFToolkit.Interactivity
{
    /// <summary>
    /// Gives you some commands when clicking in an ItemsControl or its items.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <ListBox 
    ///          Interactivity:ListBehavior.ItemDoubleClickedCommand="{Binding ItemDoubleClickedCommand}"
    ///          
    ///          Interactivity:ListBehavior.ItemClickedCommand="{Binding ItemClickedCommand}"
    ///          
    ///          Interactivity:ListBehavior.EmptyAreaDoubleClickCommand="{Binding EmptyAreaDoubleClickCommand}"
    ///          Interactivity:ListBehavior.EmptyAreaDoubleClickCommandParameter="Parameter"
    ///          
    ///          Interactivity:ListBehavior.EmptyAreaClickCommand="{Binding EmptyAreaClickCommand}"
    ///          Interactivity:ListBehavior.EmptyAreaClickCommandParameter="Parameter"
    ///          
    ///          Interactivity:ListBehavior.AutoDeselect="True"
    ///          />
    /// ]]>
    /// </code>
    /// </example>
    public class ListBehavior : DependencyObject
    {
        #region ItemDoubleClicked
        /// <summary>
        /// Gets the command which will be called when an items in a list gets double clicked.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ListBehavior.ItemDoubleClickedCommand property value for the element.</returns>
        /// <remarks>If the ItemDoubleClickedCommandParameter is not set, the double clicked item will be passed with the command.</remarks>
        public static ICommand GetItemDoubleClickedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ItemDoubleClickedCommandProperty);
        }

        /// <summary>
        /// Attaches the command to be called when an items in a list gets double clicked.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ListBehavior.ItemDoubleClickedCommand value.</param>
        public static void SetItemDoubleClickedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ItemDoubleClickedCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ListBehavior.GetItemDoubleClickedCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ListBehavior.SetItemDoubleClickedCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ItemDoubleClickedCommandProperty =
            DependencyProperty.RegisterAttached("ItemDoubleClickedCommand", typeof(ICommand), typeof(ListBehavior), new PropertyMetadata(null, OnItemDoubleClickedCommandChanged));

        /// <summary>
        /// Gets the command parameter which will be passed with the ItemDoubleClickedCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ListBehavior.ItemDoubleClickedCommandParameter property value for the element.</returns>
        public static object GetItemDoubleClickedCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(ItemDoubleClickedCommandParameterProperty);
        }

        /// <summary>
        /// Attaches the command parameter to be passed with the ItemDoubleClickedCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ListBehavior.ItemDoubleClickedCommandParameter value.</param>
        public static void SetItemDoubleClickedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ItemDoubleClickedCommandParameterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ListBehavior.GetItemDoubleClickedCommandParameter(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ListBehavior.SetItemDoubleClickedCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ItemDoubleClickedCommandParameterProperty =
            DependencyProperty.RegisterAttached("ItemDoubleClickedCommandParameter", typeof(object), typeof(ListBehavior), new PropertyMetadata(null));

        private static void OnItemDoubleClickedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = d as ItemsControl;
            if (list == null)
                return;

            if (e.NewValue != null)
                list.MouseDoubleClick -= HandleItemMouseDoubleClick;

            if (e.NewValue != null)
                list.MouseDoubleClick += HandleItemMouseDoubleClick;
        }
        #endregion ItemDoubleClicked

        #region ItemClicked
        /// <summary>
        /// Gets the command which will be called when an items in a list gets single clicked.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ListBehavior.ItemClickedCommand property value for the element.</returns>
        /// <remarks>If the ItemClickedCommandParameter is not set, the clicked item will be passed with the command.</remarks>
        public static ICommand GetItemClickedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(ItemClickedCommandProperty);
        }

        /// <summary>
        /// Attaches the command to be called when an items in a list gets single clicked.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ListBehavior.ItemClickedCommand value.</param>
        public static void SetItemClickedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(ItemClickedCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ListBehavior.GetItemClickedCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ListBehavior.SetItemClickedCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ItemClickedCommandProperty =
            DependencyProperty.RegisterAttached("ItemClickedCommand", typeof(ICommand), typeof(ListBehavior), new PropertyMetadata(null, OnItemClickedCommandChanged));

        /// <summary>
        /// Gets the command parameter which will be passed with the ItemClickedCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ListBehavior.ItemClickedCommandParameter property value for the element.</returns>
        public static object GetItemClickedCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(ItemClickedCommandParameterProperty);
        }

        /// <summary>
        /// Attaches the command parameter to be passed with the ItemClickedCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ListBehavior.ItemClickedCommandParameter value.</param>
        public static void SetItemClickedCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(ItemClickedCommandParameterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ListBehavior.GetItemClickedCommandParameter(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ListBehavior.SetItemClickedCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ItemClickedCommandParameterProperty =
            DependencyProperty.RegisterAttached("ItemClickedCommandParameter", typeof(object), typeof(ListBehavior), new PropertyMetadata(null));

        private static void OnItemClickedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = d as ItemsControl;
            if (list == null)
                return;

            if (e.NewValue != null)
                list.MouseLeftButtonUp -= HandleItemMouseClick;

            if (e.NewValue != null)
                list.MouseLeftButtonUp += HandleItemMouseClick;
        }
        #endregion ItemClicked

        #region EmptyAreaDoubleClick
        /// <summary>
        /// Gets the command which will be called when the area beside the items in a list gets double clicked.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ListBehavior.EmptyAreaDoubleClickCommand property value for the element.</returns>
        public static ICommand GetEmptyAreaDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(EmptyAreaDoubleClickCommandProperty);
        }

        /// <summary>
        /// Attaches the command to be called when the area beside the items in a list gets double clicked.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ListBehavior.EmptyAreaDoubleClickCommand value.</param>
        public static void SetEmptyAreaDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(EmptyAreaDoubleClickCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ListBehavior.GetEmptyAreaDoubleClickCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ListBehavior.SetEmptyAreaDoubleClickCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty EmptyAreaDoubleClickCommandProperty =
            DependencyProperty.RegisterAttached("EmptyAreaDoubleClickCommand", typeof(ICommand), typeof(ListBehavior), new PropertyMetadata(null, OnEmptyAreaDoubleClickCommandChanged));

        /// <summary>
        /// Gets the command parameter which will be passed with the EmptyAreaDoubleClickCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ListBehavior.EmptyAreaDoubleClickCommandParameter property value for the element.</returns>
        public static object GetEmptyAreaDoubleClickCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(EmptyAreaDoubleClickCommandParameterProperty);
        }

        /// <summary>
        /// Attaches the command parameter to be passed with the EmptyAreaDoubleClickCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ListBehavior.EmptyAreaDoubleClickCommandParameter value.</param>
        public static void SetEmptyAreaDoubleClickCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(EmptyAreaDoubleClickCommandParameterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ListBehavior.GetEmptyAreaDoubleClickCommandParameter(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ListBehavior.SetEmptyAreaDoubleClickCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty EmptyAreaDoubleClickCommandParameterProperty =
            DependencyProperty.RegisterAttached("EmptyAreaDoubleClickCommandParameter", typeof(object), typeof(ListBehavior), new PropertyMetadata(null));

        private static void OnEmptyAreaDoubleClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = d as ItemsControl;
            if (list == null)
                return;

            if (e.NewValue != null)
                list.MouseDoubleClick -= HandleEmptyAreaMouseDoubleClick;

            if (e.NewValue != null)
                list.MouseDoubleClick += HandleEmptyAreaMouseDoubleClick;
        }
        #endregion EmptyAreaDoubleClick

        #region EmptyAreaClick
        /// <summary>
        /// Gets the command which will be called when the area beside the items in a list gets single clicked.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ListBehavior.EmptyAreaDoubleClickCommand property value for the element.</returns>
        public static ICommand GetEmptyAreaClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(EmptyAreaClickCommandProperty);
        }

        /// <summary>
        /// Attaches the command to be called when the area beside the items in a list gets single clicked.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ListBehavior.EmptyAreaClickCommand value.</param>
        public static void SetEmptyAreaClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(EmptyAreaClickCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ListBehavior.GetEmptyAreaClickCommand(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ListBehavior.SetEmptyAreaClickCommand(DependencyObject, ICommand)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty EmptyAreaClickCommandProperty =
            DependencyProperty.RegisterAttached("EmptyAreaClickCommand", typeof(ICommand), typeof(ListBehavior), new PropertyMetadata(null, OnEmptyAreaClickCommandChanged));

        /// <summary>
        /// Gets the command parameter which will be passed with the EmptyAreaClickCommand.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ListBehavior.EmptyAreaClickCommandParameter property value for the element.</returns>
        public static object GetEmptyAreaClickCommandParameter(DependencyObject obj)
        {
            return (object)obj.GetValue(EmptyAreaClickCommandParameterProperty);
        }

        /// <summary>
        /// Attaches the command parameter to be passed with the EmptyAreaClickCommand.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ListBehavior.EmptyAreaClickCommandParameter value.</param>
        public static void SetEmptyAreaClickCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(EmptyAreaClickCommandParameterProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ListBehavior.GetEmptyAreaClickCommandParameter(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ListBehavior.SetEmptyAreaClickCommandParameter(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty EmptyAreaClickCommandParameterProperty =
            DependencyProperty.RegisterAttached("EmptyAreaClickCommandParameter", typeof(object), typeof(ListBehavior), new PropertyMetadata(null));

        private static void OnEmptyAreaClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = d as ItemsControl;
            if (list == null)
                return;

            if (e.NewValue != null)
                list.MouseLeftButtonUp -= HandleEmptyAreaMouseClick;

            if (e.NewValue != null)
                list.MouseLeftButtonUp += HandleEmptyAreaMouseClick;
        }
        #endregion EmptyAreaClick

        #region AutoDeselect
        /// <summary>
        /// Gets the value which indicates if the items should be deselected automatically when the area beside the items got single clicked.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ListBehavior.AutoDeselect property value for the element.</returns>
        public static bool GetAutoDeselect(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoDeselectProperty);
        }

        /// <summary>
        /// Attaches a value which indicates if the items should be deselected automatically when the area beside the items got single clicked.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ListBehavior.AutoDeselect value.</param>
        public static void SetAutoDeselect(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoDeselectProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ListBehavior.GetAutoDeselect(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ListBehavior.SetAutoDeselect(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty AutoDeselectProperty =
            DependencyProperty.RegisterAttached("AutoDeselect", typeof(bool), typeof(ListBehavior), new PropertyMetadata(false, OnAutoDeselectChanged));

        private static void OnAutoDeselectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var list = d as ItemsControl;
            if (list == null)
                return;

            if ((bool)e.OldValue)
                list.MouseLeftButtonUp -= HandleAutoDeselectClick;

            if ((bool)e.NewValue)
                list.MouseLeftButtonUp += HandleAutoDeselectClick;
        }
        #endregion AutoDeselect

        private static void HandleItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var data = GetData(sender, e);
            if (data == null || data.ItemContainer == null)
                return;

            var command = GetItemDoubleClickedCommand(data.List);
            var parameter = GetItemDoubleClickedCommandParameter(data.List) ?? data.ItemContainer.DataContext;

            Invoke(command, parameter);
        }

        private static void HandleItemMouseClick(object sender, MouseButtonEventArgs e)
        {
            var data = GetData(sender, e);
            if (data == null || data.ItemContainer == null)
                return;

            var command = GetItemClickedCommand(data.List);
            var parameter = GetItemClickedCommandParameter(data.List) ?? data.ItemContainer.DataContext;

            Invoke(command, parameter);
        }

        private static void HandleEmptyAreaMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var data = GetData(sender, e);
            if (data == null || data.ItemContainer != null)
                return;

            var command = GetEmptyAreaDoubleClickCommand(data.List);
            var parameter = GetEmptyAreaDoubleClickCommandParameter(data.List);

            Invoke(command, parameter);
        }

        private static void HandleEmptyAreaMouseClick(object sender, MouseButtonEventArgs e)
        {
            var data = GetData(sender, e);
            if (data == null || data.ItemContainer != null)
                return;

            var command = GetEmptyAreaClickCommand(data.List);
            var parameter = GetEmptyAreaClickCommandParameter(data.List);

            Invoke(command, parameter);
        }

        private static void HandleAutoDeselectClick(object sender, MouseButtonEventArgs e)
        {
            var list = sender as Selector;
            if (list == null)
                return;

            var itemContainer = list.ContainerFromElement((DependencyObject)e.OriginalSource) as FrameworkElement;
            if (itemContainer != null)
                return;

            list.SelectedIndex = -1;
        }

        private static Data GetData(object sender, MouseButtonEventArgs e)
        {
            var list = sender as ItemsControl;
            if (list == null)
                return null;

            return new Data(list, list.ContainerFromElement((DependencyObject)e.OriginalSource) as FrameworkElement);
        }

        private static void Invoke(ICommand command, object parameter)
        {
            if (command != null && command.CanExecute(parameter))
                command.Execute(parameter);
        }

        private class Data
        {
            public Data(DependencyObject list, FrameworkElement itemContainer)
            {
                List = list;
                ItemContainer = itemContainer;
            }

            public DependencyObject List { get; set; }
            public FrameworkElement ItemContainer { get; set; }
        }
    }
}
