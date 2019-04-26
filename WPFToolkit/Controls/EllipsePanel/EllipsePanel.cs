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
using System.Windows.Media;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Arranges child elements in a configurable ellipse form.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <ItemsControl ItemsSource="{Binding Player}">
    ///     <ItemsControl.ItemsPanel>
    ///         <ItemsPanelTemplate>
    ///             <WPFToolkit:EllipsePanel ElementStartPosition="Bottom"
    ///                                      EllipseRotateDirection="Clockwise"
    ///                                      ElementsRotateDirection="Outroversive"
    ///                                      RotateElements="True" />
    ///         </ItemsPanelTemplate>
    ///     </ItemsControl.ItemsPanel>
    /// </ItemsControl>
    /// 
    /// <WPFToolkit:EllipsePanel>
    ///     <Button Content="One" />
    ///     <Button Content="Two" />
    ///     <Button Content="Three" />
    ///     <Button Content="Four" />
    /// </WPFToolkit:EllipsePanel>
    /// ]]>
    /// </code></example>
    public class EllipsePanel : Panel
    {
        private readonly EllipseGeometry _ellipse = new EllipseGeometry();

        /// <summary>
        /// Gets or sets if the child elements has to be rotated.
        /// </summary>
        [DefaultValue(false)]
        public bool RotateElements
        {
            get { return (bool)GetValue(RotateElementsProperty); }
            set { SetValue(RotateElementsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsePanel.RotateElements" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateElementsProperty =
            DependencyProperty.Register("RotateElements", typeof(bool), typeof(EllipsePanel), new PropertyMetadata(false, ValueChanged));

        /// <summary>
        /// Gets or sets the rotating direction of the child elements if <see cref="DW.WPFToolkit.Controls.EllipsePanel.RotateElements" /> is set to true.
        /// </summary>
        public ElementsRotateDirection ElementsRotateDirection
        {
            get { return (ElementsRotateDirection)GetValue(ElementsRotateDirectionProperty); }
            set { SetValue(ElementsRotateDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsePanel.ElementsRotateDirection" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementsRotateDirectionProperty =
            DependencyProperty.Register("ElementsRotateDirection", typeof(ElementsRotateDirection), typeof(EllipsePanel), new PropertyMetadata(ValueChanged));

        /// <summary>
        /// Gets or sets the direction where the child elements in the ellipse should be alligned.
        /// </summary>
        public SweepDirection EllipseRotateDirection
        {
            get { return (SweepDirection)GetValue(EllipseRotateDirectionProperty); }
            set { SetValue(EllipseRotateDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsePanel.EllipseRotateDirection" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty EllipseRotateDirectionProperty =
            DependencyProperty.Register("EllipseRotateDirection", typeof(SweepDirection), typeof(EllipsePanel), new PropertyMetadata(ValueChanged));

        /// <summary>
        /// Gets or sets where the first child element has to start in the ellipse.
        /// </summary>
        public ElementStartPosition ElementStartPosition
        {
            get { return (ElementStartPosition)GetValue(ElementStartPositionProperty); }
            set { SetValue(ElementStartPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsePanel.ElementStartPosition" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ElementStartPositionProperty =
            DependencyProperty.Register("ElementStartPosition", typeof(ElementStartPosition), typeof(EllipsePanel), new PropertyMetadata(ValueChanged));

        private static void ValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var control = (EllipsePanel)obj;
            control.InvalidateArrange();
        }

        /// <summary>
        /// Lets each child calculating is needed size.
        /// </summary>
        /// <param name="availableSize">The available space by the parent control.</param>
        /// <returns>The calculated size needed for the control.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
                child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Positionates each child in an ellipse form depending on the amount of child controls.
        /// </summary>
        /// <param name="finalSize">The maximum possible space given by the parent control.</param>
        /// <returns>The calculated needed space in sum of all available child controls.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (finalSize.Height <= 0 ||
                finalSize.Width <= 0)
                return finalSize;

            if (InternalChildren.Count <= 0)
                return base.ArrangeOverride(finalSize);

            ResetEllipse(finalSize);

            var figure = _ellipse.GetOutlinedPathGeometry().Figures[0];
            var pathGeometry = new PathGeometry(new[] { figure });

            var points = new List<Point>();
            var tangents = new List<Point>();
            var distance = (double)1 / (double)InternalChildren.Count;
            var position = GetElementStartPositionValue();
            foreach (var child in InternalChildren)
            {
                Point point, tangent;
                pathGeometry.GetPointAtFractionLength(position, out point, out tangent);
                points.Add(point);
                tangents.Add(tangent);
                position += distance;
                if (position > 1)
                    position -= 1;
            }

            if (EllipseRotateDirection == SweepDirection.Counterclockwise)
            {
                points.Reverse();
                tangents.Reverse();
                var point = points.Last();
                points.Remove(point);
                points.Insert(0, point);

                var tangent = tangents.Last();
                tangents.Remove(tangent);
                tangents.Insert(0, tangent);
            }

            int pos = 0;
            foreach (UIElement child in InternalChildren)
            {
                var childSize = child.DesiredSize;
                var elementPos = points[pos];
                elementPos.X -= childSize.Width / 2;
                elementPos.Y -= childSize.Height / 2;
                child.SnapsToDevicePixels = true;
                child.Arrange(new Rect(elementPos, childSize));
                if (RotateElements)
                {
                    var elementCenter = new Size(childSize.Width / 2, childSize.Height / 2);
                    var transforms = new TransformGroup();

                    var centerPoint = new Point(finalSize.Width / 2, finalSize.Height / 2);
                    var angle = CalculatePileToPlayerAngle(elementPos, centerPoint);
                    transforms.Children.Add(new RotateTransform(angle, elementCenter.Width, elementCenter.Height));

                    //transforms.Children.Add(new RotateTransform((Math.Atan2(tangents[pos].Y, tangents[pos].X)
                    //                                                * 180
                    //                                                / Math.PI)
                    //                                                + GetElementsRotateDirectionValue(),
                    //                                            elementCenter.Width,
                    //                                            elementCenter.Height));
                    child.RenderTransform = transforms;
                }
                else
                    child.RenderTransform = null;
                ++pos;
            }
            return base.ArrangeOverride(finalSize);
        }

        private double CalculatePileToPlayerAngle(Point start, Point end)
        {
            var angle = 0.0;

            end.X = end.X - start.X;
            end.Y = end.Y - start.Y;
            angle = Math.Atan2(end.Y, end.X) / (Math.PI / 180);

            angle += 90;
            if (angle < 0)
                angle += 360;
            if (angle >= 360)
                angle %= 360;
            return angle;
        }

        private double GetElementStartPositionValue()
        {
            switch (ElementStartPosition)
            {
                case ElementStartPosition.Left:
                    return 0.75;
                case ElementStartPosition.Top:
                    return 0;
                case ElementStartPosition.Right:
                    return 0.25;
                case ElementStartPosition.Bottom:
                    return 0.5;
            }
            return 0;
        }

        private double GetElementsRotateDirectionValue()
        {
            switch (ElementsRotateDirection)
            {
                case ElementsRotateDirection.Introversive:
                    return 0;
                case ElementsRotateDirection.Outroversive:
                    return 180;
                case ElementsRotateDirection.Clockwise:
                    return 90;
                case ElementsRotateDirection.Counterclockwise:
                    return -90;
            }
            return 0;
        }

        private void ResetEllipse(Size finalSize)
        {
            _ellipse.RadiusX = finalSize.Width / 2;
            _ellipse.RadiusY = finalSize.Height / 2;
            _ellipse.Center = new Point(finalSize.Width / 2, finalSize.Height / 2);
        }
    }
}
