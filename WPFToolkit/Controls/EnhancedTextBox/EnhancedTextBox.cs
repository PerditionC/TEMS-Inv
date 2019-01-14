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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Enhances the <see cref="System.Windows.Controls.TextBox" /> by the possibilities to show background text, drop files and folders and place additional controls in.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <WPFToolkit:EnhancedTextBox InfoText="Required" AllowedDropType="Files" Separator=";" />
    /// ]]>
    /// </code>
    /// </example>
    [TemplatePart(Name = "PART_InfoText", Type = typeof(TextBlock))]
    public class EnhancedTextBox : TextBox
    {
        static EnhancedTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EnhancedTextBox), new FrameworkPropertyMetadata(typeof(EnhancedTextBox)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox" /> class.
        /// </summary>
        public EnhancedTextBox()
        {
            Loaded += InfoTextBox_Loaded;
            PreviewDragOver += DroppableTextBox_PreviewDragOver;
            PreviewDrop += DroppableTextBox_PreviewDrop;
        }

        private void InfoTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshInfoAppearance();
        }

        private TextBlock _infoText;

        /// <summary>
        /// The template gets added to the control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            RefreshInfoAppearance();
        }

        /// <summary>
        /// Takes care about hiding the info text in the background depending on the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.InfoAppearance" /> property.
        /// </summary>
        /// <param name="e">The parameter passed by the caller.</param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (GetInfoTextBlock())
            {
                if (InfoAppearance != InfoAppearance.OnEmpty)
                    _infoText.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Takes care about display the info text in the background depending on the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.InfoAppearance" /> property.
        /// </summary>
        /// <param name="e">The parameter passed by the caller.</param>
        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if (GetInfoTextBlock())
            {
                if (InfoAppearance != InfoAppearance.None &&
                    string.IsNullOrEmpty(Text))
                    _infoText.Visibility = System.Windows.Visibility.Visible;
            }
        }

        /// <summary>
        /// Takes care about display or hide the info text in the background depending on the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.InfoAppearance" /> property.
        /// </summary>
        /// <param name="e">The parameter passed by the caller.</param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            if (GetInfoTextBlock())
            {
                if (string.IsNullOrEmpty(Text))
                {
                    if (InfoAppearance == InfoAppearance.OnEmpty)
                        _infoText.Visibility = Visibility.Visible;
                }
                else if (_infoText.Visibility == Visibility.Visible)
                    _infoText.Visibility = Visibility.Collapsed;
            }
        }

        private void RefreshInfoAppearance()
        {
            if (GetInfoTextBlock())
            {
                if (InfoAppearance == InfoAppearance.None ||
                    !string.IsNullOrEmpty(Text))
                    _infoText.Visibility = Visibility.Collapsed;
            }
        }

        private bool GetInfoTextBlock()
        {
            if (_infoText == null)
                _infoText = GetTemplateChild("PART_InfoText") as TextBlock;
            return _infoText != null;
        }

        private void DroppableTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (!string.IsNullOrEmpty(GetContent(e)))
                e.Effects = DragDropEffect;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private void DroppableTextBox_PreviewDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;
            var contentText = GetContent(e);
            if (string.IsNullOrEmpty(contentText))
                return;
            ((TextBox)sender).Text = contentText;
            this.Focus();
        }

        private string GetContent(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var content = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (content != null)
                {
                    switch (AllowedDropType)
                    {
                        case DroppableTypes.File:
                            if (content.Length == 1 &&
                                File.Exists(content[0]))
                                return content[0];
                            break;
                        case DroppableTypes.Files:
                            foreach (var value in content)
                            {
                                if (!File.Exists(value))
                                    return null;
                            }
                            return string.Join(Separator, content);
                        case DroppableTypes.FilesFolders:
                            foreach (var value in content)
                            {
                                if (!File.Exists(value) &&
                                    !Directory.Exists(value))
                                    return null;
                            }
                            return string.Join(Separator, content);
                        case DroppableTypes.Folder:
                            if (content.Length == 1 &&
                                Directory.Exists(content[0]))
                                return content[0];
                            break;
                        case DroppableTypes.Folders:
                            foreach (string value in content)
                            {
                                if (!Directory.Exists(value))
                                    return null;
                            }
                            return string.Join(Separator, content);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets or sets a value which indicates when the info text in the background is shown.
        /// </summary>
        [DefaultValue(InfoAppearance.OnLostFocus)]
        public InfoAppearance InfoAppearance
        {
            get { return (InfoAppearance)GetValue(InfoAppearanceProperty); }
            set { SetValue(InfoAppearanceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.InfoAppearance" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoAppearanceProperty =
            DependencyProperty.Register("InfoAppearance", typeof(InfoAppearance), typeof(EnhancedTextBox), new UIPropertyMetadata(InfoAppearance.OnLostFocus, OnInfoAppearanceChanged));

        private static void OnInfoAppearanceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((EnhancedTextBox)sender).RefreshInfoAppearance();
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
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.InfoText" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextProperty =
            DependencyProperty.Register("InfoText", typeof(string), typeof(EnhancedTextBox), new UIPropertyMetadata(""));

        /// <summary>
        /// Gets or sets the font style of the info text shown in the background.
        /// </summary>
        public FontStyle InfoTextFontStyle
        {
            get { return (FontStyle)GetValue(InfoTextFontStyleProperty); }
            set { SetValue(InfoTextFontStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.InfoTextFontStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextFontStyleProperty =
            DependencyProperty.Register("InfoTextFontStyle", typeof(FontStyle), typeof(EnhancedTextBox), new UIPropertyMetadata(FontStyles.Italic));

        /// <summary>
        /// Gets or sets the foreground color of the info text shown in the background
        /// </summary>
        public Brush InfoTextForeground
        {
            get { return (Brush)GetValue(InfoTextForegroundProperty); }
            set { SetValue(InfoTextForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.InfoTextForeground" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextForegroundProperty =
            DependencyProperty.Register("InfoTextForeground", typeof(Brush), typeof(EnhancedTextBox), new UIPropertyMetadata(Brushes.Gray));

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
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.InfoTextHorizontalAlignment" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextHorizontalAlignmentProperty =
            DependencyProperty.Register("InfoTextHorizontalAlignment", typeof(HorizontalAlignment), typeof(EnhancedTextBox), new UIPropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// Gets or sets the vertical alignment info text shown in the background.
        /// </summary>
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment InfoTextVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(InfoTextVerticalAlignmentProperty); }
            set { SetValue(InfoTextVerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.InfoTextVerticalAlignment" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextVerticalAlignmentProperty =
            DependencyProperty.Register("InfoTextVerticalAlignment", typeof(VerticalAlignment), typeof(EnhancedTextBox), new UIPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Gets or sets the margin for the info text shown in the background.
        /// </summary>
        public Thickness InfoTextMargin
        {
            get { return (Thickness)GetValue(InfoTextMarginProperty); }
            set { SetValue(InfoTextMarginProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.InfoTextMargin" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextMarginProperty =
            DependencyProperty.Register("InfoTextMargin", typeof(Thickness), typeof(EnhancedTextBox));

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
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.InfoTextStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InfoTextStyleProperty =
            DependencyProperty.Register("InfoTextStyle", typeof(Style), typeof(EnhancedTextBox), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets an additional control placed inside the text box.
        /// </summary>
        [DefaultValue(null)]
        public object FirstControl
        {
            get { return (object)GetValue(FirstControlProperty); }
            set { SetValue(FirstControlProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.FirstControl" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstControlProperty =
            DependencyProperty.Register("FirstControl", typeof(object), typeof(EnhancedTextBox), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value which indicates where the additional <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.FirstControl" /> has to be placed in the text box.
        /// </summary>
        [DefaultValue(Dock.Left)]
        public Dock FirstControlPosition
        {
            get { return (Dock)GetValue(FirstControlPositionProperty); }
            set { SetValue(FirstControlPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.FirstControlPosition" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstControlPositionProperty =
            DependencyProperty.Register("FirstControlPosition", typeof(Dock), typeof(EnhancedTextBox), new UIPropertyMetadata(Dock.Left));

        /// <summary>
        /// Gets or sets an second additional control placed inside the text box.
        /// </summary>
        [DefaultValue(null)]
        public object SecondControl
        {
            get { return (object)GetValue(SecondControlProperty); }
            set { SetValue(SecondControlProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.SecondControl" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondControlProperty =
            DependencyProperty.Register("SecondControl", typeof(object), typeof(EnhancedTextBox), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value which indicates where the additional <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.SecondControl" /> has to be placed in the text box.
        /// </summary>
        [DefaultValue(Dock.Right)]
        public Dock SecondControlPosition
        {
            get { return (Dock)GetValue(SecondControlPositionProperty); }
            set { SetValue(SecondControlPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.SecondControlPosition" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondControlPositionProperty =
            DependencyProperty.Register("SecondControlPosition", typeof(Dock), typeof(EnhancedTextBox), new UIPropertyMetadata(Dock.Right));

        /// <summary>
        /// Gets or sets a value which indicates what the text box allows to drop in.
        /// </summary>
        [DefaultValue(DroppableTypes.File)]
        public DroppableTypes AllowedDropType
        {
            get { return (DroppableTypes)GetValue(AllowedDropTypeProperty); }
            set { SetValue(AllowedDropTypeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.AllowedDropType" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowedDropTypeProperty =
            DependencyProperty.Register("AllowedDropType", typeof(DroppableTypes), typeof(EnhancedTextBox), new UIPropertyMetadata(DroppableTypes.File));

        /// <summary>
        /// Gets or sets a value which will be used as a separator if multiple elements can be dropped to the textbox. See <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.AllowedDropType" />.
        /// </summary>
        [DefaultValue("; ")]
        public string Separator
        {
            get { return (string)GetValue(SeparatorProperty); }
            set { SetValue(SeparatorProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.Separator" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeparatorProperty =
            DependencyProperty.Register("Separator", typeof(string), typeof(EnhancedTextBox), new UIPropertyMetadata("; "));

        /// <summary>
        /// Gets or sets the mouse icon when files or folders (See <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.AllowedDropType" />) will be dropped into the text box.
        /// </summary>
        [DefaultValue(DragDropEffects.Link)]
        public DragDropEffects DragDropEffect
        {
            get { return (DragDropEffects)GetValue(DragDropEffectProperty); }
            set { SetValue(DragDropEffectProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.EnhancedTextBox.DragDropEffect" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DragDropEffectProperty =
            DependencyProperty.Register("DragDropEffect", typeof(DragDropEffects), typeof(EnhancedTextBox), new UIPropertyMetadata(DragDropEffects.Link));
    }
}
