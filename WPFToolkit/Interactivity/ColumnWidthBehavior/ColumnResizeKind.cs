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

namespace DW.WPFToolkit.Interactivity
{
    /// <summary>
    /// Defines how the columns has to be resized by the <see cref="DW.WPFToolkit.Interactivity.ColumnWidthBehavior" />.
    /// </summary>
    public enum ColumnResizeKind
    {
        /// <summary>
        /// The column widths stays unchanged.
        /// </summary>
        NoResize = -1,

        /// <summary>
        /// The column widths are defined by the owner control width.
        /// </summary>
        ByControl = -2,

        /// <summary>
        /// The column widths are defined by their shown content.
        /// </summary>
        ByContent = -3,

        /// <summary>
        /// The column widths are calculated proportional by the owner control width.
        /// </summary>
        Proportional = -4
    }
}
