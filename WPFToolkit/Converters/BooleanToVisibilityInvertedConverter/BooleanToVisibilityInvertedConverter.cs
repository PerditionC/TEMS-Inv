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
using System.Windows;
using System.Windows.Data;

namespace DW.WPFToolkit.Converters
{
    /// <summary>
    /// Represents the converter that converts Boolean values to and from System.Windows.Visibility enumeration values like the <see cref="System.Windows.Controls.BooleanToVisibilityConverter" /> but in the opposite way.
    /// </summary>
    /// <example>
    /// <code lang="XAML">
    /// <![CDATA[
    /// <StackPanel>
    ///     <StackPanel.Resources>
    ///         <Converters:BooleanToVisibilityInvertedConverter x:Key="BooleanToVisibilityInvertedConverter" />
    ///     </StackPanel.Resources>
    /// 
    ///     <CheckBox Content="Hide" x:Name="HideCheckBox" />
    ///     
    ///     <Label Content="Text" Visibility="{Binding IsChecked, ElementName=HideCheckBox, Converter={StaticResource BooleanToVisibilityInvertedConverter}}" />
    ///     
    /// </StackPanel>
    /// ]]>
    /// </code>
    /// </example>
    public sealed class BooleanToVisibilityInvertedConverter : IValueConverter
    {
        /// <summary>
        /// Converts a Boolean value to a <see cref="System.Windows.Visibility" /> enumeration value.
        /// </summary>
        /// <param name="value">The Boolean value to convert. This value can be a standard Boolean value or a nullable Boolean value.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns><see cref="System.Windows.Visibility.Collapsed" /> if value is true; otherwise, <see cref="System.Windows.Visibility.Visible" />.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = false;
            if (value is bool)
                flag = (bool)value;
            else if (value is bool?)
            {
                var nullable = (bool?)value;
                flag = nullable.HasValue ? nullable.Value : false;
            }
            return (flag ? Visibility.Collapsed : Visibility.Visible);
        }

        /// <summary>
        /// Converts a <see cref="System.Windows.Visibility" /> enumeration value to a Boolean value.
        /// </summary>
        /// <param name="value">A <see cref="System.Windows.Visibility" /> enumeration value.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>true if value is <see cref="System.Windows.Visibility.Collapsed" />; otherwise, false.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((value is Visibility) &&
                    (((Visibility)value) == Visibility.Collapsed));
        }
    }
}
