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

namespace DW.WPFToolkit.Interactivity
{
    /// <summary>
    /// Holds the information if the window can be closed or not. If yes it holds also the information how the DialogResult has to be. This object is used by the <see cref="DW.WPFToolkit.Interactivity.WindowBehavior" />.
    /// </summary>
    public class WindowClosingArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Interactivity.WindowClosingArgs" /> class.
        /// </summary>
        public WindowClosingArgs()
        {
            DialogResult = true;
        }

        /// <summary>
        /// Gets or sets the value which indicates how to close the dialog if <see cref="DW.WPFToolkit.Interactivity.WindowClosingArgs.Cancel" /> is false. The default is true.
        /// </summary>
        public bool DialogResult { get; set; }

        /// <summary>
        /// Gets or sets the value to define if the closing process has to be canceled and the window to stay open.
        /// </summary>
        public bool Cancel { get; set; }
    }
}
