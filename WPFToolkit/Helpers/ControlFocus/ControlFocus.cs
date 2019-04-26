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
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace DW.WPFToolkit.Helpers
{
    /// <summary>
    /// This object gives you the quick and easy possibility to move the current focus to a specific element.
    /// </summary>
    /// <example>
    /// <code lang="csharp">
    /// <![CDATA[
    /// ControlFocus.GiveFocus(myButton);
    /// ]]>
    /// </code>
    /// </example>
    public static class ControlFocus
    {
        /// <summary>
        /// Gives the focus to the given UIElement.
        /// </summary>
        /// <param name="element">The UIElement which has to get the focus.</param>
        /// <remarks>Giving the focus will be done using the target element dispatcher with the <see cref="System.Windows.Threading.DispatcherPriority.Render" /> priority.</remarks>
        public static void GiveFocus(UIElement element)
        {
            element.Dispatcher.BeginInvoke(new Action(delegate
            {
                element.Focus();
                Keyboard.Focus(element);
            }),
            DispatcherPriority.Render);
        }

        /// <summary>
        /// Gives the focus to the given UIElement with a Callback.
        /// </summary>
        /// <param name="element">The UIElement which has to get the focus.</param>
        /// <param name="actionOnFocus">The callback which will be called when the control got the focus. It will called just before the element.Focus will called and the KeyboardFocus will be set.</param>
        /// <remarks>Giving the focus will be done using the target element dispatcher with the <see cref="System.Windows.Threading.DispatcherPriority.Render" /> priority.</remarks>
        public static void GiveFocus(UIElement element, Action actionOnFocus)
        {
            element.Dispatcher.BeginInvoke(new Action(() =>
                                                        {
                                                            actionOnFocus();
                                                            element.Focus();
                                                            Keyboard.Focus(element);
                                                        }),
            DispatcherPriority.Render);
        }
    }
}
