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
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;

namespace DW.WPFToolkit.Interactivity
{
    /// <summary>
    /// Brings the functionality to the TextBlock and Label to show the text in the tooltip automatically when its cut.
    /// </summary>
    /// <remarks>In the case of the Label the Content.ToString() will be used to get the text. If a tooltip is set already it will be overwritten.</remarks>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <TextBlock Text="{Binding AnyLongtext}" Interactivity:CutTooltipBehavior.ShowTooltip="Width" />
    /// ]]>
    /// </code>
    /// </example>
    public class CutTooltipBehavior : DependencyObject
    {
        private readonly FrameworkElement _owner;
        private CutTextKind _cutTextKind;

        private CutTooltipBehavior(FrameworkElement owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Gets the value that defines when the tooltip should be shown.
        /// </summary>
        /// <param name="obj">The element from which the property value is read.</param>
        /// <returns>he DW.WPFToolkit.Interactivity.CutTooltipBehavior.ShowTooltip property value for the element.</returns>
        public static CutTextKind GetShowTooltip(DependencyObject obj)
        {
            return (CutTextKind)obj.GetValue(ShowTooltipProperty);
        }

        /// <summary>
        /// Sets the value that defines when the tooltip should be shown.
        /// </summary>
        /// <param name="obj">The element to which the attached property is written.</param>
        /// <param name="value">The needed DW.WPFToolkit.Interactivity.CutTooltipBehavior.ShowTooltip value.</param>
        public static void SetShowTooltip(DependencyObject obj, CutTextKind value)
        {
            obj.SetValue(ShowTooltipProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Interactivity.CutTooltipBehavior.GetShowTooltip(DependencyObject)" /> <see cref="DW.WPFToolkit.Interactivity.CutTooltipBehavior.SetShowTooltip(DependencyObject, CutTextKind)" /> attached property.
        /// </summary>
        public static readonly DependencyProperty ShowTooltipProperty =
            DependencyProperty.RegisterAttached("ShowTooltip", typeof(CutTextKind), typeof(CutTooltipBehavior), new PropertyMetadata(OnShowTooltipChanged));

        private static CutTooltipBehavior GetBehavior(DependencyObject obj)
        {
            return (CutTooltipBehavior)obj.GetValue(BehaviorProperty);
        }

        private static void SetBehavior(DependencyObject obj, CutTooltipBehavior value)
        {
            obj.SetValue(BehaviorProperty, value);
        }

        private static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.RegisterAttached("Behavior", typeof(CutTooltipBehavior), typeof(CutTooltipBehavior), new PropertyMetadata(null));

        private static void OnShowTooltipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is TextBlock) && !(d is Label))
                throw new InvalidOperationException("The CutTooltipBehavior can be attached to a TextBlock or Label only.");

            var behavior = GetBehavior(d);
            if (behavior == null)
            {
                behavior = new CutTooltipBehavior(d as FrameworkElement);
                SetBehavior(d, behavior);
            }

            behavior.OnShowTooltipChanged((CutTextKind)e.OldValue, (CutTextKind)e.NewValue);
        }

        private void OnShowTooltipChanged(CutTextKind oldValue, CutTextKind newValue)
        {
            _cutTextKind = (CutTextKind)newValue;

            if (oldValue != CutTextKind.None)
            {
                _owner.IsVisibleChanged -= OnIsVisibleChanged;
                _owner.LayoutUpdated -= OnLayoutUpdated;
                _owner.SizeChanged -= OnSizeChanged;
            }
            if (newValue != CutTextKind.None)
            {
                _owner.IsVisibleChanged += OnIsVisibleChanged;
                _owner.LayoutUpdated += OnLayoutUpdated;
                _owner.SizeChanged += OnSizeChanged;
            }
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CalculateTooltip();
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            CalculateTooltip();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CalculateTooltip();
        }

        private void CalculateTooltip()
        {
            var text = GetContentText();
            if (string.IsNullOrWhiteSpace(text))
                return;

            var fontInfo = GetFontInfo();
            if (fontInfo == null)
                return;

            var isWrapped = GetIsWrapped();

            Calculate(text, fontInfo, isWrapped);
        }

