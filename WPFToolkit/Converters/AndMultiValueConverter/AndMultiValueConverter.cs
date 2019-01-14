// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows.Data;
using NLog;

namespace DW.WPFToolkit.Converters
{
    /// <summary>
    /// returns the result of ANDing multiple boolean values
    /// </summary>
    public class AndMultiValueConverter : IMultiValueConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // Convert enum [value] to boolean, true if matches [param]
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug("AndMultiValueConverter Convert");

            bool result = true;
            foreach (object value in values)
            {
                if (value is bool)
                    result = result && (bool)value;
            }

            return result;
        }

        // Convert boolean to enum, returning [param] if true
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug("AndMultiValueConverter ConvertBack");
            throw new NotImplementedException();
        }
    }
}
