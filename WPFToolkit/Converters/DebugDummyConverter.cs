// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Diagnostics;
using System.Windows.Data;
using NLog;

namespace DW.WPFToolkit.Converters
{
    /// <summary>
    /// To help debug WPF binding and conversion
    /// In {Binding} add {Binding Path=...,diag:PresentationTraceSources.TraceLevel=High,Converter={StaticResource DebugDummyConverter}}
    /// and in Resource section add e.g.    
    ///     /UserControl.Resources>
    ///         /uc:DebugDummyConverter x:Key="DebugDummyConverter" />
    ///     //UserControl.Resources>
    /// </summary>
    public class DebugDummyConverter : IValueConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug("DebugDummyConverter Convert=>value=" + value?.ToString());
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug("DebugDummyConverter ConvertBack=>value=" + value?.ToString());
            Debugger.Break();
            return value;
        }
    }
}
