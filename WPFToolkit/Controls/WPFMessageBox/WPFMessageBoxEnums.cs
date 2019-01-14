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
    /// Defines which buttons to show in the <see cref="DW.WPFToolkit.Controls.WPFMessageBox" />.
    /// </summary>
    public enum WPFMessageBoxButtons
    {
        /// <summary>
        /// The WPFMessageBox has just the OK button.
        /// </summary>
        OK = 0,

        /// <summary>
        /// The WPFMessageBox has the OK and Cancel buttons.
        /// </summary>
        OKCancel = 1,

        /// <summary>
        /// The WPFMessageBox has the Abort, Retry and Ignore buttons.
        /// </summary>
        AbortRetryIgnore = 2,

        /// <summary>
        /// The WPFMessageBox has the Yes, No and Cancel buttons.
        /// </summary>
        YesNoCancel = 3,

        /// <summary>
        /// The WPFMessageBox has the Yes and No buttons.
        /// </summary>
        YesNo = 4,

        /// <summary>
        /// The WPFMessageBox has the Retry and Cancel buttons.
        /// </summary>
        RetryCancel = 5,

        /// <summary>
        /// The WPFMessageBox has the Cancel, Try Again and Continue buttons.
        /// </summary>
        CancelTryAgainContinue = 6,
    }

    /// <summary>
    /// Represents the result of the <see cref="DW.WPFToolkit.Controls.WPFMessageBox" /> and its default button after calling show.
    /// </summary>
    public enum WPFMessageBoxResult
    {
        /// <summary>
        /// The WPFMessageBox has been closed without a result and has no default button.
        /// </summary>
        None = 0,

        /// <summary>
        /// The WPFMessageBox has been closed by the OK button and the OK button is the default button.
        /// </summary>
        OK = 1,

        /// <summary>
        /// The WPFMessageBox has been closed by the Cancel button and the Cancel button is the default button.
        /// </summary>
        Cancel = 2,

        /// <summary>
        /// The WPFMessageBox has been closed by the Abort button and the Abort button is the default button.
        /// </summary>
        Abort = 3,

        /// <summary>
        /// The WPFMessageBox has been closed by the Retry button and the Retry button is the default button.
        /// </summary>
        Retry = 4,

        /// <summary>
        /// The WPFMessageBox has been closed by the Ignore button and the Ignore button is the default button.
        /// </summary>
        Ignore = 5,

        /// <summary>
        /// The WPFMessageBox has been closed by the Yes button and the Yes button is the default button.
        /// </summary>
        Yes = 6,

        /// <summary>
        /// The WPFMessageBox has been closed by the No button and the No button is the default button.
        /// </summary>
        No = 7,

        /// <summary>
        /// The WPFMessageBox has been closed by the Continue button and the Continue button is the default button.
        /// </summary>
        Continue = 8,

        /// <summary>
        /// The WPFMessageBox has been closed by the YesToAll button and the YesToAll button is the default button.
        /// </summary>
        YesToAll = 9,

        /// <summary>
        /// The WPFMessageBox has been closed by the NoToAll button and the NoToAll button is the default button.
        /// </summary>
        NoToAll = 10
    }

    /// <summary>
    /// Defines which icon to show in the <see cref="DW.WPFToolkit.Controls.WPFMessageBox" />.
    /// </summary>
    public enum WPFMessageBoxImage
    {
        /// <summary>
        /// The WPFMessageBox has no icon.
        /// </summary>
        None = 0,

        /// <summary>
        /// The WPFMessageBox has the Error icon.
        /// </summary>
        Error = 16,

        /// <summary>
        /// The WPFMessageBox has the Hand icon.
        /// </summary>
        Hand = 16,

        /// <summary>
        /// The WPFMessageBox has the Stop icon.
        /// </summary>
        Stop = 16,

        /// <summary>
        /// The WPFMessageBox has the Question icon.
        /// </summary>
        Question = 32,

        /// <summary>
        /// The WPFMessageBox has the Exclamation icon.
        /// </summary>
        Exclamation = 48,

        /// <summary>
        /// The WPFMessageBox has the Warning icon.
        /// </summary>
        Warning = 48,

        /// <summary>
        /// The WPFMessageBox has the Information icon.
        /// </summary>
        Information = 64,

        /// <summary>
        /// The WPFMessageBox has the Asterisk icon.
        /// </summary>
        Asterisk = 64,
    }
}