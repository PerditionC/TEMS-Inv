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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Returns the description of a given enum value.
    /// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {
        /// <summary>
        /// Takes the value and tries to cast it to System.Enum to read it description attribute.
        /// </summary>
        /// <param name="value">The enum value which description should be read.</param>
        /// <param name="targetType">This parameter is not used.</param>
        /// <param name="parameter">This parameter is not used.</param>
        /// <param name="culture">This parameter is not used.</param>
        /// <returns>The description text of the enum value if any; the enum name as a string. If the parameter is not an System.Enum it returns string.Empty</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Enum))
                return string.Empty;

            return GetDescription((Enum)value);
        }

        private static string GetDescription(Enum @enum)
        {
            var enumType = @enum.GetType();
            var enumField = enumType.GetField(@enum.ToString());

            var descriptionAttribute = Attribute.GetCustomAttribute(enumField, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return descriptionAttribute == null ? enumField.ToString() : descriptionAttribute.Description;
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
            throw new NotImplementedException();
        }
    }
}
