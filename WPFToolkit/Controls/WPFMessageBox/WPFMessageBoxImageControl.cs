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
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Represents the icon shown in the <see cref="DW.WPFToolkit.Controls.WPFMessageBox" />.
    /// </summary>
    public class WPFMessageBoxImageControl : Control
    {
        static WPFMessageBoxImageControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPFMessageBoxImageControl), new FrameworkPropertyMetadata(typeof(WPFMessageBoxImageControl)));
        }

        /// <summary>
        /// Gets or sets the image to be shown.
        /// </summary>
        [DefaultValue(WPFMessageBoxImage.None)]
        public WPFMessageBoxImage Image
        {
            get { return (WPFMessageBoxImage)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.WPFMessageBoxImageControl.Image" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(WPFMessageBoxImage), typeof(WPFMessageBoxImageControl), new PropertyMetadata(WPFMessageBoxImage.None, OnImageChanged));

        /// <summary>
        /// Gets or sets the BitmapSource which represents the image to show.
        /// </summary>
        [DefaultValue(null)]
        public BitmapSource BitmapSource
        {
            get { return (BitmapSource)GetValue(BitmapSourceProperty); }
            set { SetValue(BitmapSourceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.WPFMessageBoxImageControl.BitmapSource" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty BitmapSourceProperty =
            DependencyProperty.Register("BitmapSource", typeof(BitmapSource), typeof(WPFMessageBoxImageControl), new PropertyMetadata(null));

        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (WPFMessageBoxImageControl)d;
            control.OnImageChanged();
        }

        private void OnImageChanged()
        {
            switch (Image)
            {
                //case WPFMessageBoxImage.Error:
                //case WPFMessageBoxImage.Hand:
                case WPFMessageBoxImage.Stop:
                    BitmapSource = IconToImage(System.Drawing.SystemIcons.Error);
                    break;

                //case WPFMessageBoxImage.Exclamation:
                case WPFMessageBoxImage.Warning:
                    BitmapSource = IconToImage(System.Drawing.SystemIcons.Warning);
                    break;

                //case WPFMessageBoxImage.Asterisk:
                case WPFMessageBoxImage.Information:
                    BitmapSource = IconToImage(System.Drawing.SystemIcons.Information);
                    break;

                case WPFMessageBoxImage.Question:
                    BitmapSource = IconToImage(System.Drawing.SystemIcons.Question);
                    break;
            }
        }

        private BitmapSource IconToImage(System.Drawing.Icon icon)
        {
            return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
