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
using System.Windows;

namespace DW.WPFToolkit.Helpers
{
    /// <summary>
    /// Holds the data passed when a specific WinAPI message has appear. This is used in the <see cref="DW.WPFToolkit.Helpers.WindowObserver" />.
    /// </summary>
    public sealed class NotifyEventArgs : EventArgs
    {
        internal NotifyEventArgs(Window observedWindow, int messageId)
        {
            ObservedWindow = observedWindow;
            MessageId = messageId;
        }

        /// <summary>
        /// Gets the window which has raised the specific WinAPI message.
        /// </summary>
        public Window ObservedWindow { get; private set; }

        /// <summary>
        /// Gets the appeared WinAPI message. See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public int MessageId { get; private set; }
    }
}
