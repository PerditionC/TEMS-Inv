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

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Represents the details button shown in the <see cref="DW.WPFToolkit.Controls.WPFMessageBox" />.
    /// </summary>
    public class WPFMessageBoxDetailsButtonControl : Expander
    {
        static WPFMessageBoxDetailsButtonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPFMessageBoxDetailsButtonControl), new FrameworkPropertyMetadata(typeof(WPFMessageBoxDetailsButtonControl)));
        }

        /// <summary>
        /// Gets or sets the text to show on the button if the details are not shown.
        /// </summary>
        [DefaultValue(null)]
        public string CollapsedHeaderText
        {
            get { return (string)GetValue(CollapsedHeaderTextProperty); }
            set { SetValue(CollapsedHeaderTextProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.WPFMessageBoxDetailsButtonControl.CollapsedHeaderText" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty CollapsedHeaderTextProperty =
            DependencyProperty.Register("CollapsedHeaderText", typeof(string), typeof(WPFMessageBoxDetailsButtonControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the text to show on the button if the details are shown.
        /// </summary>
        [DefaultValue(null)]
        public string ExpandedHeaderText
        {
            get { return (string)GetValue(ExpandedHeaderTextProperty); }
            set { SetValue(ExpandedHeaderTextProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.WPFMessageBoxDetailsButtonControl.ExpandedHeaderText" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandedHeaderTextProperty =
            DependencyProperty.Register("ExpandedHeaderText", typeof(string), typeof(WPFMessageBoxDetailsButtonControl), new UIPropertyMetadata(null));
    }
}
