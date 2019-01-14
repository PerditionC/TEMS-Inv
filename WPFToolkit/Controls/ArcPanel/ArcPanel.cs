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
using System.Windows.Media;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Arranges child elements in a configurable arc form.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <ItemsControl ItemsSource="{Binding Cards}">
    ///     <ItemsControl.ItemsPanel>
    ///         <ItemsPanelTemplate>
    ///             <WPFToolkit:ArcPanel Width="210" Height="100" Bend="3" />
    ///         </ItemsPanelTemplate>
    ///     </ItemsControl.ItemsPanel>
    /// </ItemsControl>
    /// 
    /// <WPFToolkit:ArcPanel Width="300" Height="150" Bend="2">
    ///     <Button Content="First" />
    ///     <Button Content="Second" />
    ///     <Button Content="Third" />
    /// </WPFToolkit:ArcPanel>
    /// ]]>
    /// </code>
    /// </example>
    public class ArcPanel : Panel
    {
        private readonly PathFigure _figure = new PathFigure();

        /// <summary>
        /// Gets or sets a value that defines if the child elements has to be rotated by the arc line.
        /// </summary>
        [DefaultValue(true)]
        public bool RotateElements
        {
            get { return (bool)GetValue(RotateElementsProperty); }
            set { SetValue(RotateElementsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ArcPanel.RotateElements" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateElementsProperty =
            DependencyProperty.Register("RotateElements", typeof(bool), typeof(ArcPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Gets or sets a value that defines how strong the arc line should be bended.
        /// </summary>
        [DefaultValue(1.0)]
        public double Bend
        {
            get { return (double)GetValue(BendProperty); }
            set { SetValue(BendProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ArcPanel.Bend" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty BendProperty =
            DependencyProperty.Register("Bend", typeof(double), typeof(ArcPanel), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.ArcPanel" /> class.
        /// </summary>
        public ArcPanel()
        {
            _figure.StartPoint = new Point(0, 10);
            _figure.Segments.Add(new ArcSegment(new Point(200, 10), new Size(300, 150), 0, false, SweepDirection.Clockwise, false));
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
        /// Positionates each child in an arc line form depending on the Bend and amount of child controls.
        /// </summary>
        /// <param name="finalSize">The maximum possible space given by the parent control.</param>
        /// <returns>The calculated needed space in sum of all available child controls.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (InternalChildren.Count <= 0 || !(finalSize.Width > 0))
                return base.ArrangeOverride(finalSize);

            ResetArc(finalSize);

            var pathLength = PathCalculator.GetPathFigureLength(_figure);
            var pathGeometry = new PathGeometry(new PathFigure[] { _figure });
            var distance = (double)1 / (double)(InternalChildren.Count + 1);
            var position = distance;
            foreach (UIElement child in InternalChildren)
            {
                Point point, tangent;
                pathGeometry.GetPointAtFractionLength(position, out point, out tangent);

                Size childSize = child.DesiredSize;
                point.X -= childSize.Width / 2;
                point.Y -= childSize.Height;
                child.Arrange(new Rect(point, childSize));

                if (RotateElements)
                {
                    var elementCenter = new Size(childSize.Width / 2, childSize.Height / 2);
                    var transforms = new TransformGroup();
                    transforms.Children.Add(new RotateTransform((Math.Atan2(tangent.Y, tangent.X)
                                                                 * 180
                                                                 / Math.PI),
                        elementCenter.Width,
                        elementCenter.Height));
                    child.RenderTransform = transforms;
                }
                else
                    child.RenderTransform = null;
                    
                position += distance;
            }
            return base.ArrangeOverride(finalSize);
        }

        private void ResetArc(Size finalSize)
        {
            var segment = (ArcSegment)_figure.Segments[0];
            _figure.StartPoint = new Point(0, finalSize.Height);
            segment.Point = new Point(finalSize.Width, finalSize.Height);
            segment.Size = new Size(finalSize.Width, finalSize.Height * Bend);
        }
    }
}
