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

using System;
using System.Globalization;
using System.Windows.Data;
using NLog;

namespace DW.WPFToolkit.Converters
{
    /// <summary>
    /// Represents the converter that converts the state if the parameter is null to a boolean value depending on the parameter.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <StackPanel>
    ///     <StackPanel.Resources>
    ///         <Converters:NullToBooleanConverter x:Key="NullToBooleanConverter" />
    ///     </StackPanel.Resources>
    /// 
    ///     <Label Content="Element Is There" IsEnabled="{Binding Element, Converter={StaticResource NullToBooleanConverter}, ConverterParameter={x:Static Converters:NullToBooleanDirection.NullIsFalse}}" />
    ///     
    /// </StackPanel>
    /// ]]>
    /// </code>
    /// </example>
    public sealed class NullToBooleanConverter : IValueConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Converts the null state of an object to a boolean representation depending on the parameter.
        /// </summary>
        /// <param name="value">The object to check for null.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">The <see cref="NullToBooleanDirection" /> which defines what to return.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>True if the value is not null; otherwise false. With the <see cref="NullToBooleanDirection.NullIsTrue" /> its opposite.</returns>
        /// <remarks>The default of the <see cref="NullToBooleanDirection" /> is <see cref="NullToBooleanDirection.NullIsFalse" />.</remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            logger.Debug("NullToBooleanConverter Convert=>value is " + ((value != null) ? "not" : "") + " null");

            var direction = NullToBooleanDirection.NullIsFalse;
            if (parameter is NullToBooleanDirection)
                direction = (NullToBooleanDirection)parameter;

            if (value == null)
            {
                if (direction == NullToBooleanDirection.NullIsFalse)
                    return false;
                return true;
            }
            if (direction == NullToBooleanDirection.NullIsFalse)
                return true;
            return false;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">This parameter is not used.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>nothing</returns>
        /// <exception cref="System.NotImplementedException">The convert back is not intended to be used.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            logger.Debug("NullToBooleanConverter ConvertBack=>value is " + ((value != null) ? "not" : "") + " null");
            throw new NotImplementedException();
        }
    }
}
