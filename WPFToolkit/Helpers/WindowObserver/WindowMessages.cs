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

namespace DW.WPFToolkit.Helpers
{
    /// <summary>
    /// Contains a lot of available WinAPI messages. 
    /// See http://msdn.microsoft.com/en-us/library/ff468922(v=VS.85).aspx 
    /// or http://msdn.microsoft.com/en-us/library/windows/desktop/ms644927(v=vs.85).aspx
    /// or http://www.autohotkey.com/docs/misc/SendMessageList.htm
    /// </summary>
    public struct WindowMessages
    {
        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NULL = 0x00;
        
        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CREATE = 0x01;
        
        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DESTROY = 0x02;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MOVE = 0x03;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SIZE = 0x05;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_ACTIVATE = 0x06;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SETFOCUS = 0x07;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_KILLFOCUS = 0x08;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_ENABLE = 0x0A;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SETREDRAW = 0x0B;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SETTEXT = 0x0C;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_GETTEXT = 0x0D;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_GETTEXTLENGTH = 0x0E;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_PAINT = 0x0F;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CLOSE = 0x10;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_QUERYENDSESSION = 0x11;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_QUIT = 0x12;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_QUERYOPEN = 0x13;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_ERASEBKGND = 0x14;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SYSCOLORCHANGE = 0x15;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_ENDSESSION = 0x16;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SYSTEMERROR = 0x17;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SHOWWINDOW = 0x18;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CTLCOLOR = 0x19;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_WININICHANGE = 0x1A;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SETTINGCHANGE = 0x1A;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DEVMODECHANGE = 0x1B;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_ACTIVATEAPP = 0x1C;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_FONTCHANGE = 0x1D;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_TIMECHANGE = 0x1E;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CANCELMODE = 0x1F;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SETCURSOR = 0x20;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MOUSEACTIVATE = 0x21;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CHILDACTIVATE = 0x22;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_QUEUESYNC = 0x23;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_GETMINMAXINFO = 0x24;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_PAINTICON = 0x26;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_ICONERASEBKGND = 0x27;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NEXTDLGCTL = 0x28;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SPOOLERSTATUS = 0x2A;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DRAWITEM = 0x2B;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MEASUREITEM = 0x2C;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DELETEITEM = 0x2D;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_VKEYTOITEM = 0x2E;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CHARTOITEM = 0x2F;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SETFONT = 0x30;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_GETFONT = 0x31;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SETHOTKEY = 0x32;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_GETHOTKEY = 0x33;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_QUERYDRAGICON = 0x37;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_COMPAREITEM = 0x39;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_COMPACTING = 0x41;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_WINDOWPOSCHANGING = 0x46;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_WINDOWPOSCHANGED = 0x47;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_POWER = 0x48;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_COPYDATA = 0x4A;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CANCELJOURNAL = 0x4B;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NOTIFY = 0x4E;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_INPUTLANGCHANGEREQUEST = 0x50;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_INPUTLANGCHANGE = 0x51;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_TCARD = 0x52;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_HELP = 0x53;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_USERCHANGED = 0x54;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NOTIFYFORMAT = 0x55;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CONTEXTMENU = 0x7B;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_STYLECHANGING = 0x7C;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_STYLECHANGED = 0x7D;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DISPLAYCHANGE = 0x7E;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_GETICON = 0x7F;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SETICON = 0x80;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCCREATE = 0x81;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCDESTROY = 0x82;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCCALCSIZE = 0x83;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCHITTEST = 0x84;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCPAINT = 0x85;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCACTIVATE = 0x86;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_GETDLGCODE = 0x87;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCMOUSEMOVE = 0xA0;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCLBUTTONDOWN = 0xA1;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCLBUTTONUP = 0xA2;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCLBUTTONDBLCLK = 0xA3;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCRBUTTONDOWN = 0xA4;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCRBUTTONUP = 0xA5;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCRBUTTONDBLCLK = 0xA6;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCMBUTTONDOWN = 0xA7;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCMBUTTONUP = 0xA8;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCMBUTTONDBLCLK = 0xA9;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_KEYFIRST = 0x100;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_KEYDOWN = 0x100;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_KEYUP = 0x101;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CHAR = 0x102;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DEADCHAR = 0x103;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SYSKEYDOWN = 0x104;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SYSKEYUP = 0x105;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SYSCHAR = 0x106;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SYSDEADCHAR = 0x107;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_KEYLAST = 0x108;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_IME_STARTCOMPOSITION = 0x10D;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_IME_ENDCOMPOSITION = 0x10E;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_IME_COMPOSITION = 0x10F;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_IME_KEYLAST = 0x10F;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_INITDIALOG = 0x110;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_COMMAND = 0x111;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SYSCOMMAND = 0x112;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_TIMER = 0x113;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_HSCROLL = 0x114;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_VSCROLL = 0x115;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_INITMENU = 0x116;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_INITMENUPOPUP = 0x117;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MENUSELECT = 0x11F;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MENUCHAR = 0x120;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_ENTERIDLE = 0x121;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CTLCOLORMSGBOX = 0x132;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CTLCOLOREDIT = 0x133;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CTLCOLORLISTBOX = 0x134;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CTLCOLORBTN = 0x135;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CTLCOLORDLG = 0x136;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CTLCOLORSCROLLBAR = 0x137;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CTLCOLORSTATIC = 0x138;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MOUSEFIRST = 0x200;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MOUSEMOVE = 0x200;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_LBUTTONDOWN = 0x201;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_LBUTTONUP = 0x202;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_LBUTTONDBLCLK = 0x203;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_RBUTTONDOWN = 0x204;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_RBUTTONUP = 0x205;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_RBUTTONDBLCLK = 0x206;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MBUTTONDOWN = 0x207;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MBUTTONUP = 0x208;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MBUTTONDBLCLK = 0x209;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MOUSEWHEEL = 0x20A;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MOUSEHWHEEL = 0x20E;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_PARENTNOTIFY = 0x210;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_ENTERMENULOOP = 0x211;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_EXITMENULOOP = 0x212;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NEXTMENU = 0x213;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SIZING = 0x214;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CAPTURECHANGED = 0x215;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MOVING = 0x216;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_POWERBROADCAST = 0x218;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DEVICECHANGE = 0x219;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MDICREATE = 0x220;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MDIDESTROY = 0x221;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MDIACTIVATE = 0x222;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MDIRESTORE = 0x223;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MDINEXT = 0x224;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MDIMAXIMIZE = 0x225;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MDITILE = 0x226;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MDICASCADE = 0x227;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MDIICONARRANGE = 0x228;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MDIGETACTIVE = 0x229;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MDISETMENU = 0x230;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_ENTERSIZEMOVE = 0x231;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_EXITSIZEMOVE = 0x232;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DROPFILES = 0x233;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MDIREFRESHMENU = 0x234;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_IME_SETCONTEXT = 0x281;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_IME_NOTIFY = 0x282;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_IME_CONTROL = 0x283;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_IME_COMPOSITIONFULL = 0x284;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_IME_SELECT = 0x285;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_IME_CHAR = 0x286;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_IME_KEYDOWN = 0x290;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_IME_KEYUP = 0x291;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MOUSEHOVER = 0x2A1;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_NCMOUSELEAVE = 0x2A2;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_MOUSELEAVE = 0x2A3;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CUT = 0x300;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_COPY = 0x301;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_PASTE = 0x302;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CLEAR = 0x303;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_UNDO = 0x304;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_RENDERFORMAT = 0x305;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_RENDERALLFORMATS = 0x306;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DESTROYCLIPBOARD = 0x307;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DRAWCLIPBOARD = 0x308;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_PAINTCLIPBOARD = 0x309;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_VSCROLLCLIPBOARD = 0x30A;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_SIZECLIPBOARD = 0x30B;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_ASKCBFORMATNAME = 0x30C;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_CHANGECBCHAIN = 0x30D;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_HSCROLLCLIPBOARD = 0x30E;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_QUERYNEWPALETTE = 0x30F;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_PALETTEISCHANGING = 0x310;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_PALETTECHANGED = 0x311;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_HOTKEY = 0x312;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_PRINT = 0x317;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_PRINTCLIENT = 0x318;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_HANDHELDFIRST = 0x358;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_HANDHELDLAST = 0x35F;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_PENWINFIRST = 0x380;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_PENWINLAST = 0x38F;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_COALESCE_FIRST = 0x390;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_COALESCE_LAST = 0x39F;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DDE_FIRST = 0x3E0;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DDE_INITIATE = 0x3E0;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DDE_TERMINATE = 0x3E1;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DDE_ADVISE = 0x3E2;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DDE_UNADVISE = 0x3E3;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DDE_ACK = 0x3E4;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DDE_DATA = 0x3E5;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DDE_REQUEST = 0x3E6;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DDE_POKE = 0x3E7;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DDE_EXECUTE = 0x3E8;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_DDE_LAST = 0x3E8;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_USER = 0x400;

        /// <summary>
        /// See <see cref="DW.WPFToolkit.Helpers.WindowMessages" />.
        /// </summary>
        public static readonly int WM_APP = 0x8000;
    }
}
