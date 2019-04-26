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
using System.Windows.Media;
using System.Windows.Shapes;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Represents a overlay surface to be placed in front of the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />.
    /// </summary>
    public class GappedOverlay : Control
    {
        static GappedOverlay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GappedOverlay), new FrameworkPropertyMetadata(typeof(GappedOverlay)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.GappedOverlay" /> class.
        /// </summary>
        public GappedOverlay()
        {
            _outherEllipse = new EllipseGeometry();
            _outherGapEllipse = new EllipseGeometry();
            _innerGapEllipse = new EllipseGeometry();
            _innerEllipse = new EllipseGeometry();

            Loaded += (sender, e) => CalculateGaps();
        }

        /// <summary>
        /// Gets or sets the radius of the outher ellipse of the whole GappedOverlay
        /// </summary>
        public double OutherRadius
        {
            get { return (double)GetValue(OutherRadiusProperty); }
            set { SetValue(OutherRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.GappedOverlay.OutherRadius" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutherRadiusProperty =
            DependencyProperty.Register("OutherRadius", typeof(double), typeof(GappedOverlay), new UIPropertyMetadata(OnOutherRadiusChanged));

        private static void OnOutherRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (GappedOverlay)sender;
            control.OnOutherRadiusChanged();
        }

        /// <summary>
        /// Gets or sets the radius of the outher ellipse. There the gaps will start.
        /// </summary>
        public double OutherGapRadius
        {
            get { return (double)GetValue(OutherGapRadiusProperty); }
            set { SetValue(OutherGapRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.GappedOverlay.OutherGapRadius" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutherGapRadiusProperty =
            DependencyProperty.Register("OutherGapRadius", typeof(double), typeof(GappedOverlay), new UIPropertyMetadata(OnOutherGapRadiusChanged));

        private static void OnOutherGapRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (GappedOverlay)sender;
            control.OnOutherGapRadiusChanged();
        }

        /// <summary>
        /// Gets or sets the radius of the inner ellipse. There the gaps will end.
        /// </summary>
        public double InnerGapRadius
        {
            get { return (double)GetValue(InnerGapRadiusProperty); }
            set { SetValue(InnerGapRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.GappedOverlay.InnerGapRadius" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerGapRadiusProperty =
            DependencyProperty.Register("InnerGapRadius", typeof(double), typeof(GappedOverlay), new UIPropertyMetadata(OnInnerGapRadiusChanged));

        private static void OnInnerGapRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (GappedOverlay)sender;
            control.OnInnerGapRadiusChanged();
        }

        /// <summary>
        /// Gets or sets the radius of the end point in center of the whole GappedOverlay.
        /// </summary>
        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.GappedOverlay.InnerRadius" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadius", typeof(double), typeof(GappedOverlay), new UIPropertyMetadata(OnInnerRadiusChanged));

        private static void OnInnerRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (GappedOverlay)sender;
            control.OnInnerRadiusChanged();
        }

        /// <summary>
        /// Gets or sets the distances between each gap.
        /// </summary>
        public double GapDistance
        {
            get { return (double)GetValue(GapDistanceProperty); }
            set { SetValue(GapDistanceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.GappedOverlay.GapDistance" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty GapDistanceProperty =
            DependencyProperty.Register("GapDistance", typeof(double), typeof(GappedOverlay), new UIPropertyMetadata(OnGapDistanceChanged));

        private static void OnGapDistanceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (GappedOverlay)sender;
            control.CalculateGaps();
        }

        /// <summary>
        /// Gets or sets the distance between the gaps and the other ellipse of the GappedOverlay.
        /// </summary>
        public double OutherDistance
        {
            get { return (double)GetValue(OutherDistanceProperty); }
            set { SetValue(OutherDistanceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.GappedOverlay.OutherDistance" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutherDistanceProperty =
            DependencyProperty.Register("OutherDistance", typeof(double), typeof(GappedOverlay), new UIPropertyMetadata(OnOutherDistanceChanged));

        private static void OnOutherDistanceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (GappedOverlay)sender;
            control.CalculateGaps();
        }

        /// <summary>
        /// Gets or sets the distance between the gaps and the inner ellipse of the center point in the GappedOverlay.
        /// </summary>
        public double InnerDistance
        {
            get { return (double)GetValue(InnerDistanceProperty); }
            set { SetValue(InnerDistanceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.GappedOverlay.InnerDistance" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerDistanceProperty =
            DependencyProperty.Register("InnerDistance", typeof(double), typeof(GappedOverlay), new UIPropertyMetadata(OnInnerDistanceChanged));

        private static void OnInnerDistanceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (GappedOverlay)sender;
            control.CalculateGaps();
        }

        /// <summary>
        /// Gets or sets the brush of the outher ellipse of the GappedOverlay.
        /// </summary>
        public Brush OutherCircleBrush
        {
            get { return (Brush)GetValue(OutherCircleBrushProperty); }
            set { SetValue(OutherCircleBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.GappedOverlay.OutherCircleBrush" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutherCircleBrushProperty =
            DependencyProperty.Register("OutherCircleBrush", typeof(Brush), typeof(GappedOverlay));

        /// <summary>
        /// Gets or sets the thickness of the outher ellipse of the GappedOverlay
        /// </summary>
        public double OutherCircleThickness
        {
            get { return (double)GetValue(OutherCircleThicknessProperty); }
            set { SetValue(OutherCircleThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.GappedOverlay.OutherCircleThickness" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutherCircleThicknessProperty =
            DependencyProperty.Register("OutherCircleThickness", typeof(double), typeof(GappedOverlay));

        /// <summary>
        /// Gets or sets the rotating angle of the whole GappedOverlay.
        /// </summary>
        public double RotateAngle
        {
            get { return (double)GetValue(RotateAngleProperty); }
            set { SetValue(RotateAngleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.GappedOverlay.RotateAngle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateAngleProperty =
            DependencyProperty.Register("RotateAngle", typeof(double), typeof(GappedOverlay), new UIPropertyMetadata(OnRotateAngleChanged));

        private static void OnRotateAngleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (GappedOverlay)sender;
            control.CalculateGaps();
        }

        private void OnOutherRadiusChanged()
        {
            Width = OutherRadius * 2.0;
            Height = Width;

            _outherEllipse.Center = new Point(OutherRadius, OutherRadius);
            _outherEllipse.RadiusX = OutherRadius;
            _outherEllipse.RadiusY = OutherRadius;

            _outherGapEllipse.Center = new Point(OutherRadius, OutherRadius);

            _innerGapEllipse.Center = new Point(OutherRadius, OutherRadius);

            _innerEllipse.Center = new Point(OutherRadius, OutherRadius);

            CalculateGaps();
        }

        private void OnOutherGapRadiusChanged()
        {
            _outherGapEllipse.RadiusX = OutherGapRadius;
            _outherGapEllipse.RadiusY = OutherGapRadius;

            CalculateGaps();
        }

        private void OnInnerGapRadiusChanged()
        {
            _innerGapEllipse.RadiusX = InnerGapRadius;
            _innerGapEllipse.RadiusY = InnerGapRadius;

            CalculateGaps();
        }

        private void OnInnerRadiusChanged()
        {
            _innerEllipse.RadiusX = InnerRadius;
            _innerEllipse.RadiusY = InnerRadius;

            CalculateGaps();
        }

        /// <summary>
        /// The template gets added to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _ellipse = GetTemplateChild("PART_Ellipse") as Ellipse;

            CalculateGaps();
        }

        private Ellipse _ellipse;

        private readonly EllipseGeometry _outherEllipse;
        private readonly EllipseGeometry _outherGapEllipse;
        private readonly EllipseGeometry _innerGapEllipse;
        private readonly EllipseGeometry _innerEllipse;

        private void CalculateGaps()
        {
            if (_ellipse == null)
                return;

            var start = 0.0;
            var fulldistance = GapDistance;
            var outherDifference = OutherDistance;
            var innerDifference = InnerDistance;

            var group = new GeometryGroup();

            while (start <= 1.0)
            {
                var point1 = GetPosition(start, _outherGapEllipse);
                var point2 = GetPosition(start, _outherEllipse);
                var point3 = GetPosition(fulldistance, _outherEllipse);
                var point4 = GetPosition(fulldistance, _outherGapEllipse);
                var point5 = GetPosition(fulldistance - outherDifference, _outherGapEllipse);
                var point6 = GetPosition(fulldistance - innerDifference, _innerGapEllipse);
                var point7 = GetPosition(fulldistance, _innerGapEllipse);
                var point8 = GetPosition(fulldistance, _innerEllipse);
                var point9 = GetPosition(start, _innerEllipse);
                var point10 = GetPosition(start, _innerGapEllipse);
                var point11 = GetPosition(start + innerDifference, _innerGapEllipse);
                var point12 = GetPosition(start + outherDifference, _outherGapEllipse);
                var point13 = point1;

                var pieFigure = new PathFigure();
                var pieGeometry = new PathGeometry();
                pieGeometry.Figures.Add(pieFigure);

                var line1 = new LineSegment(point2, false);
                var line2 = new ArcSegment(point3, new Size(OutherRadius, OutherRadius), 45, false, SweepDirection.Clockwise, false);
                var line3 = new LineSegment(point4, false);
                var line4 = new ArcSegment(point5, new Size(OutherGapRadius, OutherGapRadius), 45, false, SweepDirection.Counterclockwise, false);
                var line5 = new LineSegment(point6, false);
                var line6 = new ArcSegment(point7, new Size(InnerGapRadius, InnerGapRadius), 45, false, SweepDirection.Clockwise, false);
                var line7 = new LineSegment(point8, false);
                var line8 = new ArcSegment(point9, new Size(InnerRadius, InnerRadius), 45, false, SweepDirection.Counterclockwise, false);
                var line9 = new LineSegment(point10, false);
                var line10 = new ArcSegment(point11, new Size(InnerGapRadius, InnerGapRadius), 45, false, SweepDirection.Clockwise, false);
                var line11 = new LineSegment(point12, false);
                var line12 = new ArcSegment(point13, new Size(OutherGapRadius, OutherGapRadius), 45, false, SweepDirection.Counterclockwise, false);

                pieFigure.StartPoint = point1;
                pieFigure.Segments.Add(line1);
                pieFigure.Segments.Add(line2);
                pieFigure.Segments.Add(line3);
                pieFigure.Segments.Add(line4);
                pieFigure.Segments.Add(line5);
                pieFigure.Segments.Add(line6);
                pieFigure.Segments.Add(line7);
                pieFigure.Segments.Add(line8);
                pieFigure.Segments.Add(line9);
                pieFigure.Segments.Add(line10);
                pieFigure.Segments.Add(line11);
                pieFigure.Segments.Add(line12);

                group.Children.Add(pieGeometry);

                start += GapDistance;
                fulldistance += GapDistance;
            }

            _ellipse.Clip = group;
            _ellipse.RenderTransform = new RotateTransform(RotateAngle);
        }

        private Point GetPosition(double value, EllipseGeometry geometry)
        {
            var pathGeometry = geometry.GetOutlinedPathGeometry();
            if (pathGeometry.Figures.Count == 0)
                return geometry.Center;
            var outherGeometry = new PathGeometry(new[] { pathGeometry.Figures[0] });
            Point outherPoint, outherTangent;
            outherGeometry.GetPointAtFractionLength(value, out outherPoint, out outherTangent);
            return outherPoint;
        }
    }
}
