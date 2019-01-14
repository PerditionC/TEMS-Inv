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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Brings an easy to use MenuItem to be used in the ContextMenu no matter if its in an own VisualTree or not.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <TextBlock Text="{Binding Name}">
    ///     <TextBlock.ContextMenu>
    ///         <ContextMenu>
    ///             <Toolkit:ContextMenuItem Header="Edit Global"
    ///                                      Command="{Binding EditCommand}"
    ///                                      CommandParameter="{Binding ItemDataContext, RelativeSource={RelativeSource Self}}" />
    ///             <Toolkit:ContextMenuItem Header="Edit Directly"
    ///                                      IsBindToSelf="True"
    ///                                      Command="{Binding EditCommand}" />
    ///         </ContextMenu>
    ///     </TextBlock.ContextMenu>
    /// </TextBlock>
    /// ]]>
    /// </code>
    /// </example>
    public class ContextMenuItem : MenuItem
    {
        /// <summary>
        /// Called when the <see cref="System.Windows.FrameworkElement.IsInitialized" /> property is set to true.
        /// </summary>
        /// <param name="e">The event data for the <see cref="System.Windows.FrameworkElement.IsInitialized" /> event.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            var binding = new Binding();
            binding.Path = new PropertyPath("PlacementTarget");
            binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(ContextMenu), 1);
            SetBinding(ElementHolderProperty, binding);
        }

        /// <summary>
        /// Gets or sets the data as original DataContext for the case the DataContext get changed to the parent object.
        /// </summary>
        [DefaultValue(null)]
        public object ItemDataContext
        {
            get { return (object)GetValue(ItemDataContextProperty); }
            set { SetValue(ItemDataContextProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ContextMenuItem.ItemDataContext" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemDataContextProperty =
            DependencyProperty.Register("ItemDataContext", typeof(object), typeof(ContextMenuItem), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a placeholder for the current item parent from the other visual tree. This is used internally, please do not set it to another value otherwise the ContextMenuItem will not work properly.
        /// </summary>
        public object ElementHolder
        {
            get { return (object)GetValue(ElementHolderProperty); }
            set { SetValue(ElementHolderProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ContextMenuItem.ElementHolder" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementHolderProperty =
            DependencyProperty.Register("ElementHolder", typeof(object), typeof(ContextMenuItem), new UIPropertyMetadata(OnElementHolderChanged));

        /// <summary>
        /// The DataContext will be the element like original.
        /// </summary>
        [DefaultValue(false)]
        public bool IsBindToSelf
        {
            get { return (bool)GetValue(IsBindToSelfProperty); }
            set { SetValue(IsBindToSelfProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ContextMenuItem.IsBindToSelf" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBindToSelfProperty =
            DependencyProperty.Register("IsBindToSelf", typeof(bool), typeof(ContextMenuItem), new UIPropertyMetadata(false));

        private static void OnElementHolderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var control = (ContextMenuItem)d;
                var parent = VisualTreeHelper.GetParent(e.NewValue as DependencyObject);
                while (parent != null)
                {
                    if (parent is Window ||
                        parent is UserControl)
                        break;

                    if (control.ItemDataContext == null &&
                        parent is FrameworkElement)
                        control.ItemDataContext = ((FrameworkElement)parent).DataContext;

                    parent = VisualTreeHelper.GetParent(parent);
                }
                if (parent != null)
                    control.DataContext = ((FrameworkElement)parent).DataContext;

                if (control.IsBindToSelf)
                    control.DataContext = control.ItemDataContext;
            }
        }
    }
}
