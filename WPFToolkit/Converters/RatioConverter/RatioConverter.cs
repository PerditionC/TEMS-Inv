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
    /// WPF converter to multiply bound value based on supplied ratio
    /// Derived from https://stackoverflow.com/questions/8121906/resize-wpf-window-and-contents-depening-on-screen-resolution
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class RatioConverter : MarkupExtension, IValueConverter, IMultiValueConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static RatioConverter _instance;

        public RatioConverter() { }

        // returns Source value multiplied by ratio (parameter)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            logger.Debug("RatioConverter Convert");

            double ratio = System.Convert.ToDouble(value);

            // do not let the culture default to local to prevent variable outcome due to decimal syntax
            double size = ratio * System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);

            return size.ToString("G0", CultureInfo.InvariantCulture);
        }

        // first parameter source value, 2nd parameter is the ratio (multiplier), and optional 3rd parameter is the max value
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            logger.Debug("RatioConverter Convert");

            double ratio;
            if (values.Length < 1)
            {
                logger.Error("RatioConverter called without base value!");
                throw new ArgumentException("Missing required base value argument!");
            }
            if (values.Length < 2)
            {
                logger.Warn("RatioConverter called with no ratio provided!  Using ratio of 1.");
                ratio = 1.0;
            }
            else
            {
                ratio = System.Convert.ToDouble(values[1]);
            }

            // do not let the culture default to local to prevent variable outcome due to decimal syntax
            double size = ratio * System.Convert.ToDouble(values[0], CultureInfo.InvariantCulture);

            // if max size provided, cap to max value
            if (values.Length > 2)
            {
                double maxSize = System.Convert.ToDouble(values[2]);
                if (size > maxSize)
                    size = maxSize;
            }

            return size.ToString("G0", CultureInfo.InvariantCulture);
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // read only converter...
            logger.Debug("RatioConverter ConvertBack");
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // read only converter...
            logger.Debug("RatioConverter ConvertBack");
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new RatioConverter());
        }
    }
}
