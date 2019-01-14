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
    /// Defines how the automatic selection of the number in the <see cref="DW.WPFToolkit.Controls.NumberBox" /> should behave.
    /// </summary>
    public enum NumberBoxSelection
    {
        /// <summary>
        /// No automatic selection will be done.
        /// </summary>
        None,

        /// <summary>
        /// The number gets selected when the NumberBox got the focus.
        /// </summary>
        OnFocus,

        /// <summary>
        /// The number will be selected when increment or decrement the value using arrow keys or up/down buttons.
        /// </summary>
        OnUpDown,

        /// <summary>
        /// The number will be selected when the NumberBox got the focus or the value gets incremented or decremented using the arrow keys or up/down buttons.
        /// </summary>
        OnFocusAndUpDown
    }
}