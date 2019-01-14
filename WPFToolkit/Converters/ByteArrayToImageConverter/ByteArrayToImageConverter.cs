// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NLog;

namespace DW.WPFToolkit.Converters
{
    /// <summary>
    /// converts raw image file data as byte[] to a System.Windows.Controls.Image
    /// </summary>
    //[ValueConversion(typeof(byte[]), typeof(BitmapImage))]
    public class ByteArrayToImageConverter : IValueConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static BitmapImage ByteArrayToBitmapImage(byte[] imageBuffer)
        {
            try
            {
                if (imageBuffer == null || imageBuffer.Length == 0) return null;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(imageBuffer);
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to convert byte[] to BitmapImage - {e.Message}");
                throw;
            }
        }

        // Convert image data [value] to Image object
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug($"ByteArrayToImageConverter Convert()");
            if (value == null) return null;

            if (targetType != typeof(ImageSource))
            {
                throw new InvalidOperationException("The target must be a Image");
            }
            var imageBuffer = value as byte[];
            return ByteArrayToBitmapImage(imageBuffer);
        }

        // Convert Image object to raw image data
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug($"ByteArrayToImageConverter ConvertBack()");
            throw new NotImplementedException("ByteArrayToImageConverter.ConvertBack not currently implemented!");
        }
    }
}
