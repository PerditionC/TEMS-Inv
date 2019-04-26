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

using System.Windows;
using System.Windows.Controls.Primitives;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Represents the button on the side of the <see cref="DW.WPFToolkit.Controls.SplitButton" /> to expand or collapse the child items.
    /// </summary>
    public class SplitToggleButton : ToggleButton
    {
        static SplitToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitToggleButton), new FrameworkPropertyMetadata(typeof(SplitToggleButton)));
        }

        /// <summary>
        /// Gets or sets the outher radius of the button in the template.
        /// </summary>
        public CornerRadius OutherCornerRadius
        {
            get { return (CornerRadius)GetValue(OutherCornerRadiusProperty); }
            set { SetValue(OutherCornerRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SplitToggleButton.OutherCornerRadius" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutherCornerRadiusProperty =
            DependencyProperty.Register("OutherCornerRadius", typeof(CornerRadius), typeof(SplitToggleButton));

        /// <summary>
        /// Gets or sets the inner radius of the buttion in the template.
        /// </summary>
        public CornerRadius InnerCornerRadius
        {
            get { return (CornerRadius)GetValue(InnerCornerRadiusProperty); }
            set { SetValue(InnerCornerRadiusProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SplitToggleButton.InnerCornerRadius" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerCornerRadiusProperty =
            DependencyProperty.Register("InnerCornerRadius", typeof(CornerRadius), typeof(SplitToggleButton));

        /// <summary>
        /// Gets or sets the thickness of the other border in the template.
        /// </summary>
        public Thickness OutherBorderThickness
        {
            get { return (Thickness)GetValue(OutherBorderThicknessProperty); }
            set { SetValue(OutherBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SplitToggleButton.OutherBorderThickness" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutherBorderThicknessProperty =
            DependencyProperty.Register("OutherBorderThickness", typeof(Thickness), typeof(SplitToggleButton));

        /// <summary>
        /// Gets or sets the thickness of the inner border in the template.
        /// </summary>
        public Thickness InnerBorderThickness
        {
            get { return (Thickness)GetValue(InnerBorderThicknessProperty); }
            set { SetValue(InnerBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.SplitToggleButton.InnerBorderThickness" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerBorderThicknessProperty =
            DependencyProperty.Register("InnerBorderThickness", typeof(Thickness), typeof(SplitToggleButton));
    }
}
