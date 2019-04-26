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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// The panel which is used in the <see cref="DW.WPFToolkit.Controls.NavigationBar" /> which arranges the <see cref="DW.WPFToolkit.Controls.NavigationBarItem" />s.
    /// </summary>
    public class NavigationBarPanel : Panel
    {
        /// <summary>
        /// Gets the value which indicates if a item hosted in the NavigationBarPanel is expanded or not.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>The DW.WPFToolkit.Controls.NavigationBarPanel.IsExpanded property value for the element.</returns>
        public static bool GetIsExpanded(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsExpandedProperty);
        }

        /// <summary>
        /// Attaches the value which indicates if a item hosted in the NavigationBarPanel is expanded or not.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Controls.NavigationBarPanel.IsExpanded value.</param>
        public static void SetIsExpanded(DependencyObject obj, bool value)
        {
            obj.SetValue(IsExpandedProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.NavigationBarPanel.GetIsExpanded(DependencyObject)" /> <see cref="DW.WPFToolkit.Controls.NavigationBarPanel.SetIsExpanded(DependencyObject, bool)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.RegisterAttached("IsExpanded", typeof(bool), typeof(NavigationBarPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the orientation of the NavigationBarPanel.
        /// </summary>
        [DefaultValue(Orientation.Vertical)]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.NavigationBarPanel.Orientation" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(NavigationBarPanel), new PropertyMetadata(Orientation.Vertical));

        /// <summary>
        /// Lets each child calculating is needed size.
        /// </summary>
        /// <param name="constraint">The available space by the parent control.</param>
        /// <returns>The calculated size needed for the control.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            var maximumWidth = 0.0;
            var maximumHeight = 0.0;
            var childWidths = 0.0;
            var childHeights = 0.0;

            var skippedObjects = new List<UIElement>();

            foreach (UIElement element in InternalChildren)
            {
                if (element == null)
                    continue;

                var availableSize = new Size(Math.Max(0.0, constraint.Width - childWidths),
                                             Math.Max(0.0, constraint.Height - childHeights));
                element.Measure(availableSize);

                if (GetIsExpanded(element))
                {
                    skippedObjects.Add(element);
                    continue;
                }

                var desiredSize = element.DesiredSize;
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        maximumHeight = Math.Max(maximumHeight, childHeights + desiredSize.Height);
                        childWidths += desiredSize.Width;
                        break;

                    case Orientation.Vertical:
                        maximumWidth = Math.Max(maximumWidth, childWidths + desiredSize.Width);
                        childHeights += desiredSize.Height;
                        break;
                }
            }

            var sizeLeft = new Size(Math.Max(0.0, constraint.Width - childWidths),
                                    Math.Max(0.0, constraint.Height - childHeights));
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    sizeLeft.Width /= Math.Max(skippedObjects.Count, 1.0);
                    break;
                case Orientation.Vertical:
                    sizeLeft.Height /= Math.Max(skippedObjects.Count, 1.0);
                    break;
            }

            foreach (var element in skippedObjects)
            {
                element.Measure(sizeLeft);
                if (Orientation == Orientation.Horizontal)
                    childWidths += element.DesiredSize.Width;
                else
                    childHeights += element.DesiredSize.Height;
            }

            return new Size(childWidths, childHeights);
        }

        /// <summary>
        /// Positionates each child in the available space of the NavigationBar depending on the expanding states.
        /// </summary>
        /// <param name="arrangeSize">The maximum possible space given by the parent control.</param>
        /// <returns>The calculated needed space in sum of all available child controls.</returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var sizes = new Size[InternalChildren.Count];
            for (var i = 0; i < InternalChildren.Count; ++i)
            {
                var element = InternalChildren[i];
                if (element == null)
                    continue;

                sizes[i] = element.DesiredSize;
                var isSharing = GetIsExpanded(element);
                switch (Orientation)
                {
                    case Orientation.Horizontal:
                        if (isSharing)
                            sizes[i].Width = 0;
                        sizes[i].Height = arrangeSize.Height;
                        break;
                    case Orientation.Vertical:
                        if (isSharing)
                            sizes[i].Height = 0;
                        sizes[i].Width = arrangeSize.Width;
                        break;
                }
            }

            var heightLeft = arrangeSize.Height;
            var widthLeft = arrangeSize.Width;
            for (var i = 0; i < InternalChildren.Count; ++i)
            {
                var element = InternalChildren[i];
                if (element == null)
                    continue;

                var isSharing = GetIsExpanded(element);
                if (!isSharing && Orientation == Orientation.Horizontal)
                    widthLeft -= sizes[i].Width;
                else if (!isSharing && Orientation == Orientation.Vertical)
                    heightLeft -= sizes[i].Height;
            }

            double heightItemsCout = sizes.Count(i => i.Height == 0);
            double widthItemsCout = sizes.Count(i => i.Width == 0);

            var missingItemHeight = heightLeft / Math.Max(heightItemsCout, 1);
            var missingItemWidth = widthLeft / Math.Max(widthItemsCout, 1);

            for (var i = 0; i < InternalChildren.Count; ++i)
            {
                var element = InternalChildren[i];
                if (element == null)
                    continue;

                if (sizes[i].Height == 0)
                    sizes[i].Height = missingItemHeight;
                if (sizes[i].Width == 0)
                    sizes[i].Width = missingItemWidth;
            }

            var x = 0.0;
            var y = 0.0;
            for (var i = 0; i < InternalChildren.Count; ++i)
            {
                var element = InternalChildren[i];
                if (element == null)
                    continue;

                var size = sizes[i];
                var position = new Rect(x, y, size.Width, size.Height);
                element.Arrange(position);

                if (Orientation == Orientation.Horizontal)
                    x += size.Width;
                else
                    y += size.Height;
            }

            return arrangeSize;
        }
    }
}