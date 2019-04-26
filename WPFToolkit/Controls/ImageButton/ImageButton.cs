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
using System.Windows.Media.Imaging;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Enhances the <see cref="System.Windows.Controls.Button" /> to show an disabled image. The bound image will be shown monochrome if the button is disabled.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <UniformGrid Rows="1" DockPanel.Dock="Bottom" HorizontalAlignment="Center">
    /// 
    ///     <Toolkit:ImageButton Content="Back"
    ///                          ImageSource="/MyAssembly;component/Data/Previous.png" />
    /// 
    ///     <Toolkit:ImageButton Content="Next"
    ///                          ImageSource="/MyAssembly.Demo;component/Data/Next.png" 
    ///                          ImagePosition="Right"
    ///                          ImageMargin="4,0,0,0" />
    /// 
    ///     <Toolkit:ImageButton Content="Finish"
    ///                          IsEnabled="False"
    ///                          ImageSource="/MyAssembly;component/Data/OK.png" />
    /// 
    ///     <Toolkit:ImageButton Content="Cancel"
    ///                          ImageSource="/MyAssembly;component/Data/Cancel.png" />
    /// 
    /// </UniformGrid>
    /// ]]>
    /// </code>
    /// </example>
    public class ImageButton : Button
    {
        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }

        /// <summary>
        /// Gets or sets the source if the button image.
        /// </summary>
        [DefaultValue(null)]
        public BitmapSource ImageSource
        {
            get { return (BitmapSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ImageButton.ImageSource" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(BitmapSource), typeof(ImageButton), new UIPropertyMetadata(null, OnImageSourceChanged));

        private static void OnImageSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = (ImageButton)o;
            if (e.NewValue != null &&
                !control._sizeIsSet)
            {
                var bitmapSource = e.NewValue as BitmapSource;
                control.DisabledImageSource = new FormatConvertedBitmap(bitmapSource, PixelFormats.Gray32Float, null, 0);
                control.ImageHeight = bitmapSource.Height;
                control.ImageWidth = bitmapSource.Width;
            }
        }

        /// <summary>
        /// Gets or sets the image to be used when the button is disabled. This will set internaly by the <see cref="DW.WPFToolkit.Controls.ImageButton.ImageSource" /> to a monochrome image.
        /// </summary>
        [DefaultValue(null)]
        public BitmapSource DisabledImageSource
        {
            get { return (BitmapSource)GetValue(DisabledImageSourceProperty); }
            set { SetValue(DisabledImageSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ImageButton.DisabledImageSource" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisabledImageSourceProperty =
            DependencyProperty.Register("DisabledImageSource", typeof(BitmapSource), typeof(ImageButton), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the width of the image shown in the button.
        /// </summary>
        [DefaultValue(16.0)]
        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ImageButton.ImageWidth" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageButton), new UIPropertyMetadata(16.0, OnSizeChanged));

        /// <summary>
        /// Gets or sets the height of the image shown in the button.
        /// </summary>
        [DefaultValue(16.0)]
        public double ImageHeight
        {
            get { return (double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ImageButton.ImageHeight" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(ImageButton), new UIPropertyMetadata(16.0, OnSizeChanged));

        private bool _sizeIsSet;

        private static void OnSizeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var control = (ImageButton)o;
            control._sizeIsSet = true;
        }

        /// <summary>
        /// Gets or sets the margin of the image shown in the button.
        /// </summary>
        public Thickness ImageMargin
        {
            get { return (Thickness)GetValue(ImageMarginProperty); }
            set { SetValue(ImageMarginProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ImageButton.ImageMargin" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(ImageButton), new UIPropertyMetadata(new Thickness(0, 0, 2, 0)));

        /// <summary>
        /// Gets or sets a value that indicates where the image have to be placed in the button.
        /// </summary>
        [DefaultValue(Dock.Left)]
        public Dock ImagePosition
        {
            get { return (Dock)GetValue(ImagePositionProperty); }
            set { SetValue(ImagePositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ImageButton.ImagePosition" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImagePositionProperty =
            DependencyProperty.Register("ImagePosition", typeof(Dock), typeof(ImageButton), new UIPropertyMetadata(Dock.Left));

        /// <summary>
        /// Gets or sets the horizontal alignment of the image shown in the button.
        /// </summary>
        [DefaultValue(HorizontalAlignment.Center)]
        public HorizontalAlignment HorizontalImageAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalImageAlignmentProperty); }
            set { SetValue(HorizontalImageAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ImageButton.HorizontalImageAlignment" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalImageAlignmentProperty =
            DependencyProperty.Register("HorizontalImageAlignment", typeof(HorizontalAlignment), typeof(ImageButton), new UIPropertyMetadata(HorizontalAlignment.Center));

        /// <summary>
        /// Gets or sets the vertical alignment of the image shown in the button.
        /// </summary>
        [DefaultValue(VerticalAlignment.Center)]
        public VerticalAlignment VerticalImageAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalImageAlignmentProperty); }
            set { SetValue(VerticalImageAlignmentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ImageButton.VerticalImageAlignment" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalImageAlignmentProperty =
            DependencyProperty.Register("VerticalImageAlignment", typeof(VerticalAlignment), typeof(ImageButton), new UIPropertyMetadata(VerticalAlignment.Center));

        /// <summary>
        /// Gets or sets a value that indicated how the image have to be stretched in the button.
        /// </summary>
        [DefaultValue(Stretch.Uniform)]
        public Stretch ImageStretch
        {
            get { return (Stretch)GetValue(ImageStretchProperty); }
            set { SetValue(ImageStretchProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.ImageButton.ImageStretch" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageStretchProperty =
            DependencyProperty.Register("ImageStretch", typeof(Stretch), typeof(ImageButton), new UIPropertyMetadata(Stretch.Uniform));
    }
}
