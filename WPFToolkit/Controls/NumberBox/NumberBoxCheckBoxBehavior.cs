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
    /// Defines what should happen to the  <see cref="DW.WPFToolkit.Controls.NumberBox" /> if the internal checkbox is checked.
    /// </summary>
    public enum NumberBoxCheckBoxBehavior
    {
        /// <summary>
        /// Nothing happens internally.
        /// </summary>
        None,

        /// <summary>
        /// The <see cref="DW.WPFToolkit.Controls.NumberBox" /> input field is disabled if the box is checked.
        /// </summary>
        DisableIfChecked,

        /// <summary>
        /// The <see cref="DW.WPFToolkit.Controls.NumberBox" /> input field is enabled if the box is checked.
        /// </summary>
        EnableIfChecked
    }
}
