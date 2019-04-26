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
using System.ComponentModel;
using System.Windows.Markup;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Defines the actions which should be done when the <see cref="DW.WPFToolkit.Controls.NumberBox" /> losts the focus.
    /// </summary>
    public class LostFocusBehavior : MarkupExtension
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.LostFocusBehavior" /> class.
        /// </summary>
        public LostFocusBehavior()
            : this(ValueBehavior.None)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.LostFocusBehavior" /> class.
        /// </summary>
        /// <param name="value">The behavior for the input value when null.</param>
        public LostFocusBehavior(ValueBehavior value)
        {
            Value = value;
            TrimLeadingZero = false;
            FormatText = null;
        }

        /// <summary>
        /// Gets or sets the behavior to be applied to the NumberBox value when it its empty (null).
        /// </summary>
        /// <remarks>Default value is ValueBehavior.None.</remarks>
        [DefaultValue(ValueBehavior.None)]
        public ValueBehavior Value { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if leading zero's should be trimmed from the value or not.
        /// </summary>
        /// <remarks>The default value is false.</remarks>
        [DefaultValue(false)]
        public bool TrimLeadingZero { get; set; }

        /// <summary>
        /// Gets or sets the format text (string.Format) to be applied to the text.
        /// </summary>
        /// <remarks>The default value is null. No format.</remarks>
        [DefaultValue(null)]
        public string FormatText { get; set; }

        /// <summary>
        /// Returns the object with its configured behaviors.
        /// </summary>
        /// <param name="serviceProvider">Not Used</param>
        /// <returns>The object with its configured behaviors.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}