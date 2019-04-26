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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;

namespace DW.WPFToolkit.Helpers
{
    /// <summary>
    /// Brings possibilities to easy listen for WinAPI events.
    /// </summary>
    /// <example>
    /// <code lang="csharp">
    /// <![CDATA[
    /// public partial class MainView
    /// {
    ///     public MainView()
    ///     {
    ///         InitializeComponent();
    /// 
    ///         var observer = new WindowObserver(this);
    ///         observer.AddCallback(OnEventHappened);
    ///     }
    /// 
    ///     private void OnEventHappened(NotifyEventArgs e)
    ///     {
    ///         if (e.MessageId == WindowMessages.WM_NCLBUTTONDBLCLK)
    ///         {
    ///             // User double clicked in the non client area (title bar mostly)
    ///         }
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class WindowObserver
    {
        private readonly Window _observedWindow;
        private readonly List<Callback> _callbacks;

        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Helpers.WindowObserver" /> class.
        /// </summary>
        /// <param name="observedWindow">The window which WinAPI messages should be observed.</param>
        /// <exception cref="System.ArgumentNullException">observedWindow is null.</exception>
        public WindowObserver(Window observedWindow)
        {
            if (observedWindow == null)
                throw new ArgumentNullException("observedWindow");

            _callbacks = new List<Callback>();

            _observedWindow = observedWindow;
            if (!observedWindow.IsLoaded)
                observedWindow.Loaded += WindowLoaded;
            else
                HookIn();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ((Window)sender).Loaded -= WindowLoaded;
            
            HookIn();
        }

        private void HookIn()
        {
            var handle = new WindowInteropHelper(_observedWindow).Handle;
            HwndSource.FromHwnd(handle).AddHook(WindowProc);
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            NotifyMessage(msg);
            NotifyCallbacks(msg);

            return (IntPtr)0;
        }

        /// <summary>
        /// Occurs when the observed window has send the a WinAPI message.
        /// </summary>
        public event EventHandler<NotifyEventArgs> Message;

        private void NotifyMessage(int msg)
        {
            var handler = Message;
            if (handler != null)
                handler(this, new NotifyEventArgs(_observedWindow, msg));
        }

        /// <summary>
        /// Registers a calback to be invoked when a WinAPI message appears in the observed window.
        /// </summary>
        /// <param name="callback">The callback to be invoked when a WinAPI message appears in the observed window.</param>
        /// <remarks>The callback is not registered as a WeakReference, consider using <see cref="DW.WPFToolkit.Helpers.WindowObserver.RemoveCallback(Action{NotifyEventArgs})" /> to remove a callback if its not needed anymore.</remarks>
        /// <exception cref="System.ArgumentNullException">callback is null.</exception>
        public void AddCallback(Action<NotifyEventArgs> callback)
        {
            AddCallbackFor(null, callback);
        }

        /// <summary>
        /// Registers a calback to be invoked when the specific WinAPI message appears in the observed window.
        /// </summary>
        /// <param name="messageId">The WinAPI message to listen for. If its null all WinAPI messages will be forwarded to the callback.</param>
        /// <param name="callback">The callback to be invoked when the specific WinAPI message appears in the observed window.</param>
        /// <remarks>The callback is not registered as a WeakReference, consider using <see cref="DW.WPFToolkit.Helpers.WindowObserver.RemoveCallback(Action{NotifyEventArgs})" /> to remove a callback if its not needed anymore.</remarks>
        /// <exception cref="System.ArgumentNullException">callback is null.</exception>
        public void AddCallbackFor(int? messageId, Action<NotifyEventArgs> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            _callbacks.Add(new Callback(messageId, callback));
        }

        private void NotifyCallbacks(int message)
        {
            for (var i = 0; i < _callbacks.Count; i++)
            {
                if (_callbacks[i].ListenMessageId == null ||
                     _callbacks[i].ListenMessageId == message)
                    _callbacks[i].Action(new NotifyEventArgs(_observedWindow, message));
            }
        }

        /// <summary>
        /// Removed the previous registered callback.
        /// </summary>
        /// <param name="callback">The previous registered callback to remove. If it is remoed already nothing happens.</param>
        /// <exception cref="System.ArgumentNullException">callback is null.</exception>
        public void RemoveCallback(Action<NotifyEventArgs> callback)
        {
            if (callback == null)
                throw new ArgumentNullException("callback");

            _callbacks.RemoveAll(c => c.Action == callback);
        }

        /// <summary>
        /// Removes all registered callbacks.
        /// </summary>
        public void ClearCallbacks()
        {
            _callbacks.Clear();
        }

        /// <summary>
        /// Removes all callbacks which listen for a specific WinAPI message.
        /// </summary>
        /// <param name="messageId">The WinAPI message the callbacks does listen for.</param>
        public void RemoveCallbacksFor(int messageId)
        {
            _callbacks.RemoveAll(c => c.ListenMessageId == messageId);
        }
    }
}
