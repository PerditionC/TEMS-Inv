// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows.Data;
using NLog;

namespace DW.WPFToolkit.Converters
{
    /// <summary>
    /// Converts null value to empty string, otherwise returns value
    /// </summary>
    public class NullableToEmptyStringConverter : IValueConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // Convert value to "" if null otherwise returns value
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug("NullableToEmptyStringConverter Convert=>value=" + value?.ToString());
            if (value == null) return "";
            return value;
        }

        // Convert value to null if "" (or null) otherwise returns value
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug("NullableToEmptyStringConverter ConvertBack=>value=" + value?.ToString());

            var s = value as string;
            if (string.IsNullOrEmpty(s)) return null;

            return value;
        }
    }
}
