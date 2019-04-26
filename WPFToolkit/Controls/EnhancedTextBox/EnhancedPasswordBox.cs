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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Hosts and enhances the <see cref="System.Windows.Controls.PasswordBox"/> to be able to bind the pasword value and show info text in the background.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <WPFToolkit:EnhancedPasswordBox Password="{Binding Password}" InfoText="Required" InfoAppearance="OnEmpty" />
    /// ]]>
    /// </code>
    /// </example>
    [TemplatePart(Name = "PART_InfoText", Type = typeof(TextBlock))]
    [TemplatePart(Name = "PART_PasswordBox", Type = typeof(PasswordBox))]
    public class EnhancedPasswordBox : Control
    {
        static EnhancedPasswordBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnhancedPasswordBox), new FrameworkPropertyMetadata(typeof(EnhancedPasswordBox)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.EnhancedPasswordBox" /> class.
        /// </summary>
        public EnhancedPasswordBox()
        {
            Loaded += new RoutedEventHandler(InfoTextBox_Loaded);
        }

        private void InfoTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshInfoAppearance();
            if (!string.IsNullOrEmpty(Password))
                TakePasswordOver(this, Password);
        }

        private bool _selfChange;
        private PasswordBox _innerPasswordBox;
        private TextBlock _infoText;

        /// <summary>
        /// The template gets added to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _innerPasswordBox = GetTemplateChild("PART_PasswordBox") as PasswordBox;
            if (_innerPasswordBox != null)
            {
                _innerPasswordBox.PasswordChanged += PasswordBox_PasswordChanged;

                _innerPasswordBox.GotFocus += InnerPasswordBox_GotFocus;
                _innerPasswordBox.LostFocus += InnerPasswordBox_LostFocus;
                _innerPasswordBox.PasswordChanged += InnerPasswordBox_PasswordChanged;
            }

            RefreshInfoAppearance();
        }

        private void InnerPasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (GetInfoTextBlock())
            {
                if (InfoAppearance != InfoAppearance.OnEmpty)
                    _infoText.Visibility = Visibility.Collapsed;
            }
        }

        private void InnerPasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (GetInfoTextBlock())
            {
                if (InfoAppearance != InfoAppearance.None &&
                    string.IsNullOrEmpty(Password))
                    _infoText.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void InnerPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (GetInfoTextBlock())
            {
                var passwordBox = (PasswordBox)sender;
                if (string.IsNullOrEmpty(passwordBox.Password))
                {
                    if (InfoAppearance == InfoAppearance.OnEmpty)
                        _infoText.Visibility = Visibility.Visible;
                }
                else if (_infoText.Visibility == Visibility.Visible)
                    _infoText.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Moves the focus into the inner password box if the control got the focus.
        /// </summary>
        /// <param name="e">The parameter passed by the caller.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (_innerPasswordBox != null)
                _innerPasswordBox.Focus();
            base.OnGotFocus(e);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var box = (PasswordBox)sender;
            _selfChange = true;
            Password = box.Password;
            _selfChange = false;
        }
        
        private static void OnPasswordChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            TakePasswordOver(o as EnhancedPasswordBox, e.NewValue);
        }

        private static void TakePasswordOver(EnhancedPasswordBox control, object password)
        {
            if (!control._selfChange &&
                password != null &&
                control._innerPasswordBox != null)
            {
                control._innerPasswordBox.PasswordChanged -= control.PasswordBox_PasswordChanged;
                control._innerPasswordBox.Password = password.ToString();
                control._innerPasswordBox.PasswordChanged += control.PasswordBox_PasswordChanged;
            }
        }

        private void RefreshInfoAppearance()
        {
            if (GetInfoTextBlock())
            {
                if (InfoAppearance == InfoAppearance.None)
                    _infoText.Visibility = Visibility.Collapsed;
                if (!string.IsNullOrEmpty(Password))
                    _infoText.Visibility = Visibility.Collapsed;
            }
        }

        private bool GetInfoTextBlock()
        {
            if (_infoText == null)
                _infoText = GetTemplateChild("PART_InfoText") as TextBlock;
            return _infoText != null;
        }

        /// <summary>
        /// Gets or sets a value which indicated when the info text in the background is shown.
        /// </summary>
        [DefaultValue(InfoAppearance.OnLostFocus)]
        public InfoAppearance InfoAppearance
        {
            get { return (InfoAppearance)GetValue(InfoAppearanceProperty); }
            set { SetValue(InfoAppearanceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedPasswordBox.InfoAppearance" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoAppearanceProperty =
            DependencyProperty.Register("InfoAppearance", typeof(InfoAppearance), typeof(EnhancedPasswordBox), new UIPropertyMetadata(InfoAppearance.OnLostFocus, OnInfoAppearanceChanged));

        private static void OnInfoAppearanceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((EnhancedPasswordBox)sender).RefreshInfoAppearance();
        }

        /// <summary>
        /// Gets or sets the info text shown in the background.
        /// </summary>
        [DefaultValue("")]
        public string InfoText
        {
            get { return (string)GetValue(InfoTextProperty); }
            set { SetValue(InfoTextProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedPasswordBox.InfoText" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextProperty =
            DependencyProperty.Register("InfoText", typeof(string), typeof(EnhancedPasswordBox), new UIPropertyMetadata(""));

        /// <summary>
        /// Gets or sets the font style of the info text shown in the background.
        /// </summary>
        public FontStyle InfoTextFontStyle
        {
            get { return (FontStyle)GetValue(InfoTextFontStyleProperty); }
            set { SetValue(InfoTextFontStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedPasswordBox.InfoTextFontStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextFontStyleProperty =
            DependencyProperty.Register("InfoTextFontStyle", typeof(FontStyle), typeof(EnhancedPasswordBox), new UIPropertyMetadata(FontStyles.Italic));

        /// <summary>
        /// Gets or sets the foreground color of the info text shown in the background.
        /// </summary>
        public Brush InfoTextForeground
        {
            get { return (Brush)GetValue(InfoTextForegroundProperty); }
            set { SetValue(InfoTextForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedPasswordBox.InfoTextForeground" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextForegroundProperty =
            DependencyProperty.Register("InfoTextForeground", typeof(Brush), typeof(EnhancedPasswordBox), new UIPropertyMetadata(Brushes.Gray));

        /// <summary>
        /// Gets or sets the horizontal alignment of the info text shown in the background.
        /// </summary>
        [DefaultValue(HorizontalAlignment.Left)]
        public HorizontalAlignment InfoTextHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(InfoTextHorizontalAlignmentProperty); }
            set { SetValue(InfoTextHorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedPasswordBox.InfoTextHorizontalAlignment" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextHorizontalAlignmentProperty =
            DependencyProperty.Register("InfoTextHorizontalAlignment", typeof(HorizontalAlignment), typeof(EnhancedPasswordBox), new UIPropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// Gets or sets the vertical alignment of the info text shown in the background
        /// </summary>
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment InfoTextVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(InfoTextVerticalAlignmentProperty); }
            set { SetValue(InfoTextVerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedPasswordBox.InfoTextVerticalAlignment" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextVerticalAlignmentProperty =
            DependencyProperty.Register("InfoTextVerticalAlignment", typeof(VerticalAlignment), typeof(EnhancedPasswordBox), new UIPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Gets or sets the margin of the info text shown in the background.
        /// </summary>
        public Thickness InfoTextMargin
        {
            get { return (Thickness)GetValue(InfoTextMarginProperty); }
            set { SetValue(InfoTextMarginProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedPasswordBox.InfoTextMargin" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextMarginProperty =
            DependencyProperty.Register("InfoTextMargin", typeof(Thickness), typeof(EnhancedPasswordBox));

        /// <summary>
        /// Gets or sets the style of the info text shown in the background.
        /// </summary>
        [DefaultValue(null)]
        public Style InfoTextStyle
        {
            get { return (Style)GetValue(InfoTextStyleProperty); }
            set { SetValue(InfoTextStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedPasswordBox.InfoTextStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextStyleProperty =
            DependencyProperty.Register("InfoTextStyle", typeof(Style), typeof(EnhancedPasswordBox), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the password typed in the text box.
        /// </summary>
        [DefaultValue("")]
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedPasswordBox.Password" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(EnhancedPasswordBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPasswordChanged));
    }
}
