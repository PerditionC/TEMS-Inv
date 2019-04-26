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
using System.Windows;
using System.Windows.Interop;

namespace DW.WPFToolkit.Interactivity
{
    internal static class WindowTitleBar
    {
        internal static void RemoveTitleItems(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var windowLong = GetWindowLong(hwnd, GWL_STYLE);
            windowLong &= ~WS_SYSMENU;
            SetWindowLong(hwnd, GWL_STYLE, windowLong);
        }

        internal static void DisableSystemMenu(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var windowLong = GetWindowLong(hwnd, GWL_EXSTYLE);
            windowLong |= WS_EX_DLGMODALFRAME;
            uint windowFlags = SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED;
            SetWindowLong(hwnd, GWL_EXSTYLE, windowLong);
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, windowFlags);
        }

        internal static void DisableMinimizeButton(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var windowLong = GetWindowLong(hwnd, GWL_STYLE);
            windowLong &= ~WS_MINIMIZEBOX;
            SetWindowLong(hwnd, GWL_STYLE, windowLong);
        }

        internal static void DisableMaximizeButton(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var windowLong = GetWindowLong(hwnd, GWL_STYLE);
            windowLong &= ~WS_MAXIMIZEBOX;
            SetWindowLong(hwnd, GWL_STYLE, windowLong);
        }

        internal static void DisableCloseButton(Window window)
        {
            var hwndSource = PresentationSource.FromVisual(window) as HwndSource;
            if (hwndSource != null)
                hwndSource.AddHook(DisableCloseButtonHook);
        }

        private static IntPtr DisableCloseButtonHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SHOWWINDOW)
            {
                var hMenu = GetSystemMenu(hwnd, false);
                if (hMenu != IntPtr.Zero)
                    EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            }
            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr windowHandle, bool revert);

        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr menuHandle, uint itemId, uint enable);

        private const int GWL_EXSTYLE = -20;
        private const int GWL_STYLE = -16;

        private const int SC_CLOSE = 0xF060;

        private const int MF_BYCOMMAND = 0x00000000;
        private const int MF_GRAYED = 0x00000001;

        private const int WS_EX_DLGMODALFRAME = 0x0001;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;
        private const int WS_SYSMENU = 0x80000;

        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;

        private const int WM_SHOWWINDOW = 0x00000018;
    }
}
