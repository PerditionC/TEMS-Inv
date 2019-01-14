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

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Defines what the <see cref="DW.WPFToolkit.Controls.NumberBox" /> should do when it lose the focus without a value (null).
    /// </summary>
    public enum ValueBehavior
    {
        /// <summary>
        /// Nothing should be done, the value stays on null.
        /// </summary>
        None,

        /// <summary>
        /// The default number defined by <see cref="DW.WPFToolkit.Controls.NumberBox.DefaultNumber" /> will be placed in.
        /// </summary>
        PlaceDefaultNumber,

        /// <summary>
        /// The minimum value of the number type will be placed in.
        /// </summary>
        /// <remarks>For BigInteger there is no minimum, it stays on null.</remarks>
        PlaceMinimumNumber,

        /// <summary>
        /// The maximum value of the number type will be placed in.
        /// </summary>
        /// <remarks>For BigInteger there is no maximum, it stays on null.</remarks>
        PlaceMaximumNumber
    }
}