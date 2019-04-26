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
    /// Represents a single line to drag in a specific direction. This is used in the <see cref="DW.WPFToolkit.Controls.Resizer" />.
    /// </summary>
    public class FrameResizer : Thumb
    {
        static FrameResizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FrameResizer), new FrameworkPropertyMetadata(typeof(FrameResizer)));
        }

        /// <summary>
        /// Gets or sets the direction where the frame resizer can be moved to.
        /// </summary>
        [DefaultValue(FrameResizerDirections.LeftRight)]
        public FrameResizerDirections Direction
        {
            get { return (FrameResizerDirections)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.FrameResizer.Direction" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(FrameResizerDirections), typeof(FrameResizer), new UIPropertyMetadata(FrameResizerDirections.LeftRight));

        /// <summary>
        /// Gets or sets the position of the frame resizer inside the <see cref="DW.WPFToolkit.Controls.Resizer" /> control.
        /// </summary>
        [DefaultValue(FrameResizerPositions.Right)]
        public FrameResizerPositions Position
        {
            get { return (FrameResizerPositions)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DW.WPFToolkit.Controls.FrameResizer.Position" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(FrameResizerPositions), typeof(FrameResizer), new UIPropertyMetadata(FrameResizerPositions.Right));
    }
}
