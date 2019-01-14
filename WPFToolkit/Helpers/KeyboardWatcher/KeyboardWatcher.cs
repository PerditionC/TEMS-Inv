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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace DW.WPFToolkit.Helpers
{
    /// <summary>
    /// Brings possibilities to listen for keyboard events even the current application has not the focus. Its the so called System Keyboard Hooks.
    /// </summary>
    /// <example>
    /// <code lang="csharp">
    /// <![CDATA[
    /// public class MainViewModel
    /// {
    ///     public MainViewModel()
    ///     {
    ///         var watcher = new KeyboardWatcher();
    ///         watcher.AddCallback(KeyPressed);
    /// 
    ///         watcher.BeginWatch();
    ///     }
    /// 
    ///     private void KeyPressed(KeyStateChangedArgs e)
    ///     {
    ///         if (e.Key == Key.F10 && e.State == KeyState.Pressed)
    ///         {
    ///             // User Pressed the F10 key
    ///         }
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class KeyboardWatcher : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DW.WPFToolkit.Helpers.KeyboardWatcher" /> class.
        /// </summary>
        public KeyboardWatcher()
        {
            _callbacks = new Dictionary<KeyboardWatchToken, KeyboardWatcherCallback>();
            _proc = HookCallback; // Unmanaged callbacks has to be kept alive
            _hookId = IntPtr.Zero;
        }

        private readonly Dictionary<KeyboardWatchToken, KeyboardWatcherCallback> _callbacks;

        /// <summary>
        /// Occurs when the user pressed or released key.
        /// </summary>
        public event EventHandler<KeyStateChangedArgs> KeyStateChanged;

        private void OnKeyStateChanged(Key key, KeyState state)
        {
            var handler = KeyStateChanged;
            if (handler != null)
                handler(this, new KeyStateChangedArgs(key, state));
        }

        /// <summary>
        /// Registers a callback which got called no matter which keys were pressed or released.
        /// </summary>
        /// <param name="callback">The callback to be called.</param>
        /// <returns>A token which represends the current callback to be used for <see cref="DW.WPFToolkit.Helpers.KeyboardWatcher.RemoveCallback" />.</returns>
        public KeyboardWatchToken AddCallback(Action<KeyStateChangedArgs> callback)
        {
            return AddCallback(new Key[0], new[] { KeyState.Pressed, KeyState.Released }, callback);
        }

        /// <summary>
        /// Registers a callback which got called no matter which keys came into the specific state.
        /// </summary>
        /// <param name="state">The key state which has to be watched for.</param>
        /// <param name="callback">The callback to be called.</param>
        /// <returns>A token which represends the current callback to be used for <see cref="DW.WPFToolkit.Helpers.KeyboardWatcher.RemoveCallback" />.</returns>
        public KeyboardWatchToken AddCallback(KeyState state, Action<KeyStateChangedArgs> callback)
        {
            return AddCallback(new Key[0], new[] { state }, callback);
        }

        /// <summary>
        /// Registers a callback which got called no matter which keys came into the specific states.
        /// </summary>
        /// <param name="states">The key states which has to be watched for.</param>
        /// <param name="callback">The callback to be called.</param>
        /// <returns>A token which represends the current callback to be used for <see cref="DW.WPFToolkit.Helpers.KeyboardWatcher.RemoveCallback" />.</returns>
        public KeyboardWatchToken AddCallback(IEnumerable<KeyState> states, Action<KeyStateChangedArgs> callback)
        {
            return AddCallback(new Key[0], states, callback);
        }

        /// <summary>
        /// Registers a callback which got called if the specific key changed its state.
        /// </summary>
        /// <param name="key">The key which has to be watched for.</param>
        /// <param name="callback">The callback to be called.</param>
        /// <returns>A token which represends the current callback to be used for <see cref="DW.WPFToolkit.Helpers.KeyboardWatcher.RemoveCallback" />.</returns>
        public KeyboardWatchToken AddCallback(Key key, Action<KeyStateChangedArgs> callback)
        {
            return AddCallback(new[] { key }, new[] { KeyState.Pressed, KeyState.Released }, callback);
        }

        /// <summary>
        /// Registers a callback which got called if one of the given key changed their state.
        /// </summary>
        /// <param name="keys">The keys which has to be watched for.</param>
        /// <param name="callback">The callback to be called.</param>
        /// <returns>A token which represends the current callback to be used for <see cref="DW.WPFToolkit.Helpers.KeyboardWatcher.RemoveCallback" />.</returns>
        public KeyboardWatchToken AddCallback(IEnumerable<Key> keys, Action<KeyStateChangedArgs> callback)
        {
            return AddCallback(keys, new[] { KeyState.Pressed, KeyState.Released }, callback);
        }

        /// <summary>
        /// Registers a callback which got called if the specific key changed its state to the given one.
        /// </summary>
        /// <param name="key">The key which has to be watched for.</param>
        /// <param name="state">The key state which has to be watched for.</param>
        /// <param name="callback">The callback to be called.</param>
        /// <returns>A token which represends the current callback to be used for <see cref="DW.WPFToolkit.Helpers.KeyboardWatcher.RemoveCallback" />.</returns>
        public KeyboardWatchToken AddCallback(Key key, KeyState state, Action<KeyStateChangedArgs> callback)
        {
            return AddCallback(new[] { key }, new[] { state }, callback);
        }

        /// <summary>
        /// Registers a callback which got called if the specific key changed its state to one of the given states.
        /// </summary>
        /// <param name="key">The key which has to be watched for.</param>
        /// <param name="states">The key states which has to be watched for.</param>
        /// <param name="callback">The callback to be called.</param>
        /// <returns>A token which represends the current callback to be used for <see cref="DW.WPFToolkit.Helpers.KeyboardWatcher.RemoveCallback" />.</returns>
        public KeyboardWatchToken AddCallback(Key key, IEnumerable<KeyState> states, Action<KeyStateChangedArgs> callback)
        {
            return AddCallback(new[] { key }, states, callback);
        }

        /// <summary>
        /// Registers a callback which got called if one of the given keys changed their state to the specific one.
        /// </summary>
        /// <param name="keys">The keys which has to be watched for.</param>
        /// <param name="state">The key state which has to be watched for.</param>
        /// <param name="callback">The callback to be called.</param>
        /// <returns>A token which represends the current callback to be used for <see cref="DW.WPFToolkit.Helpers.KeyboardWatcher.RemoveCallback" />.</returns>
        public KeyboardWatchToken AddCallback(IEnumerable<Key> keys, KeyState state, Action<KeyStateChangedArgs> callback)
        {
            return AddCallback(keys, new[] { state }, callback);
        }

        /// <summary>
        /// Registers a callback which got called if one of the given keys changed their state to one of the given states.
        /// </summary>
        /// <param name="keys">The keys which has to be watched for.</param>
        /// <param name="states">The key states which has to be watched for.</param>
        /// <param name="callback">The callback to be called.</param>
        /// <returns>A token which represends the current callback to be used for <see cref="DW.WPFToolkit.Helpers.KeyboardWatcher.RemoveCallback" />.</returns>
        public KeyboardWatchToken AddCallback(IEnumerable<Key> keys, IEnumerable<KeyState> states, Action<KeyStateChangedArgs> callback)
        {
            var token = new KeyboardWatchToken();
            var callbackItem = new KeyboardWatcherCallback
            {
                Callback = callback,
                Keys = keys,
                KeyStates = states,
                Token = token
            };
            _callbacks[token] = callbackItem;
            return token;
        }

        /// <summary>
        /// Removes a registered callbacks by the token.
        /// </summary>
        /// <param name="token">The token of the registered callback.</param>
        /// <remarks>If the token is not known anymore nothing happen.</remarks>
        public void RemoveCallback(KeyboardWatchToken token)
        {
            if (_callbacks.ContainsKey(token))
                _callbacks.Remove(token);
        }

        /// <summary>
        /// Removes all known callbacks.
        /// </summary>
        public void ClearCallbacks()
        {
            _callbacks.Clear();
        }

        /// <summary>
        /// Begin watching for system wide keyboard events.
        /// </summary>
        public void BeginWatch()
        {
            _hookId = SetHook();
        }

        /// <summary>
        /// Stop watching for system wide keyboard events.
        /// </summary>
        public void StopWatch()
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }

        /// <summary>
        /// Disposes the object. It calls <see cref="DW.WPFToolkit.Helpers.KeyboardWatcher.StopWatch()" /> if not done already.
        /// </summary>
        public void Dispose()
        {
            if (_hookId == IntPtr.Zero)
                return;

            StopWatch();
        }

        private void NotifyCallbacks(IntPtr keyCode, KeyState state)
        {
            var key = KeyInterop.KeyFromVirtualKey(Marshal.ReadInt32(keyCode));

            var callbacks = _callbacks.Values.Where(c => (c.Keys.Contains(key) || !c.Keys.Any()) && c.KeyStates.Contains(state)).ToList();
            foreach (var callback in callbacks)
                callback.Callback(new KeyStateChangedArgs(key, state));

            OnKeyStateChanged(key, state);
        }

        private static KeyboardProc _proc;
        private delegate IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private IntPtr _hookId;

        private IntPtr SetHook()
        {
            using (var process = Process.GetCurrentProcess())
            using (var module = process.MainModule)
                return SetWindowsHookEx((int)HookType.WH_KEYBOARD_LL, _proc, GetModuleHandle(module.ModuleName), 0);
        }

        private IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                return CallNextHookEx(_hookId, code, wParam, lParam);

            if (wParam == (IntPtr)KeyboardMessages.WM_KEYDOWN || wParam == (IntPtr)KeyboardMessages.WM_SYSKEYDOWN)
                NotifyCallbacks(lParam, KeyState.Pressed);
            else if (wParam == (IntPtr)KeyboardMessages.WM_KEYUP || wParam == (IntPtr)KeyboardMessages.WM_SYSKEYUP)
                NotifyCallbacks(lParam, KeyState.Released);

            return CallNextHookEx(_hookId, code, wParam, lParam);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int hookId, KeyboardProc callbackFunction, IntPtr moduleHandle, uint threadId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hookId);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hookId, int code, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string moduleName);
    }
}
