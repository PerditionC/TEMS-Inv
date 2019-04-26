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
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace DW.WPFToolkit.Interactivity
{
    /// <summary>
    /// Brings the feature to modify the scroll position of an items control.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <ListBox ItemsSource="{Binding LogEntries}"
    ///          Interactivity:ScrollBehavior.AutoScrollToLast="True" />
    /// 
    /// <ListBox ItemsSource="{Binding LogEntries}"
    ///          Interactivity:ScrollBehavior.ScrollToItem="{Binding ImportantEntry}" />
    /// ]]>
    /// </code>
    /// </example>
    public class ScrollBehavior : DependencyObject
    {
        /// <summary>
        /// Gets the item to which it has to scroll in a list.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ScrollBehavior.ScrollToItem property value for the element.</returns>
        public static object GetScrollToItem(DependencyObject obj)
        {
            return (object)obj.GetValue(ScrollToItemProperty);
        }

        /// <summary>
        /// Attaches the item to which it has to scroll in a list.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ScrollBehavior.ScrollToItem value.</param>
        public static void SetScrollToItem(DependencyObject obj, object value)
        {
            obj.SetValue(ScrollToItemProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ScrollBehavior.GetScrollToItem(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ScrollBehavior.SetScrollToItem(DependencyObject, object)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ScrollToItemProperty =
            DependencyProperty.RegisterAttached("ScrollToItem", typeof(object), typeof(ScrollBehavior), new UIPropertyMetadata(OnScrollChanged));

        /// <summary>
        /// Gets a value that indicates if a list automaticaly have to scroll to the last item if the item collection changes.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ScrollBehavior.AutoScrollToLast property value for the element.</returns>
        public static bool GetAutoScrollToLast(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToLastProperty);
        }

        /// <summary>
        /// Attaches a value that indicates if a list automaticaly have to scroll to the last item if the item collection changes.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ScrollBehavior.AutoScrollToLast value.</param>
        public static void SetAutoScrollToLast(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToLastProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ScrollBehavior.GetAutoScrollToLast(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ScrollBehavior.SetAutoScrollToLast(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty AutoScrollToLastProperty =
            DependencyProperty.RegisterAttached("AutoScrollToLast", typeof(bool), typeof(ScrollBehavior), new UIPropertyMetadata(OnScrollChanged));

        /// <summary>
        /// Gets a value that indicates if a list automaticaly have to scroll to the selected item if the selection has been changed.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Interactivity.ScrollBehavior.AutoScrollToSelected property value for the element.</returns>
        public static bool GetAutoScrollToSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToSelectedProperty);
        }

        /// <summary>
        /// Attaches a value that indicates if a list automaticaly have to scroll to the selected item if the selection has been changed.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.ScrollBehavior.AutoScrollToSelected value.</param>
        public static void SetAutoScrollToSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToSelectedProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.ScrollBehavior.GetAutoScrollToSelected(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.ScrollBehavior.SetAutoScrollToSelected(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty AutoScrollToSelectedProperty =
            DependencyProperty.RegisterAttached("AutoScrollToSelected", typeof(bool), typeof(ScrollBehavior), new UIPropertyMetadata(OnScrollChanged));

        private static ScrollBehavior GetScrollBehavior(DependencyObject obj)
        {
            return (ScrollBehavior)obj.GetValue(ScrollBehaviorProperty);
        }

        private static void SetScrollBehavior(DependencyObject obj, ScrollBehavior value)
        {
            obj.SetValue(ScrollBehaviorProperty, value);
        }

        private static readonly DependencyProperty ScrollBehaviorProperty =
            DependencyProperty.RegisterAttached("ScrollBehavior", typeof(ScrollBehavior), typeof(ScrollBehavior), new UIPropertyMetadata(null));

        private static void OnScrollChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var listbox = sender as ListBox;
            if (listbox == null)
                throw new InvalidOperationException("The ScrollBehavior.ScrollToItem only can be attached to an ListBox or ListView");

            var scrollBehavior = GetScrollBehavior(sender);
            if (scrollBehavior == null)
            {
                scrollBehavior = new ScrollBehavior();
                SetScrollBehavior(sender, scrollBehavior);
                listbox.Loaded += new RoutedEventHandler(scrollBehavior.Listbox_Loaded);
            }
            scrollBehavior.ScrollToItem(listbox, GetScrollToItem(listbox));
        }

        private void Listbox_Loaded(object sender, RoutedEventArgs e)
        {
            var listbox = sender as ListBox;

            var autoSelectionScroll = GetAutoScrollToSelected(listbox);
            var autoLastScroll = GetAutoScrollToLast(listbox);

            ScrollToItem(listbox, GetScrollToItem(listbox));
            if (autoLastScroll)
            {
                var collection = listbox.ItemsSource as INotifyCollectionChanged;
                if (collection != null)
                    collection.CollectionChanged += (owner, arg) => { ScrollToLast(listbox); };
                ScrollToLast(listbox);
            }
            if (autoSelectionScroll)
                listbox.SelectionChanged += new SelectionChangedEventHandler(ListBox_SelectionChanged);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listbox = sender as ListBox;

            if (listbox.SelectedItem != null)
                listbox.ScrollIntoView(listbox.SelectedItem);
        }

        private void ScrollToItem(ListBox listBox, object item)
        {
            if (item != null)
                listBox.ScrollIntoView(item);
        }

        private void ScrollToLast(ListBox listBox)
        {
            if (listBox.Items.Count > 0)
                listBox.ScrollIntoView(listBox.Items[listBox.Items.Count - 1]);
        }
    }
}
