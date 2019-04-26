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
using System.Runtime.InteropServices;
using System.Text;

namespace DW.WPFToolkit.Controls
{
    internal static class SystemTexts
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int LoadString(IntPtr hInstance, uint stringId, StringBuilder lpBuffer, int nBufferMax);
        
        [DllImport("kernel32")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        internal const uint OK_CAPTION = 800;
        internal const uint CANCEL_CAPTION = 801;
        internal const uint ABORT_CAPTION = 802;
        internal const uint RETRY_CAPTION = 803;
        internal const uint IGNORE_CAPTION = 804;
        internal const uint YES_CAPTION = 805;
        internal const uint NO_CAPTION = 806;
        internal const uint CLOSE_CAPTION = 807;
        internal const uint HELP_CAPTION = 808;
        internal const uint TRYAGAIN_CAPTION = 809;
        internal const uint CONTINUE_CAPTION = 810;

        internal static string GetString(uint id)
        {
            var sb = new StringBuilder(256);
            var user32 = LoadLibrary(Environment.SystemDirectory + "\\User32.dll");
            LoadString(user32, id, sb, sb.Capacity);
            return sb.ToString();
        }
    }
}
