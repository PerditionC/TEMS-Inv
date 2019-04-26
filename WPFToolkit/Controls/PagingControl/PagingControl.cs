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
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Brings the possibility to show several pages one by one. This is possible by slide through the pages using navigation buttons or jump to a page directly.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <WPFToolkit:PagingControl LoopItems="False"
    ///                           SelectedIndex="0"
    ///                           ShowJumpBar="True">
    ///     <WPFToolkit:PagingItem Header="First Header Text" Footer="First Footer Text">
    ///         <ListBox />
    ///     </WPFToolkit:PagingItem>
    /// 
    ///     <WPFToolkit:PagingItem Header="Second Header Text" Footer="Second Footer Text">
    ///         <ListBox />
    ///     </WPFToolkit:PagingItem>
    /// 
    ///     <WPFToolkit:PagingItem Header="Third Header Text" Footer="Third Footer Text">
    ///         <ListBox />
    ///     </WPFToolkit:PagingItem>
    /// </WPFToolkit:PagingControl>
    /// ]]>
    /// </code>
    /// </example>
    [TemplatePart(Name = "PART_Previous", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Next", Type = typeof(Button))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_PresenterArea", Type = typeof(RectangleGeometry))]
    [TemplatePart(Name = "PART_VisualArea", Type = typeof(Rectangle))]
    public class PagingControl : Selector
    {
        static PagingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PagingControl), new FrameworkPropertyMetadata(typeof(PagingControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.PagingControl" /> class.
        /// </summary>
        public PagingControl()
        {
            ButtonItems = new ObservableCollection<int>();

            SizeChanged += HandleSizeChanged;
        }

        /// <summary>
        /// Gets or sets the button items used in the <see cref="DW.WPFToolkit.Controls.PagingJumpBar" />. This is used internally.
        /// </summary>
        [DefaultValue(null)]
        public ObservableCollection<int> ButtonItems
        {
            get { return (ObservableCollection<int>)GetValue(ButtonItemsProperty); }
            set { SetValue(ButtonItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.PagingControl.ButtonItems" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonItemsProperty =
            DependencyProperty.Register("ButtonItems", typeof(ObservableCollection<int>), typeof(PagingControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value that indicates if the direct jump bar is visible or not.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowJumpBar
        {
            get { return (bool)GetValue(ShowJumpBarProperty); }
            set { SetValue(ShowJumpBarProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.PagingControl.ShowJumpBar" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowJumpBarProperty =
            DependencyProperty.Register("ShowJumpBar", typeof(bool), typeof(PagingControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value that indicates where the previous page button is placed.
        /// </summary>
        [DefaultValue(Dock.Left)]
        public Dock PreviousBarPosition
        {
            get { return (Dock)GetValue(PreviousBarPositionProperty); }
            set { SetValue(PreviousBarPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.PagingControl.PreviousBarPosition" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviousBarPositionProperty =
            DependencyProperty.Register("PreviousBarPosition", typeof(Dock), typeof(PagingControl), new UIPropertyMetadata(Dock.Left));

        /// <summary>
        /// Gets or sets a value that indicates where the next page button is placed.
        /// </summary>
        [DefaultValue(Dock.Right)]
        public Dock NextBarPosition
        {
            get { return (Dock)GetValue(NextBarPositionProperty); }
            set { SetValue(NextBarPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.PagingControl.NextBarPosition" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty NextBarPositionProperty =
            DependencyProperty.Register("NextBarPosition", typeof(Dock), typeof(PagingControl), new UIPropertyMetadata(Dock.Right));

        /// <summary>
        /// Gets or sets a value that indicates where the direct jump bar have to be placed.
        /// </summary>
        [DefaultValue(Dock.Top)]
        public Dock JumpBarPosition
        {
            get { return (Dock)GetValue(JumpBarPositionProperty); }
            set { SetValue(JumpBarPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.PagingControl.JumpBarPosition" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty JumpBarPositionProperty =
            DependencyProperty.Register("JumpBarPosition", typeof(Dock), typeof(PagingControl), new UIPropertyMetadata(Dock.Top));

        /// <summary>
        /// Gets or sets the horizontal alignment of the previous and next page buttons.
        /// </summary>
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment HorizontalNavigationButtonsAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalNavigationButtonsAlignmentProperty); }
            set { SetValue(HorizontalNavigationButtonsAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.PagingControl.HorizontalNavigationButtonsAlignment" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalNavigationButtonsAlignmentProperty =
            DependencyProperty.Register("HorizontalNavigationButtonsAlignment", typeof(HorizontalAlignment), typeof(PagingControl), new UIPropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Gets or sets the vertical alignment of the previous and next page buttons.
        /// </summary>
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment VerticalNavigationButtonsAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalNavigationButtonsAlignmentProperty); }
            set { SetValue(VerticalNavigationButtonsAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.PagingControl.VerticalNavigationButtonsAlignment" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalNavigationButtonsAlignmentProperty =
            DependencyProperty.Register("VerticalNavigationButtonsAlignment", typeof(VerticalAlignment), typeof(PagingControl), new UIPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Gets or sets a value that indicates if switching to another page has a slide animation.
        /// </summary>
        [DefaultValue(true)]
        public bool HasSlideAnimation
        {
            get { return (bool)GetValue(HasSlideAnimationProperty); }
            set { SetValue(HasSlideAnimationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.PagingControl.HasSlideAnimation" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasSlideAnimationProperty =
            DependencyProperty.Register("HasSlideAnimation", typeof(bool), typeof(PagingControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets the slide animation speed to be used if <see cref="DW.WPFToolkit.Controls.PagingControl.HasSlideAnimation" /> is set to true.
        /// </summary>
        public TimeSpan AnimationSpeed
        {
            get { return (TimeSpan)GetValue(AnimationSpeedProperty); }
            set { SetValue(AnimationSpeedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.PagingControl.AnimationSpeed" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationSpeedProperty =
            DependencyProperty.Register("AnimationSpeed", typeof(TimeSpan), typeof(PagingControl), new UIPropertyMetadata(TimeSpan.FromMilliseconds(200)));

        /// <summary>
        /// Gets or sets the slide animation direction to be used if <see cref="DW.WPFToolkit.Controls.PagingControl.HasSlideAnimation" /> is set to true.
        /// </summary>
        [DefaultValue(Orientation.Horizontal)]
        public Orientation AnimationOrientation
        {
            get { return (Orientation)GetValue(AnimationOrientationProperty); }
            set { SetValue(AnimationOrientationProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.PagingControl.AnimationOrientation" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationOrientationProperty =
            DependencyProperty.Register("AnimationOrientation", typeof(Orientation), typeof(PagingControl), new UIPropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Gets or sets a value that indicates if the first page should be shown as the next page of the last and backward.
        /// </summary>
        [DefaultValue(true)]
        public bool LoopItems
        {
            get { return (bool)GetValue(LoopItemsProperty); }
            set { SetValue(LoopItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.PagingControl.LoopItems" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty LoopItemsProperty =
            DependencyProperty.Register("LoopItems", typeof(bool), typeof(PagingControl), new UIPropertyMetadata(true));

        /// <summary>
        /// Recreates the jump bar items hold in the <see cref="DW.WPFToolkit.Controls.PagingControl.ButtonItems" /> property as soon the items collection changes.
        /// </summary>
        /// <param name="e">The parameter passed by the caller.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            ButtonItems.Clear();
            for (int i = 0; i < Items.Count; ++i)
                ButtonItems.Add(i);
        }

        /// <summary>
        /// Generates a new child item container to hold in the <see cref="DW.WPFToolkit.Controls.PagingControl" />.
        /// </summary>
        /// <returns>The generated child item container</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PagingItem();
        }

        /// <summary>
        /// Checks if the item is already the correct item container. If not the <see cref="DW.WPFToolkit.Controls.PagingControl.GetContainerForItemOverride" /> will be used to generate the right container.
        /// </summary>
        /// <param name="item">The item to shown in the <see cref="DW.WPFToolkit.Controls.PagingControl" />.</param>
        /// <returns>True if the item is the correct item container already.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PagingItem;
        }

        /// <summary>
        /// The template gets added to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _previousButton = GetTemplateChild("PART_Previous") as Button;
            if (_previousButton != null)
                _previousButton.Click += Previous_Click;

            _nextButton = GetTemplateChild("PART_Next") as Button;
            if (_nextButton != null)
                _nextButton.Click += Next_Click;

            _contentPresenter = GetTemplateChild("PART_ContentPresenter") as ContentControl;
            _visibleClipArea = GetTemplateChild("PART_PresenterArea") as RectangleGeometry;
            _oldContentRectangle = GetTemplateChild("PART_VisualArea") as Rectangle;
            _visibleAreaGrid = GetTemplateChild("PART_MainGrid") as Grid;

            EnableDisableButtons();
        }

        private ContentControl _contentPresenter;
        private RectangleGeometry _visibleClipArea;
        private Rectangle _oldContentRectangle;
        private Grid _visibleAreaGrid;
        private Button _previousButton;
        private Button _nextButton;

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            if (Items.Count > 0)
            {
                var index = SelectedIndex;
                if (index > 0)
                    SelectedIndex = --index;
                else if (LoopItems)
                    SelectedIndex = Items.Count - 1;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (Items.Count > 0)
            {
                var index = SelectedIndex;
                if (index < Items.Count - 1)
                    SelectedIndex = ++index;
                else if (LoopItems)
                    SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Switches the current displayed page. If <see cref="DW.WPFToolkit.Controls.PagingControl.HasSlideAnimation" /> is set to true the animation starts.
        /// </summary>
        /// <param name="e">The parameter passed by the owner.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);

            EnableDisableButtons();

            if (!CanAnimate(e.RemovedItems, e.AddedItems))
                return;

            RenderAndShowOldContent(e.RemovedItems[0] as FrameworkElement);

            if (AnimateRightToLeft(e.RemovedItems[0], e.AddedItems[0]))
                SwitchDisplayContent(0, GetSizeToMove()*-1, GetSizeToMove(), 0);
            else
                SwitchDisplayContent(0, GetSizeToMove(), GetSizeToMove()*-1, 0);
        }

        private void EnableDisableButtons()
        {
            if (LoopItems)
                return;

            if (_nextButton != null)
                _nextButton.IsEnabled = CanMoveNext();
            if (_previousButton != null)
                _previousButton.IsEnabled = CanMoveBack();
        }

        private bool CanMoveNext()
        {
            return SelectedIndex < Items.Count - 1;
        }

        private bool CanMoveBack()
        {
            return SelectedIndex > 0;
        }

        private bool CanAnimate(IList addedItems, IList removedItems)
        {
            return HasSlideAnimation &&_contentPresenter != null && addedItems.Count > 0 && removedItems.Count > 0;
        }

        private bool AnimateRightToLeft(object removedItem, object addedItem)
        {
            return Items.IndexOf(removedItem) < Items.IndexOf(addedItem);
        }

        private void RenderAndShowOldContent(FrameworkElement frameworkElement)
        {
            if (frameworkElement != null)
            {
                var renderBitmap = RenderContent(frameworkElement);
                _oldContentRectangle.Fill = new ImageBrush(BitmapFrame.Create(renderBitmap));
            }
        }

        private void SwitchDisplayContent(int imageFrom, double imageTo, double contentFrom, int contentTo)
        {
            var fadeNewInStoryboard = new Storyboard();
            Storyboard.SetTargetProperty(fadeNewInStoryboard, GetAnimationAxis());

            var moveImageOutAnimation = new DoubleAnimation(imageFrom, imageTo, new Duration(AnimationSpeed));
            Storyboard.SetTargetName(moveImageOutAnimation, "PART_VisualArea");
            fadeNewInStoryboard.Children.Add(moveImageOutAnimation);

            var moveControlInAnimation = new DoubleAnimation(contentFrom, contentTo, new Duration(AnimationSpeed));
            Storyboard.SetTargetName(moveControlInAnimation, "PART_ContentPresenter");
            fadeNewInStoryboard.Children.Add(moveControlInAnimation);

            _visibleAreaGrid.BeginStoryboard(fadeNewInStoryboard);
        }

        private PropertyPath GetAnimationAxis()
        {
            return AnimationOrientation == Orientation.Horizontal ? new PropertyPath("RenderTransform.(TranslateTransform.X)") : new PropertyPath("RenderTransform.(TranslateTransform.Y)");
        }

        private double GetSizeToMove()
        {
            return AnimationOrientation == Orientation.Horizontal ? _contentPresenter.ActualWidth : _contentPresenter.ActualHeight;
        }

        private static RenderTargetBitmap RenderContent(FrameworkElement element)
        {
            var width = (int)element.ActualWidth;
            var height = (int)element.ActualHeight;
            // TODO: Calculate used dpi
            double dpiX = 96;
            double dpiY = 96;

            var visual = new DrawingVisual();
            var drawingContext = visual.RenderOpen();
            drawingContext.DrawRectangle(new VisualBrush(element), null, new Rect(0, 0, width, height));
            drawingContext.Close();

            var bitmap = new RenderTargetBitmap(width, height, dpiX, dpiY, PixelFormats.Default);

            bitmap.Render(visual);
            return bitmap;
        }

        private void HandleSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _visibleClipArea.Rect = new Rect(0, 0, _contentPresenter.ActualWidth, _contentPresenter.ActualHeight);
        }
    }
}
