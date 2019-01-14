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
    /// Represents the formatter to be used after pressing Ctrl+C with the <see cref="DW.WPFToolkit.Controls.WPFMessageBox" /> open.
    /// </summary>
    public interface IMessageCopyFormatter
    {
        /// <summary>
        /// Called to copy the WPFMessageBox content somewhere to.
        /// </summary>
        /// <param name="title">The WPFMessageBox title.</param>
        /// <param name="message">The message shown in the WPFMessageBox.</param>
        /// <param name="buttons">The buttons available in the WPFMessageBox.</param>
        /// <param name="icon">The icon shown in the WPFMessageBox.</param>
        /// <param name="strings">The strings used in the WPFMessageBox.</param>
        void Copy(string title, string message, WPFMessageBoxButtons buttons, WPFMessageBoxImage icon, MessageBoxStrings strings);
    }
}