        private void Calculate(string text, FontInfo fontInfo, bool isWrapped)
        {
            var textLength = GetTextLength(fontInfo, text, isWrapped);

            var showTooltip = false;
            switch (_cutTextKind)
            {
                case CutTextKind.Height:
                    showTooltip = (textLength.Height > _owner.DesiredSize.Height);
                    break;
                case CutTextKind.Width:
                    showTooltip = (textLength.Width > _owner.DesiredSize.Width);
                    break;
                case CutTextKind.WithAndHeight:
                    showTooltip = (textLength.Width > _owner.DesiredSize.Width || textLength.Height > _owner.DesiredSize.Height);
                    break;
            }

            _owner.ToolTip = showTooltip ? text : null;
        }

        private Size GetTextLength(FontInfo fontInfo, string text, bool isWrapped)
        {
            var typeface = new Typeface(fontInfo.FontFamily, fontInfo.FontStyle, fontInfo.FontWeight, fontInfo.FontStretch);
            var formattedText = new FormattedText(text, Thread.CurrentThread.CurrentCulture, fontInfo.FlowDirection, typeface, fontInfo.FontSize, fontInfo.Foreground);

            if (isWrapped)
                formattedText.MaxTextWidth = fontInfo.ActualWidth;

            var padding = GetPadding();
            return new Size(formattedText.Width + padding.Left + padding.Right, formattedText.Height + padding.Bottom + padding.Top);
        }

        private Thickness GetPadding()
        {
            var textBlock = _owner as TextBlock;
            if (textBlock != null)
                return textBlock.Padding;
            var label = _owner as Label;
            if (label != null && label.Content != null)
                return label.Padding;
            return new Thickness(0);
        }

        private string GetContentText()
        {
            var textBlock = _owner as TextBlock;
            if (textBlock != null)
                return textBlock.Text;
            var label = _owner as Label;
            if (label != null && label.Content != null)
                return label.Content.ToString();
            return string.Empty;
        }

        private bool GetIsWrapped()
        {
            var textBlock = _owner as TextBlock;
            if (textBlock != null)
                return textBlock.TextWrapping != TextWrapping.NoWrap;
            return false;
        }

        private FontInfo GetFontInfo()
        {
            var textBlock = _owner as TextBlock;
            if (textBlock != null)
                return GetFontInfo((TextBlock)_owner);
            var label = _owner as Label;
            if (label != null)
                return GetFontInfo((Label)_owner);
            return null;
        }

        private FontInfo GetFontInfo(TextBlock textBlock)
        {
            return new FontInfo
            {
                FontFamily = textBlock.FontFamily,
                FontStyle = textBlock.FontStyle,
                FontWeight = textBlock.FontWeight,
                FontStretch = textBlock.FontStretch,
                FlowDirection = textBlock.FlowDirection,
                FontSize = textBlock.FontSize,
                Foreground = textBlock.Foreground,
                ActualWidth = textBlock.DesiredSize.Width,
                ActualHeight = textBlock.DesiredSize.Height
            };
        }

        private FontInfo GetFontInfo(Label label)
        {
            return new FontInfo
            {
                FontFamily = label.FontFamily,
                FontStyle = label.FontStyle,
                FontWeight = label.FontWeight,
                FontStretch = label.FontStretch,
                FlowDirection = label.FlowDirection,
                FontSize = label.FontSize,
                Foreground = label.Foreground,
                ActualWidth = label.ActualWidth,
                ActualHeight = label.ActualHeight
            };
        }

        private class FontInfo
        {
            public FontFamily FontFamily { get; set; }
            public FontStyle FontStyle { get; set; }
            public FontWeight FontWeight { get; set; }
            public FontStretch FontStretch { get; set; }
            public FlowDirection FlowDirection { get; set; }
            public double FontSize { get; set; }
            public Brush Foreground { get; set; }
            public double ActualWidth { get; set; }
            public double ActualHeight { get; set; }
        }
    }
}
