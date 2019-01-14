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
    /// Defines when the tooltip has to be created by the <see cref="DW.WPFToolkit.Interactivity.CutTooltipBehavior" />.
    /// </summary>
    public enum CutTextKind
    {
        /// <summary>
        /// The auto tooltip is disbaled.
        /// </summary>
        None,

        /// <summary>
        /// The tooltip appears when the text is longer than the available space.
        /// </summary>
        Width,

        /// <summary>
        /// The tooltip appears when the text height is highter than the available space.
        /// </summary>
        Height,

        /// <summary>
        /// The tooltip appears when the text length and height is more than the available space.
        /// </summary>
        WithAndHeight
    }
}
