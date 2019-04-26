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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Enhances the <see cref="System.Windows.Controls.WrapPanel" /> by the feature that all items will have the same size.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <ItemsControl ItemsSource="{Binding Images}">
    ///     <ItemsControl.ItemsPanel>
    ///         <ItemsPanelTemplate>
    ///             <WPFToolkit:UniformWrapPanel />
    ///         </ItemsPanelTemplate>
    ///     </ItemsControl.ItemsPanel>
    /// </ItemsControl>
    /// 
    /// <WPFToolkit:UniformWrapPanel Orientation="Horizontal" MinItemWidth="100">
    ///     <Button Content="One" />
    ///     <Button Content="Two" />
    ///     <Button Content="Three" />
    /// </WPFToolkit:UniformWrapPanel>
    /// ]]>
    /// </code>
    /// </example>
    public class UniformWrapPanel : WrapPanel
    {
        /// <summary>
        /// Gets or sets a value that defines if the common height or width will be taken by the biggest child element.
        /// </summary>
        [DefaultValue(true)]
        public bool IsAutoUniform
        {
            get { return (bool)GetValue(IsAutoUniformProperty); }
            set { SetValue(IsAutoUniformProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.UniformWrapPanel.IsAutoUniform" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAutoUniformProperty =
            DependencyProperty.Register("IsAutoUniform", typeof(bool), typeof(UniformWrapPanel), new UIPropertyMetadata(true, IsAutoUniformChanged));

        private static void IsAutoUniformChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var panel = sender as UniformWrapPanel;
            if (panel != null)
                panel.InvalidateVisual();
        }

        /// <summary>
        /// Gets or sets the minimum width of the items.
        /// </summary>
        [DefaultValue(0.0)]
        public double MinItemWidth
        {
            get { return (double)GetValue(MinItemWidthProperty); }
            set { SetValue(MinItemWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.UniformWrapPanel.MinItemWidth" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinItemWidthProperty =
            DependencyProperty.Register("MinItemWidth", typeof(double), typeof(UniformWrapPanel), new UIPropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the minimum height of the items.
        /// </summary>
        [DefaultValue(0.0)]
        public double MinItemHeight
        {
            get { return (double)GetValue(MinItemHeightProperty); }
            set { SetValue(MinItemHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.UniformWrapPanel.MinItemHeight" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinItemHeightProperty =
            DependencyProperty.Register("MinItemHeight", typeof(double), typeof(UniformWrapPanel), new UIPropertyMetadata(0.0));

        /// <summary>
        /// Lets each child calculating is needed size.
        /// </summary>
        /// <param name="availableSize">The available space by the parent control.</param>
        /// <returns>The calculated size needed for the control.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count > 0 &&
                IsAutoUniform)
            {
                foreach (UIElement el in Children)
                {
                    el.Measure(availableSize);
                    if (Orientation == Orientation.Vertical)
                        ItemHeight = MeasureItem(ItemHeight, el.DesiredSize.Height, MinItemHeight);
                    else
                        ItemWidth = MeasureItem(ItemWidth, el.DesiredSize.Width, MinItemWidth);
                }
            }
            return base.MeasureOverride(availableSize);
        }

        private double MeasureItem(double finalItemSize, double currentItemSize, double minimumSize)
        {
            if (double.IsNaN(finalItemSize))
                finalItemSize = 0;
            if (!(double.IsInfinity(currentItemSize) ||
                  double.IsNaN(currentItemSize)))
            {
                finalItemSize = Math.Max(finalItemSize, currentItemSize);
                if (finalItemSize < minimumSize)
                    finalItemSize = minimumSize;
            }
            return finalItemSize;
        }
    }
}
