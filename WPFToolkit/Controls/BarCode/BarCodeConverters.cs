// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows;
using System.Windows.Data;
using System.Drawing;
using BarcodeLib;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace DW.WPFToolkit.Controls
{
    [ValueConversion(typeof(string), typeof(BitmapSource))]
    public class BarcodeToBitmapSourceConverter : IValueConverter
    {
        private static Barcode barcode = new Barcode("00012345689-1", TYPE.CODE128, Width: 200, Height: 50, IncludeLabel: true);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ImageToBitmapSourceConverter.DoConversion(barcode.Encode((string)value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // https://stackoverflow.com/questions/10077498/show-drawing-image-in-wpf
    [ValueConversion(typeof(System.Drawing.Image), typeof(BitmapSource))]
    public class ImageToBitmapSourceConverter : IValueConverter
    {
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DoConversion((System.Drawing.Image)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static BitmapSource DoConversion(System.Drawing.Image myImage)
        {
            var bitmap = new Bitmap(myImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());

            //freeze bitmapSource and clear memory to avoid memory leaks
            bitmapSource.Freeze();
            DeleteObject(bmpPt);

            return bitmapSource;
        }
    }
}
