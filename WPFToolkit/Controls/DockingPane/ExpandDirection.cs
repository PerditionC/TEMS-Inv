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
    /// Defines in which direction the <see cref="DW.WPFToolkit.Controls.DockingPane" /> has to expand.
    /// </summary>
    public enum ExpandDirection
    {
        /// <summary>
        /// The docking pane has to expand from the top to the bottom.
        /// </summary>
        TopToDown,

        /// <summary>
        /// The docking pane has to expand from the right to the left.
        /// </summary>
        RightToLeft,

        /// <summary>
        /// The docking pane has to expand from the bottom to the top.
        /// </summary>
        BottomToUp,

        /// <summary>
        /// The docking pane has to expand from the left to the right.
        /// </summary>
        LeftToRight
    }
}