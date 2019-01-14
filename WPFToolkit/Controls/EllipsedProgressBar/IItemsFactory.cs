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
using System.Windows;

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// The object implementing this interface is used to create and updtae the items in the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" /> depending on the progress value and progress bar state.
    /// </summary>
    public interface IItemsFactory
    {
        /// <summary>
        /// This method returns the items placed into the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />.
        /// </summary>
        /// <param name="forIndeterminate">Defines if the using <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" /> is indeterminate or not.</param>
        /// <returns>The elements to be placed into the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />. These object will be passed to the <see cref="DW.WPFToolkit.Controls.IItemsFactory.EditItemsForValue(IEnumerable{UIElement}, double, double, double)" /> as soon any progress value changes.</returns>
        IEnumerable<UIElement> GenerateItems(bool forIndeterminate);

        /// <summary>
        /// Updates all items which are placed in the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" /> by the given progress value.
        /// </summary>
        /// <param name="items">The items placed in the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" /> to be updated.</param>
        /// <param name="mininum">The configured minimum value of the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />.</param>
        /// <param name="maximum">The configured maximum value of the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />.</param>
        /// <param name="value">The current progress value of the <see cref="DW.WPFToolkit.Controls.EllipsedProgressBar" />.</param>
        void EditItemsForValue(IEnumerable<UIElement> items, double mininum, double maximum, double value);
    }
}
