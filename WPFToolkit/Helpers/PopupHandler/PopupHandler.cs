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

namespace DW.WPFToolkit.Helpers
{
    /// <summary>
    /// This objects help to determine when a popup has to be closed. This can be by clicking somewhere else, clicking in the title bar or moving the window.
    /// </summary>
    /// <example>
    /// <code lang="csharp">
    /// <![CDATA[
    /// public class Control : ContentControl
    /// {
    ///     private PopupHandler _popupHandler;
    /// 
    ///     public override void OnApplyTemplate()
    ///     {
    ///         var popup = GetTemplateChild("PART_Popup") as Popup;
    ///         if (popup == null)
    ///             return;
    /// 
    ///         _popupHandler = new PopupHandler();
    ///         _popupHandler.AutoClose(popup, OnPopupClosed);
    ///     }
    /// 
    ///     private void OnPopupClosed()
    ///     {
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class PopupHandler
    {
        private WindowObserver _observer;
        private UIElement _observedControl;
        private Action _closeMethod;

        /// <summary>
        /// Starts an observing of the window which contains the control to determine when the item has to be closed.
        /// </summary>
        /// <param name="observedControl">The control which owner window has to be observed.</param>
        /// <param name="closeMethod">The close callback. This gets invoked when the owner window has send notifications to close the popup.</param>
        /// <exception cref="System.ArgumentNullException">observedControl is null.</exception>
        /// <exception cref="System.ArgumentNullException">closeMethod is null.</exception>
        public void AutoClose(UIElement observedControl, Action closeMethod)
        {
            if (observedControl == null)
                throw new ArgumentNullException("observedControl");
            if (closeMethod == null)
                throw new ArgumentNullException("closeMethod");

            _observedControl = observedControl;
            _closeMethod = closeMethod;
            var ownerWindow = Window.GetWindow(observedControl);
            if (ownerWindow != null)
            {
                _observer = new WindowObserver(ownerWindow);
                _observer.AddCallbackFor(WindowMessages.WM_NCLBUTTONDOWN, p => CallMethod());
                _observer.AddCallbackFor(WindowMessages.WM_LBUTTONDOWN, p => CallMethod());
                _observer.AddCallbackFor(WindowMessages.WM_KILLFOCUS, p => CallMethod());
            }
        }

        private void CallMethod()
        {
            if (!_observedControl.IsMouseOver)
                _closeMethod();
        }
    }
}
