// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using NLog;

namespace DW.WPFToolkit.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    public class StringMatchConverter : MarkupExtension, IMultiValueConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static StringMatchConverter _instance;

        public StringMatchConverter() { }

        /// <summary>
        /// compares 1st all provided strings, and if all are the same (ignoring case) then returns true
        /// Note: any number of strings may be passed, they must all match
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>true if all strings match, false if any are null or less than 2 strings provided or no match</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug("StringMatchConverter Convert");

            // if we don't pass in 2 items to compare, then assume no match
            if (values.Length < 2)
            {
                return false;
            }

            // loop through all provide strings and compare to 1st one provided, exit early on first non-match
            for (int i = 1; i < values.Length; i++)
            {
                if (!string.Equals(values[0] as string, values[i] as string, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            logger.Debug("StringMatchConverter ConvertBack");
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new StringMatchConverter());
        }
    }
}
