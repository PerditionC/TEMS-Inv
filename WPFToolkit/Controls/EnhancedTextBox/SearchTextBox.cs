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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Adds search and cancel buttons to the EnhancedTextBox to represent a search box shown like in the Windows explorer.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <WPFToolkit:SearchTextBox ShowSearchButton="True"
    ///                           SearchCommand="{Binding SearchCommand}"
    ///                           IsSearching="{Binding IsSearching}"
    ///                           CancelCommand="{Binding CancelSearchCommand}" />
    /// ]]>
    /// </code>
    /// </example>
    public class SearchTextBox : EnhancedTextBox
    {
        static SearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchTextBox), new FrameworkPropertyMetadata(typeof(SearchTextBox)));
        }

        /// <summary>
        /// Gets or sets a value which indicates where the search button has to be placed.
        /// </summary>
        [DefaultValue(Dock.Right)]
        public Dock SearchButtonPosition
        {
            get { return (Dock)GetValue(SearchButtonPositionProperty); }
            set { SetValue(SearchButtonPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SearchTextBox.SearchButtonPosition" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SearchButtonPositionProperty =
            DependencyProperty.Register("SearchButtonPosition", typeof(Dock), typeof(SearchTextBox), new UIPropertyMetadata(Dock.Right));

        /// <summary>
        /// Gets or sets the margin of the search button.
        /// </summary>
        public Thickness SearchButtonMargin
        {
            get { return (Thickness)GetValue(SearchButtonMarginProperty); }
            set { SetValue(SearchButtonMarginProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SearchTextBox.SearchButtonMargin" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SearchButtonMarginProperty =
            DependencyProperty.Register("SearchButtonMargin", typeof(Thickness), typeof(SearchTextBox));

        /// <summary>
        /// Gets or sets the padding of the search button.
        /// </summary>
        public Thickness SearchButtonPadding
        {
            get { return (Thickness)GetValue(SearchButtonPaddingProperty); }
            set { SetValue(SearchButtonPaddingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SearchTextBox.SearchButtonPadding" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SearchButtonPaddingProperty =
            DependencyProperty.Register("SearchButtonPadding", typeof(Thickness), typeof(SearchTextBox));

        /// <summary>
        /// Gets or sets the vertical alignment of the search button.
        /// </summary>
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment VerticalSearchButtonAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalSearchButtonAlignmentProperty); }
            set { SetValue(VerticalSearchButtonAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SearchTextBox.VerticalSearchButtonAlignment" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalSearchButtonAlignmentProperty =
            DependencyProperty.Register("VerticalSearchButtonAlignment", typeof(VerticalAlignment), typeof(SearchTextBox), new UIPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Gets or sets the horizontal alignment of the search button.
        /// </summary>
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment HorizontalSearchButtonAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalSearchButtonAlignmentProperty); }
            set { SetValue(HorizontalSearchButtonAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SearchTextBox.HorizontalSearchButtonAlignment" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalSearchButtonAlignmentProperty =
            DependencyProperty.Register("HorizontalSearchButtonAlignment", typeof(HorizontalAlignment), typeof(SearchTextBox), new UIPropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Gets or sets a value which indicates if the search button is visible or not. This has effect on the cancel button too.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowSearchButton
        {
            get { return (bool)GetValue(ShowSearchButtonProperty); }
            set { SetValue(ShowSearchButtonProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SearchTextBox.ShowSearchButton" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowSearchButtonProperty =
            DependencyProperty.Register("ShowSearchButton", typeof(bool), typeof(SearchTextBox), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets the command to be called by the search button.
        /// </summary>
        [DefaultValue(null)]
        public ICommand SearchCommand
        {
            get { return (ICommand)GetValue(SearchCommandProperty); }
            set { SetValue(SearchCommandProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SearchTextBox.SearchCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register("SearchCommand", typeof(ICommand), typeof(SearchTextBox), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter to be passed when the <see cref="DW.WPFToolkit.Controls.SearchTextBox.SearchCommand" /> gets executed.
        /// </summary>
        [DefaultValue(null)]
        public object SearchCommandParameter
        {
            get { return (object)GetValue(SearchCommandParameterProperty); }
            set { SetValue(SearchCommandParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SearchTextBox.SearchCommandParameter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SearchCommandParameterProperty =
            DependencyProperty.Register("SearchCommandParameter", typeof(object), typeof(SearchTextBox), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the command to be called by the cancel button.
        /// </summary>
        [DefaultValue(null)]
        public ICommand CancelCommand
        {
            get { return (ICommand)GetValue(CancelCommandProperty); }
            set { SetValue(CancelCommandProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SearchTextBox.CancelCommand" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CancelCommandProperty =
            DependencyProperty.Register("CancelCommand", typeof(ICommand), typeof(SearchTextBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter to be passed when the <see cref="DW.WPFToolkit.Controls.SearchTextBox.CancelCommand" /> gets executed.
        /// </summary>
        [DefaultValue(null)]
        public object CancelCommandParameter
        {
            get { return (object)GetValue(CancelCommandParameterProperty); }
            set { SetValue(CancelCommandParameterProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SearchTextBox.CancelCommandParameter" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CancelCommandParameterProperty =
            DependencyProperty.Register("CancelCommandParameter", typeof(object), typeof(SearchTextBox), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value which indicates if the search or cancel button is visible. If true the cancel button is shown; otherwise the search button.
        /// </summary>
        [DefaultValue(false)]
        public bool IsSearching
        {
            get { return (bool)GetValue(IsSearchingProperty); }
            set { SetValue(IsSearchingProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SearchTextBox.IsSearching" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSearchingProperty =
            DependencyProperty.Register("IsSearching", typeof(bool), typeof(SearchTextBox), new UIPropertyMetadata(false));
    }
}
