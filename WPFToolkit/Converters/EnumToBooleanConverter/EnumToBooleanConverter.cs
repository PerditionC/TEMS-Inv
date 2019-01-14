// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// for description of basic idea see https://stackoverflow.com/questions/397556/how-to-bind-radiobuttons-to-an-enum
using System;
using System.Windows.Data;
using NLog;

namespace DW.WPFToolkit.Converters
{
    /// <summary>
    /// To allow using radio buttons to get/set an Enum properter
    /// Create multiple RadioButtons and bind similarly
    ///     /RadioButton IsChecked="{Binding Path=MyProperty, Converter={StaticResource enumToBooleanConverter}, 
    ///         ConverterParameter={x:Static DataModel:MyEnumType.MyValue}}" Content="My value description"/>
    /// and in Resource section add e.g.    
    ///     /UserControl.Resources>
    ///         /uc:EnumToBooleanConverter x:Key="enumToBooleanConverter" />
    ///     //UserControl.Resources>
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // Convert enum [value] to boolean, true if matches [param]
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug("EnumToBooleanConverter Convert=>value=" + value?.ToString());
            return value.Equals(parameter);
        }

        // Convert boolean to enum, returning [param] if true
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug("EnumToBooleanConverter ConvertBack=>value=" + value?.ToString());
            return (bool)value ? parameter : Binding.DoNothing;
        }
    }
}
