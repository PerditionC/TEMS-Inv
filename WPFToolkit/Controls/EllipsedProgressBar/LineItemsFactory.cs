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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Implements the <see cref="DW.WPFToolkit.Controls.IItemsFactory" /> and provides line items to the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />
    /// </summary>
    public class LineItemsFactory : IItemsFactory
    {
        /// <summary>
        /// Gets or sets the line caps for the items created by the <see cref="DW.WPFToolkit.Controls.LineItemsFactory.GenerateItems(bool)" /> method. The default is <see cref="System.Windows.Media.PenLineCap.Round" />.
        /// </summary>
        [DefaultValue(PenLineCap.Round)]
        public PenLineCap Caps { get; set; }

        /// <summary>
        /// Gets or sets the line color for the items created by the <see cref="DW.WPFToolkit.Controls.LineItemsFactory.GenerateItems(bool)" /> method. The default is Colors.Blue.
        /// </summary>
        public Brush Color { get; set; }

        /// <summary>
        /// Gets or sets the amount of lines created by the <see cref="DW.WPFToolkit.Controls.LineItemsFactory.GenerateItems(bool)" /> method. The default is 10.
        /// </summary>
        [DefaultValue(10)]
        public int ItemsCount { get; set; }

        /// <summary>
        /// Gets or sets the length of the lines created by the <see cref="DW.WPFToolkit.Controls.LineItemsFactory.GenerateItems(bool)" /> method. The default is 10.
        /// </summary>
        [DefaultValue(10)]
        public double Length { get; set; }

        /// <summary>
        /// Gets or sets the value the opacity has to be thrinked for the items created by the <see cref="DW.WPFToolkit.Controls.LineItemsFactory.GenerateItems(bool)" /> method. This is not used if the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" /> is markes as indeterminate. The default is 0.1.
        /// </summary>
        [DefaultValue(0.1)]
        public double OpacityShrinking { get; set; }

        /// <summary>
        /// Gets or sets the thickness of the lines created by the <see cref="DW.WPFToolkit.Controls.LineItemsFactory.GenerateItems(bool)" /> method. The default is 4.
        /// </summary>
        public double Thickness { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.LineItemsFactory" /> class.
        /// </summary>
        public LineItemsFactory()
        {
            Caps = PenLineCap.Round;
            Color = new SolidColorBrush(Colors.Blue);
            ItemsCount = 10;
            Length = 10;
            OpacityShrinking = 0.1;
            Thickness = 4;
        }

        /// <summary>
        /// Creates the items to shown in the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />.
        /// </summary>
        /// <param name="forIndeterminate">Defines if the items are placed in an <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" /> where <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.IsIndeterminate" /> is set to true.</param>
        /// <returns>The created items to shown in the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />.</returns>
        public IEnumerable<UIElement> GenerateItems(bool forIndeterminate)
        {
            var collection = new List<Line>();
            var opacity = 1.0;
            for (int i = 0; i < ItemsCount; i++)
            {
                collection.Add(CreateLine(forIndeterminate ? opacity : 1));
                opacity -= OpacityShrinking;
            }
            collection.Reverse();
            return collection;
        }

        /// <summary>
        /// Modifies the items depending on the current progress state called by the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />.
        /// </summary>
        /// <param name="items">The items created by the <see cref="DW.WPFToolkit.Controls.EllipseItemsFactory.GenerateItems(bool)" />.</param>
        /// <param name="mininum">The minimum value defined in the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />.</param>
        /// <param name="maximum">The maximum value defined in the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" /></param>
        /// <param name="value">The current progress value in the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />.</param>
        public void EditItemsForValue(IEnumerable<UIElement> items, double mininum, double maximum, double value)
        {
            var lines = (List<Line>)items;

            var step = 1.0m / lines.Count;
            var percent = new decimal((value - mininum) / (maximum - mininum));

            var j = 0;
            for (var i = step; i <= percent; i += step, ++j)
                lines[j].Visibility = Visibility.Visible;
            for (; j < lines.Count; ++j)
                lines[j].Visibility = Visibility.Collapsed;
        }

        private Line CreateLine(double opacity)
        {
            return new Line
            {
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = Length,
                Opacity = opacity,
                StrokeThickness = Thickness,
                Stroke = Color,
                StrokeEndLineCap = Caps,
                StrokeStartLineCap = Caps
            };
        }
    }
}
