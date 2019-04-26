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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Implements the ProgressBar in an ellipsed form.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <Controls:EllipsedProgressBar Value="10"
    ///                               Minimum="0"
    ///                               Maximum="100"
    ///                               OutherRadius="20"
    ///                               RotateSpeed="0.5"
    ///                               IsIndeterminate="True" />
    /// 
    /// <Controls:EllipsedProgressBar Value="25"
    ///                               Minimum="0"
    ///                               Maximum="100"
    ///                               OutherRadius="20"
    ///                               RotateSpeed="0.8"
    ///                               IsIndeterminate="True"
    ///                               IsInversed="True" />
    /// 
    /// <Controls:EllipsedProgressBar Value="34"
    ///                               Minimum="0"
    ///                               Maximum="100"
    ///                               OutherRadius="20"
    ///                               HasPercentLabel="True" />
    /// 
    /// <Controls:EllipsedProgressBar Value="0"
    ///                               Minimum="0"
    ///                               Maximum="100"
    ///                               OutherRadius="20"
    ///                               RotateSpeed="0.8"
    ///                               DisplayKind="Pointer"
    ///                               InnerRadius="10"
    ///                               ShowInnerCircle="True"
    ///                               ShowOutherCircle="True"
    ///                               StrokeThickness="1"
    ///                               IsIndeterminate="True" />
    /// 
    /// <Grid>
    ///     <Controls:EllipsedProgressBar Value="20"
    ///                                   Minimum="0"
    ///                                   Maximum="100"
    ///                                   OutherRadius="20"
    ///                                   InnerRadius="5"
    ///                                   RotateSpeed="0.7"
    ///                                   IsIndeterminate="True" />
    ///     
    ///     <Controls:GappedOverlay OutherRadius="21" InnerRadius="5" InnerGapRadius="8" OutherGapRadius="16" />
    ///     
    /// </Grid>
    /// 
    /// <Controls:EllipsedProgressBar Value="100"
    ///                               Minimum="0"
    ///                               Maximum="100"
    ///                               InnerRadius="16"
    ///                               OutherRadius="18.5"
    ///                               RotateSpeed="0.8"
    ///                               IsIndeterminate="True"
    ///                               DisplayKind="Items">
    ///     <Controls:EllipsedProgressBar.ItemsFactory>
    ///         <Controls:EllipseItemsFactory Size="5"
    ///                                       ItemsCount="15"
    ///                                       OpacityShrinking="0.1" />
    ///     </Controls:EllipsedProgressBar.ItemsFactory>
    /// </Controls:EllipsedProgressBar>
    /// 
    /// <Controls:EllipsedProgressBar Value="100"
    ///                               Minimum="0"
    ///                               Maximum="100"
    ///                               InnerRadius="15"
    ///                               OutherRadius="20"
    ///                               RotateSpeed="0.8"
    ///                               IsIndeterminate="True"
    ///                               DisplayKind="Items">
    ///     <Controls:EllipsedProgressBar.ItemsFactory>
    ///         <Controls:LineItemsFactory Caps="Flat"
    ///                                    Length="8"
    ///                                    OpacityShrinking="0.1"
    ///                                    ItemsCount="8"
    ///                                    Thickness="2" />
    ///     </Controls:EllipsedProgressBar.ItemsFactory>
    /// </Controls:EllipsedProgressBar>
    /// ]]>
    /// </code>
    /// </example>
    [TemplatePart(Name = "PART_Pointer", Type = typeof(Path))]
    [TemplatePart(Name = "PART_Pie", Type = typeof(Ellipse))]
    [TemplatePart(Name = "PART_PercentLabel", Type = typeof(Label))]
    [TemplatePart(Name = "PART_Items", Type = typeof(EllipsePanel))]
    public class EllipsedProgressBar : ProgressBar
    {
        static EllipsedProgressBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EllipsedProgressBar), new FrameworkPropertyMetadata(typeof(EllipsedProgressBar)));
        }

        private bool _initializeCollapsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" /> class.
        /// </summary>
        public EllipsedProgressBar()
        {
            _outherEllipse = new EllipseGeometry();
            _innerEllipse = new EllipseGeometry();

            Loaded += (sender, e) =>
            {
                CalculateValues();
                OnIsIndeterminateChanged();

                _initializeCollapsed = Visibility == Visibility.Collapsed;
            };
        }

        /// <summary>
        /// Called when the element has to be rendered.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_initializeCollapsed)
            {
                _initializeCollapsed = false;

                CalculateValues();
                OnIsIndeterminateChanged();
            }
        }

        /// <summary>
        /// Gets or sets the outher radius of the ellipse.
        /// </summary>
        [DefaultValue(0.0)]
        public double OutherRadius
        {
            get { return (double)GetValue(OutherRadiusProperty); }
            set { SetValue(OutherRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.OutherRadius" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutherRadiusProperty =
            DependencyProperty.Register("OutherRadius", typeof(double), typeof(EllipsedProgressBar), new UIPropertyMetadata(0.0, OnOutherRadiusChanged));

        private static void OnOutherRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (EllipsedProgressBar)sender;
            control.OnOutherRadiusChanged();
        }

        /// <summary>
        /// Gets or sets the inner radius of the ellipse
        /// </summary>
        [DefaultValue(0.0)]
        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.InnerRadius" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerRadiusProperty =
            DependencyProperty.Register("InnerRadius", typeof(double), typeof(EllipsedProgressBar), new UIPropertyMetadata(0.0, OnInnerRadiusChanged));

        private static void OnInnerRadiusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (EllipsedProgressBar)sender;
            control.OnInnerRadiusChanged();
        }

        /// <summary>
        /// Gets or sets the stroke thickness of the ellipses.
        /// </summary>
        [DefaultValue(0.0)]
        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.StrokeThickness" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(EllipsedProgressBar), new UIPropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets a value which indicates how the progress should be displayed. See <see cref="DW.WPFToolkit.Controls.EllipsedProgressBarKind" />.
        /// </summary>
        [DefaultValue(EllipsedProgressBarKind.Pie)]
        public EllipsedProgressBarKind DisplayKind
        {
            get { return (EllipsedProgressBarKind)GetValue(DisplayKindProperty); }
            set { SetValue(DisplayKindProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.DisplayKind" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayKindProperty =
            DependencyProperty.Register("DisplayKind", typeof(EllipsedProgressBarKind), typeof(EllipsedProgressBar), new UIPropertyMetadata(EllipsedProgressBarKind.Pie, OnDisplayKindChanged));

        private static void OnDisplayKindChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (EllipsedProgressBar)sender;
            control.CalculateValues();
        }

        /// <summary>
        /// Gets or sets a value which indicates if the progress should be painted as an inversed state.
        /// </summary>
        [DefaultValue(false)]
        public bool IsInversed
        {
            get { return (bool)GetValue(IsInversedProperty); }
            set { SetValue(IsInversedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.IsInversed" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInversedProperty =
            DependencyProperty.Register("IsInversed", typeof(bool), typeof(EllipsedProgressBar), new UIPropertyMetadata(false, OnIsInversedChanged));

        private static void OnIsInversedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (EllipsedProgressBar)sender;
            control.CalculateValues();
        }

        /// <summary>
        /// Gets or sets a value which indicates of the outher ellipse is shown or not.
        /// </summary>
        [DefaultValue(false)]
        public bool ShowOutherCircle
        {
            get { return (bool)GetValue(ShowOutherCircleProperty); }
            set { SetValue(ShowOutherCircleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.ShowOutherCircle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowOutherCircleProperty =
            DependencyProperty.Register("ShowOutherCircle", typeof(bool), typeof(EllipsedProgressBar), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets the color of the outher ellipse.
        /// </summary>
        public Brush OutherCircleBrush
        {
            get { return (Brush)GetValue(OutherCircleBrushProperty); }
            set { SetValue(OutherCircleBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.OutherCircleBrush" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutherCircleBrushProperty =
            DependencyProperty.Register("OutherCircleBrush", typeof(Brush), typeof(EllipsedProgressBar));

        /// <summary>
        /// Gets or sets the color of the inner ellipse
        /// </summary>
        public Brush InnerCircleBrush
        {
            get { return (Brush)GetValue(InnerCircleBrushProperty); }
            set { SetValue(InnerCircleBrushProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.InnerCircleBrush" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerCircleBrushProperty =
            DependencyProperty.Register("InnerCircleBrush", typeof(Brush), typeof(EllipsedProgressBar));

        /// <summary>
        /// Gets or sets the thickness of the outher ellipse.
        /// </summary>
        public double OutherCircleThickness
        {
            get { return (double)GetValue(OutherCircleThicknessProperty); }
            set { SetValue(OutherCircleThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.OutherCircleThickness" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutherCircleThicknessProperty =
            DependencyProperty.Register("OutherCircleThickness", typeof(double), typeof(EllipsedProgressBar));

        /// <summary>
        /// Gets or sets the thickness of the inner ellipse.
        /// </summary>
        public double InnerCircleThickness
        {
            get { return (double)GetValue(InnerCircleThicknessProperty); }
            set { SetValue(InnerCircleThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.InnerCircleThickness" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerCircleThicknessProperty =
            DependencyProperty.Register("InnerCircleThickness", typeof(double), typeof(EllipsedProgressBar));

        /// <summary>
        /// Gets or set a value which indicates of the inner ellipse is shown or not.
        /// </summary>
        [DefaultValue(false)]
        public bool ShowInnerCircle
        {
            get { return (bool)GetValue(ShowInnerCircleProperty); }
            set { SetValue(ShowInnerCircleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.ShowInnerCircle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowInnerCircleProperty =
            DependencyProperty.Register("ShowInnerCircle", typeof(bool), typeof(EllipsedProgressBar), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets the start position of the progress in the ellipse. 0 is on Top.
        /// </summary>
        [DefaultValue(0.0)]
        public double StartAngle
        {
            get { return (double)GetValue(StartAngleProperty); }
            set { SetValue(StartAngleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.StartAngle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register("StartAngle", typeof(double), typeof(EllipsedProgressBar), new UIPropertyMetadata(0.0, OnStartAngleChanged));

        private static void OnStartAngleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (EllipsedProgressBar)sender;
            control.CalculateValues();
        }

        /// <summary>
        /// Gets or sets the dash array of the outher ellipse.
        /// </summary>
        public DoubleCollection OutherCircleDashArray
        {
            get { return (DoubleCollection)GetValue(OutherCircleDashArrayProperty); }
            set { SetValue(OutherCircleDashArrayProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.OutherCircleDashArray" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutherCircleDashArrayProperty =
            DependencyProperty.Register("OutherCircleDashArray", typeof(DoubleCollection), typeof(EllipsedProgressBar));

        /// <summary>
        /// Gets or sets the dash array of the inner circle.
        /// </summary>
        public DoubleCollection InnerCircleDashArray
        {
            get { return (DoubleCollection)GetValue(InnerCircleDashArrayProperty); }
            set { SetValue(InnerCircleDashArrayProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.InnerCircleDashArray" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerCircleDashArrayProperty =
            DependencyProperty.Register("InnerCircleDashArray", typeof(DoubleCollection), typeof(EllipsedProgressBar));

        /// <summary>
        /// Gets or sets the style of ther percentage display in center of the EllipsedProgressBar.
        /// </summary>
        public Style PercentLabelStyle
        {
            get { return (Style)GetValue(PercentLabelStyleProperty); }
            set { SetValue(PercentLabelStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.PercentLabelStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PercentLabelStyleProperty =
            DependencyProperty.Register("PercentLabelStyle", typeof(Style), typeof(EllipsedProgressBar));

        /// <summary>
        /// Gets or sets a value which indicates if a percentage text is shown in the center of the EllipsedProgressBar or not.
        /// </summary>
        [DefaultValue(false)]
        public bool HasPercentLabel
        {
            get { return (bool)GetValue(HasPercentLabelProperty); }
            set { SetValue(HasPercentLabelProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.HasPercentLabel" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasPercentLabelProperty =
            DependencyProperty.Register("HasPercentLabel", typeof(bool), typeof(EllipsedProgressBar), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets the <see cref="DW.WPFToolkit.Controls.IItemsFactory" /> to be used when the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.DisplayKind" /> is set to <see cref="DW.WPFToolkit.Controls.EllipsedProgressBarKind.Items" />.
        /// </summary>
        public IItemsFactory ItemsFactory
        {
            get { return (IItemsFactory)GetValue(ItemsFactoryProperty); }
            set { SetValue(ItemsFactoryProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.ItemsFactory" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsFactoryProperty =
            DependencyProperty.Register("ItemsFactory", typeof(IItemsFactory), typeof(EllipsedProgressBar), new UIPropertyMetadata(OnItemsFactoryChanged));

        private static void OnItemsFactoryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (EllipsedProgressBar)sender;
            control.TryTakeItems();
        }

        /// <summary>
        /// Gets or sets a value which indicates if the items created by the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.ItemsFactory" /> should be rotated or not.
        /// </summary>
        [DefaultValue(true)]
        public bool RotateItems
        {
            get { return (bool)GetValue(RotateItemsProperty); }
            set { SetValue(RotateItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.RotateItems" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateItemsProperty =
            DependencyProperty.Register("RotateItems", typeof(bool), typeof(EllipsedProgressBar), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value which indicates if the EllipsedProgressBar is indeterminate.
        /// </summary>
        [DefaultValue(false)]
        public new bool IsIndeterminate
        {
            get { return (bool)this.GetValue(IsIndeterminateProperty); }
            set { this.SetValue(IsIndeterminateProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.IsIndeterminate" /> dependency property.
        /// </summary>
        public static new readonly DependencyProperty IsIndeterminateProperty =
            ProgressBar.IsIndeterminateProperty.AddOwner(typeof(EllipsedProgressBar), new FrameworkPropertyMetadata(false, OnIsIndeterminateChanged));

        private static void OnIsIndeterminateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (EllipsedProgressBar)sender;
            if (control.IsLoaded)
                control.OnIsIndeterminateChanged();
        }

        /// <summary>
        /// Gets or sets the rotating speed to be used when <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.IsIndeterminate" /> is set to true.
        /// </summary>
        [DefaultValue(1.5)]
        public double RotateSpeed
        {
            get { return (double)GetValue(RotateSpeedProperty); }
            set { SetValue(RotateSpeedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.RotateSpeed" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateSpeedProperty =
            DependencyProperty.Register("RotateSpeed", typeof(double), typeof(EllipsedProgressBar), new UIPropertyMetadata(1.5));

        /// <summary>
        /// Gets or sets the rotate orientation to be used for the items created by the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.ItemsFactory" /> if <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.RotateItems" /> is set to true.
        /// </summary>
        [DefaultValue(SweepDirection.Clockwise)]
        public SweepDirection RotateDirection
        {
            get { return (SweepDirection)GetValue(RotateDirectionProperty); }
            set { SetValue(RotateDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.RotateDirection" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty RotateDirectionProperty =
            DependencyProperty.Register("RotateDirection", typeof(SweepDirection), typeof(EllipsedProgressBar), new UIPropertyMetadata(SweepDirection.Clockwise));

        /// <summary>
        /// Handles changed minimum value.
        /// </summary>
        /// <param name="oldMinimum">The old minimum value.</param>
        /// <param name="newMinimum">The new minimum value.</param>
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            if (CalculateValues())
                OnIsIndeterminateChanged();
        }

        /// <summary>
        /// Handles changed maximum value.
        /// </summary>
        /// <param name="oldMaximum">The old maximum value.</param>
        /// <param name="newMaximum">The new maximum value.</param>
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            if (CalculateValues())
                OnIsIndeterminateChanged();
        }

        /// <summary>
        /// Handles changed progress value.
        /// </summary>
        /// <param name="oldValue">The old progress value.</param>
        /// <param name="newValue">The new progress value.</param>
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            if (CalculateValues())
                OnIsIndeterminateChanged();
        }

        private DoubleAnimation _currentAnimation;
        private RotateTransform _currentTransform;

        private void OnIsIndeterminateChanged()
        {
            TryTakeItems();
            CalculateValues();

            if (IsIndeterminate)
                BeginAnimation();
            else
            {
                EndAnimation();
                CalculateValues();
            }
        }

        private void BeginAnimation()
        {
            if (_pieEllipse == null || _pointerPath == null || _itemsPanel == null)
                return;

            _currentAnimation = new DoubleAnimation();
            _currentAnimation.From = 0;
            _currentAnimation.To = 360;
            _currentAnimation.Duration = new Duration(TimeSpan.FromSeconds(RotateSpeed));
            _currentAnimation.RepeatBehavior = RepeatBehavior.Forever;

            _currentTransform = new RotateTransform(0, 0, 0);
            _currentTransform.BeginAnimation(RotateTransform.AngleProperty, _currentAnimation);

            if (DisplayKind == EllipsedProgressBarKind.Pie)
                _pieEllipse.RenderTransform = _currentTransform;
            else if (DisplayKind == EllipsedProgressBarKind.Pointer)
                _pointerPath.RenderTransform = _currentTransform;
            else
                _itemsPanel.RenderTransform = _currentTransform;
        }

        private void EndAnimation()
        {
            if (_currentTransform != null)
            {
                _currentTransform.BeginAnimation(RotateTransform.AngleProperty, null);
                RenderTransform = null;
                _currentAnimation = null;
                _currentTransform = null;
            }
        }

        private void OnOutherRadiusChanged()
        {
            Width = OutherRadius * 2.0;
            Height = Width;

            _outherEllipse.RadiusX = OutherRadius;
            _outherEllipse.RadiusY = OutherRadius;
            _outherEllipse.Center = new Point(OutherRadius, OutherRadius);
            _innerEllipse.Center = new Point(OutherRadius, OutherRadius);
        }

        private void OnInnerRadiusChanged()
        {
            _innerEllipse.RadiusX = InnerRadius;
            _innerEllipse.RadiusY = InnerRadius;
        }

        /// <summary>
        /// The template gets added to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _pointerPath = GetTemplateChild("PART_Pointer") as Path;
            _pieEllipse = GetTemplateChild("PART_Pie") as Ellipse;
            _percentLabel = GetTemplateChild("PART_PercentLabel") as Label;
            _itemsPanel = GetTemplateChild("PART_Items") as EllipsePanel;

            TryTakeItems();
        }

        private void TryTakeItems()
        {
            if (_itemsPanel != null && ItemsFactory != null)
            {
                _currentItems = ItemsFactory.GenerateItems(IsIndeterminate);
                _itemsPanel.Children.Clear();
                foreach (UIElement element in _currentItems)
                    _itemsPanel.Children.Add(element);
            }
        }

        private Path _pointerPath;
        private Ellipse _pieEllipse;
        private EllipsePanel _itemsPanel;

        private readonly EllipseGeometry _outherEllipse;
        private readonly EllipseGeometry _innerEllipse;
        private Label _percentLabel;
        private IEnumerable<UIElement> _currentItems;

        private bool CalculateValues()
        {
            if (!CanCalculateValues())
                return false;

            if (DisplayKind == EllipsedProgressBarKind.Pie)
                SetPieValues();
            else if (DisplayKind == EllipsedProgressBarKind.Pointer)
                SetPointerValues();
            else
                SetItemsValues();
            return true;
        }

        private void SetPieValues()
        {
            var value = GetValue(true);

            var outherRadius = _outherEllipse.RadiusX;
            var innerRadius = _innerEllipse.RadiusX;

            var outherStartPoint = GetPosition(0, _outherEllipse);
            var outherEndPoint = GetPosition(value, _outherEllipse);
            var innerStartPoint = GetPosition(0, _innerEllipse);
            var innerEndPoint = GetPosition(value, _innerEllipse);

            var lineFromCenterToCircle = new LineSegment(outherStartPoint, false);

            PathSegment outherSegment;
            PathSegment innerSegment;
            if (IsInversed)
            {
                outherSegment = new ArcSegment(outherEndPoint, new Size(outherRadius, outherRadius), 45, value < 0.5, SweepDirection.Counterclockwise, false);
                innerSegment = new ArcSegment(innerStartPoint, new Size(innerRadius, innerRadius), 45, value < 0.5, SweepDirection.Clockwise, false);
            }
            else
            {
                outherSegment = new ArcSegment(outherEndPoint, new Size(outherRadius, outherRadius), 45, value > 0.5, SweepDirection.Clockwise, false);
                innerSegment = new ArcSegment(innerStartPoint, new Size(innerRadius, innerRadius), 45, value > 0.5, SweepDirection.Counterclockwise, false);
            }

            var lineFromCircleToCenter = new LineSegment(innerEndPoint, false);

            var pieFigure = new PathFigure();
            var pieGeometry = new PathGeometry();
            pieGeometry.Figures.Add(pieFigure);

            pieFigure.StartPoint = innerStartPoint;
            pieFigure.Segments.Add(lineFromCenterToCircle);
            pieFigure.Segments.Add(outherSegment);
            pieFigure.Segments.Add(lineFromCircleToCenter);
            pieFigure.Segments.Add(innerSegment);

            _pieEllipse.Clip = pieGeometry;

            _pieEllipse.RenderTransform = new RotateTransform(StartAngle);
        }

        private void SetPointerValues()
        {
            var value = GetValue(false);

            var outherPoint = GetPosition(value, _outherEllipse);
            var innerPoint = GetPosition(value, _innerEllipse);

            var lineFromCircleToCenter = new LineSegment(innerPoint, true);

            var pointerFigure = new PathFigure();
            var pointerGeometry = new PathGeometry();
            pointerGeometry.Figures.Add(pointerFigure);

            pointerFigure.StartPoint = outherPoint;
            pointerFigure.Segments.Add(lineFromCircleToCenter);

            _pointerPath.Data = pointerGeometry;

            _pointerPath.RenderTransform = new RotateTransform(StartAngle);
        }

        private void SetItemsValues()
        {
            ItemsFactory.EditItemsForValue(_currentItems, Minimum, Maximum, Value);

            CalculatePercentageValue();

            _itemsPanel.RenderTransform = new RotateTransform(StartAngle);
        }

        private double GetValue(bool adjusted)
        {
            var value = CalculatePercentageValue();

            if (adjusted)
            {
                if (IsInversed)
                {
                    if (Math.Abs(value - 0) < 0.001)
                        value = 0.00001;
                }
                else if (Math.Abs(value - 1) < 0.001)
                    value = 0.99999;
            }
            return value;
        }

        private double CalculatePercentageValue()
        {
            var value = (Value - Minimum) / (Maximum - Minimum);
            SetPercentageText(value);
            return value;
        }

        private void SetPercentageText(double value)
        {
            _percentLabel.Content = value.ToString("0%");
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

        private bool CanCalculateValues()
        {
            if (IsIndeterminate)
            {
                if (_pieEllipse == null)
                    return false;
                if (_itemsPanel == null)
                    return false;
                return true;
            }
            if (DisplayKind == EllipsedProgressBarKind.Pie)
                return _pieEllipse != null && _outherEllipse != null && _innerEllipse != null;
            if (DisplayKind == EllipsedProgressBarKind.Pointer)
                return _pointerPath != null && _outherEllipse != null && _innerEllipse != null;
            return _itemsPanel != null && ItemsFactory != null;
        }
    }
}
