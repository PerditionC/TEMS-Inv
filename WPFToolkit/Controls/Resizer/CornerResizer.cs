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
using System.Windows.Controls.Primitives;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Represents a element in the corners of the <see cref="DW.WPFToolkit.Controls.Resizer" /> to hold and drag in a specific direction.
    /// </summary>
    public class CornerResizer : Thumb
    {
        static CornerResizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CornerResizer), new FrameworkPropertyMetadata(typeof(CornerResizer)));
        }

        /// <summary>
        /// Gets or sets the direction where the corner resizer can be moved.
        /// </summary>
        [DefaultValue(CornerResizerDirections.NEtoSW)]
        public CornerResizerDirections Direction
        {
            get { return (CornerResizerDirections)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.CornerResizer.Direction" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(CornerResizerDirections), typeof(CornerResizer), new UIPropertyMetadata(CornerResizerDirections.NEtoSW));

        /// <summary>
        /// Gets or sets the position of the corner resizer inside the <see cref="DW.WPFToolkit.Controls.Resizer" /> control.
        /// </summary>
        [DefaultValue(CornerResizerPositions.NW)]
        public CornerResizerPositions Position
        {
            get { return (CornerResizerPositions)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.CornerResizer.Position" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(CornerResizerPositions), typeof(CornerResizer), new UIPropertyMetadata(CornerResizerPositions.NW));
    }
}
