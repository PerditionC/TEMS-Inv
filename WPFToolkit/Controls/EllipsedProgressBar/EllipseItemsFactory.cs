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
    /// Implements the <see cref="DW.WPFToolkit.Controls.IItemsFactory" /> and provides ellipse items to the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />
    /// </summary>
    public class EllipseItemsFactory : IItemsFactory
    {
        /// <summary>
        /// Gets or sets the color for each item returned by the <see cref="DW.WPFToolkit.Controls.EllipseItemsFactory.GenerateItems(bool)" /> method. The default is Colors.Red.
        /// </summary>
        public Brush Color { get; set; }

        /// <summary>
        /// Gets or sets the amount of items created by the <see cref="DW.WPFToolkit.Controls.EllipseItemsFactory.GenerateItems(bool)" /> method. The default is 10.
        /// </summary>
        [DefaultValue(10)]
        public int ItemsCount { get; set; }

        /// <summary>
        /// Gets or sets the value the opacity has to be thrinked for the items created by the <see cref="DW.WPFToolkit.Controls.EllipseItemsFactory.GenerateItems(bool)" /> method. This is not used if the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" /> is markes as indeterminate. The default is 0.1.
        /// </summary>
        [DefaultValue(0.1)]
        public double OpacityShrinking { get; set; }

        /// <summary>
        /// Gets or sets the size for the items created by the <see cref="DW.WPFToolkit.Controls.EllipseItemsFactory.GenerateItems(bool)" /> method. The default is 8.
        /// </summary>
        [DefaultValue(8)]
        public double Size { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Controls.EllipseItemsFactory" /> class.
        /// </summary>
        public EllipseItemsFactory()
        {
            Color = new SolidColorBrush(Colors.Red);
            ItemsCount = 10;
            OpacityShrinking = 0.1;
            Size = 8;
        }

        /// <summary>
        /// Creates the items to shown in the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />.
        /// </summary>
        /// <param name="forIndeterminate">Defines if the items are placed in an <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" /> where <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar.IsIndeterminate" /> is set to true.</param>
        /// <returns>The created items to shown in the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />.</returns>
        public IEnumerable<UIElement> GenerateItems(bool forIndeterminate)
        {
            var collection = new List<Ellipse>();
            var opacity = 1.0;
            for (int i = 0; i < ItemsCount; i++)
            {
                collection.Add(CreateEllipse(forIndeterminate ? opacity : 1));
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
            var ellipses = (List<Ellipse>)items;

            var step = 1.0m / ellipses.Count;
            var percent = new decimal((value - mininum) / (maximum - mininum));

            var j = 0;
            for (var i = step; i <= percent; i += step, ++j)
                ellipses[j].Visibility = Visibility.Visible;
            for (; j < ellipses.Count; ++j)
                ellipses[j].Visibility = Visibility.Collapsed;
        }

        private Ellipse CreateEllipse(double opacity)
        {
            return new Ellipse { Height = Size, Width = Size, Fill = Color, Opacity = opacity };
        }
    }
}
