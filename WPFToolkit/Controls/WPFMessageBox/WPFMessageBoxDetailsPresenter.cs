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
    /// Holds the details content shown if details in the <see cref="DW.WPFToolkit.Controls.WPFMessageBox"/> are expanded.
    /// </summary>
    public class WPFMessageBoxDetailsPresenter : ContentControl
    {
        static WPFMessageBoxDetailsPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WPFMessageBoxDetailsPresenter), new FrameworkPropertyMetadata(typeof(WPFMessageBoxDetailsPresenter)));
        }

        /// <summary>
        /// Gets or sets a value which indicates if the details are visible or not.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDetailsExpanded
        {
            get { return (bool)GetValue(IsDetailsExpandedProperty); }
            set { SetValue(IsDetailsExpandedProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.WPFMessageBoxDetailsPresenter.IsDetailsExpanded" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDetailsExpandedProperty =
            DependencyProperty.Register("IsDetailsExpanded", typeof(bool), typeof(WPFMessageBoxDetailsPresenter), new UIPropertyMetadata(false));
    }
}
