// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using NLog;

namespace DW.WPFToolkit.Converters
{
    /// <summary>
    /// returns the result of ANDing multiple boolean values
    /// </summary>
    public class AndMultiValueConverter : MarkupExtension, IMultiValueConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static AndMultiValueConverter _instance;

        // Convert enum [value] to boolean, true if matches [param]
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
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
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            logger.Debug("AndMultiValueConverter ConvertBack");
            throw new NotImplementedException();
        }

        // return a instance of converter so don't have to explicitly create
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new AndMultiValueConverter());
        }
    }
}
