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
using System.Security;
using System.Windows;
using DW.WPFToolkit.Controls;

namespace DW.WPFToolkit.Services
{
    /// <summary>
    /// Wraps up the static MessageBox object to have it testable. See <see cref="System.Windows.MessageBox" />.
    /// </summary>
    /// <example>
    /// <code lang="csharp">
    /// <![CDATA[
    /// private readonly IMessageBoxService _messageBoxService;
    /// 
    /// public MainViewModel(IMessageBoxService messageBoxService)
    /// {
    ///     _messageBoxService = messageBoxService;
    /// }
    /// 
    /// public void Show1()
    /// {
    ///     _messageBoxService.Show("Show the messagebox without an owner");
    /// }
    /// 
    /// public void Show2()
    /// {
    ///     _messageBoxService.Show("Show the messagebox without an owner", "But with a caption");
    /// }
    /// 
    /// public void Show3()
    /// {
    ///     _messageBoxService.Show(ViewKey.MainView, "Show the messagebox with an owner", "And with a caption", MessageBoxButton.OK, MessageBoxImage.Information);
    /// }
    /// ]]>
    /// </code>
    /// 
    /// <code lang="csharp">
    /// <![CDATA[
    /// public partial class App
    /// {
    ///     private readonly IWindowService _windowService;
    ///     private readonly IMessageBoxService _messageBoxService;
    /// 
    ///     public App()
    ///     {
    ///         _windowService = new WindowService();
    ///         _messageBoxService = new MessageBoxService();
    /// 
    ///         RegisterServices();
    ///     }
    /// 
    ///     private void RegisterServices()
    ///     {
    ///         _windowService.Register<MainView>(ViewKey.MainView);
    ///         _windowService.WindowCreated += HandleWindowCreated;
    ///         _windowService.WindowClosed += HandleWindowClosed;
    ///     }
    /// 
    ///     private void HandleWindowCreated(object sender, WindowStateEventArgs e)
    ///     {
    ///         _messageBoxService.RegisterOwner(e.Window, e.WindowKey);
    ///     }
    /// 
    ///     private void HandleWindowClosed(object sender, WindowStateEventArgs e)
    ///     {
    ///         _messageBoxService.UnregisterOwner(e.WindowKey);
    ///     }
    /// 
    ///     protected override void OnStartup(StartupEventArgs e)
    ///     {
    ///         _windowService.ShowDialog(ViewKey.MainView, new MainViewModel());
    ///     }
    /// }
    /// 
    /// public enum ViewKey
    /// {
    ///     MainView
    /// }
    /// ]]>
    /// </code>
    /// </example>
    public class WPFMessageBoxService : IWPFMessageBoxService
    {
        private readonly Dictionary<object, Window> _owner;

        /// <summary>
        /// Initializes a new instance of the DW.WPFToolkit.Services.WPFMessageBoxService class.
        /// </summary>
        public WPFMessageBoxService()
        {
            _owner = new Dictionary<object, Window>();
        }

        /// <summary>
        /// Registers an owner window by a key to be used in the show methods.
        /// </summary>
        /// <param name="owner">The window to register.</param>
        /// <param name="key">The corresponding window key.</param>
        /// <remarks>If the key is known already the old will be overwritten. The window reference will not removed automatically, consider call the <see cref="UnregisterOwner(object)" /> for removing an old window.</remarks>
        public void RegisterOwner(Window owner, object key)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            if (key == null)
                throw new ArgumentNullException("key");

            _owner[key] = owner;
        }

        /// <summary>
        /// Removes a registered owner window by its key.
        /// </summary>
        /// <param name="key">The corresponding window key.</param>
        public void UnregisterOwner(object key)
        {
            if (key != null && _owner.ContainsKey(key))
                _owner.Remove(key);
        }

        /// <summary>
        /// Displays a message box that has a message and that returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <returns>A WPFMessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        [SecurityCritical]
        public WPFMessageBoxResult Show(string messageBoxText)
        {
            return Show(null, messageBoxText, string.Empty, WPFMessageBoxButtons.OK, WPFMessageBoxImage.None, WPFMessageBoxResult.None, new WPFMessageBoxOptions());
        }

        /// <summary>
        /// Displays a message box that has a message and title bar caption; and that returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <returns>A WPFMessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        [SecurityCritical]
        public WPFMessageBoxResult Show(string messageBoxText, string caption)
        {
            return Show(null, messageBoxText, caption, WPFMessageBoxButtons.OK, WPFMessageBoxImage.None, WPFMessageBoxResult.None, new WPFMessageBoxOptions());
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays a message and returns a result.
        /// </summary>
        /// <param name="ownerKey">The key of the registered window by the <see cref="RegisterOwner(Window, object)" /> that represents the owner window of the message box. If the key is null or no window is known with the key, the box will be called without an owner.</param>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <returns>A WPFMessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        [SecurityCritical]
        public WPFMessageBoxResult Show(object ownerKey, string messageBoxText)
        {
            return Show(ownerKey, messageBoxText, string.Empty, WPFMessageBoxButtons.OK, WPFMessageBoxImage.None, WPFMessageBoxResult.None, new WPFMessageBoxOptions());
        }

        /// <summary>
        /// Displays a message box that has a message, title bar caption, and button; and that returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A WPFMessageBoxButtons value that specifies which button or buttons to display.</param>
        /// <returns>A WPFMessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        [SecurityCritical]
        public WPFMessageBoxResult Show(string messageBoxText, string caption, WPFMessageBoxButtons button)
        {
            return Show(null, messageBoxText, caption, button, WPFMessageBoxImage.None, WPFMessageBoxResult.None, new WPFMessageBoxOptions());
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays a message and title bar caption; and it returns a result.
        /// </summary>
        /// <param name="ownerKey">The key of the registered window by the <see cref="RegisterOwner(Window, object)" /> that represents the owner window of the message box. If the key is null or no window is known with the key, the box will be called without an owner.</param>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <returns>A WPFMessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        [SecurityCritical]
        public WPFMessageBoxResult Show(object ownerKey, string messageBoxText, string caption)
        {
            return Show(ownerKey, messageBoxText, caption, WPFMessageBoxButtons.OK, WPFMessageBoxImage.None, WPFMessageBoxResult.None, new WPFMessageBoxOptions());
        }

        /// <summary>
        /// Displays a message box that has a message, title bar caption, button, and icon; and that returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A WPFMessageBoxButtons value that specifies which button or buttons to display.</param>
        /// <param name="icon">A WPFMessageBoxImage value that specifies the icon to display.</param>
        /// <returns>A WPFMessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        [SecurityCritical]
        public WPFMessageBoxResult Show(string messageBoxText, string caption, WPFMessageBoxButtons button, WPFMessageBoxImage icon)
        {
            return Show(null, messageBoxText, caption, button, icon, WPFMessageBoxResult.None, new WPFMessageBoxOptions());
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays a message, title bar caption, and button; and it also returns a result.
        /// </summary>
        /// <param name="ownerKey">The key of the registered window by the <see cref="RegisterOwner(Window, object)" /> that represents the owner window of the message box. If the key is null or no window is known with the key, the box will be called without an owner.</param>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A WPFMessageBoxButtons value that specifies which button or buttons to display.</param>
        /// <returns>A WPFMessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        [SecurityCritical]
        public WPFMessageBoxResult Show(object ownerKey, string messageBoxText, string caption, WPFMessageBoxButtons button)
        {
            return Show(ownerKey, messageBoxText, caption, button, WPFMessageBoxImage.None, WPFMessageBoxResult.None, new WPFMessageBoxOptions());
        }

        /// <summary>
        /// Displays a message box that has a message, title bar caption, button, and icon; and that accepts a default message box result and returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A WPFMessageBoxButtons value that specifies which button or buttons to display.</param>
        /// <param name="icon">A WPFMessageBoxImage value that specifies the icon to display.</param>
        /// <param name="defaultResult">A WPFMessageBoxResult value that specifies the default result of the message box.</param>
        /// <returns>A WPFMessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        [SecurityCritical]
        public WPFMessageBoxResult Show(string messageBoxText, string caption, WPFMessageBoxButtons button, WPFMessageBoxImage icon, WPFMessageBoxResult defaultResult)
        {
            return Show(null, messageBoxText, caption, button, icon, defaultResult, new WPFMessageBoxOptions());
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays a message, title bar caption, button, and icon; and it also returns a result.
        /// </summary>
        /// <param name="ownerKey">The key of the registered window by the <see cref="RegisterOwner(Window, object)" /> that represents the owner window of the message box. If the key is null or no window is known with the key, the box will be called without an owner.</param>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A WPFMessageBoxButtons value that specifies which button or buttons to display.</param>
        /// <param name="icon">A WPFMessageBoxImage value that specifies the icon to display.</param>
        /// <returns>A WPFMessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        [SecurityCritical]
        public WPFMessageBoxResult Show(object ownerKey, string messageBoxText, string caption, WPFMessageBoxButtons button, WPFMessageBoxImage icon)
        {
            return Show(ownerKey, messageBoxText, caption, button, icon, WPFMessageBoxResult.None, new WPFMessageBoxOptions());
        }

        /// <summary>
        /// Displays a message box that has a message, title bar caption, button, and icon; and that accepts a default message box result, complies with the specified options, and returns a result.
        /// </summary>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A WPFMessageBoxButtons value that specifies which button or buttons to display.</param>
        /// <param name="icon">A WPFMessageBoxImage value that specifies the icon to display.</param>
        /// <param name="defaultResult">A WPFMessageBoxResult value that specifies the default result of the message box.</param>
        /// <param name="options">A WPFMessageBoxOptions value object that specifies the options.</param>
        /// <returns>A WPFMessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        [SecurityCritical]
        public WPFMessageBoxResult Show(string messageBoxText, string caption, WPFMessageBoxButtons button, WPFMessageBoxImage icon, WPFMessageBoxResult defaultResult, WPFMessageBoxOptions options)
        {
            return Show(null, messageBoxText, caption, button, icon, defaultResult, options);
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays a message, title bar caption, button, and icon; and accepts a default message box result and returns a result.
        /// </summary>
        /// <param name="ownerKey">The key of the registered window by the <see cref="RegisterOwner(Window, object)" /> that represents the owner window of the message box. If the key is null or no window is known with the key, the box will be called without an owner.</param>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A WPFMessageBoxButtons value that specifies which button or buttons to display.</param>
        /// <param name="icon">A WPFMessageBoxImage value that specifies the icon to display.</param>
        /// <param name="defaultResult">A WPFMessageBoxResult value that specifies the default result of the message box.</param>
        /// <returns>A WPFMessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        [SecurityCritical]
        public WPFMessageBoxResult Show(object ownerKey, string messageBoxText, string caption, WPFMessageBoxButtons button, WPFMessageBoxImage icon, WPFMessageBoxResult defaultResult)
        {
            return Show(ownerKey, messageBoxText, caption, button, icon, defaultResult, new WPFMessageBoxOptions());
        }

        /// <summary>
        /// Displays a message box in front of the specified window. The message box displays a message, title bar caption, button, and icon; and accepts a default message box result, complies with the specified options, and returns a result.
        /// </summary>
        /// <param name="ownerKey">The key of the registered window by the <see cref="RegisterOwner(Window, object)" /> that represents the owner window of the message box. If the key is null or no window is known with the key, the box will be called without an owner.</param>
        /// <param name="messageBoxText">A System.String that specifies the text to display.</param>
        /// <param name="caption">A System.String that specifies the title bar caption to display.</param>
        /// <param name="button">A WPFMessageBoxButtons value that specifies which button or buttons to display.</param>
        /// <param name="icon">A WPFMessageBoxImage value that specifies the icon to display.</param>
        /// <param name="defaultResult">A WPFMessageBoxResult value that specifies the default result of the message box.</param>
        /// <param name="options">A WPFMessageBoxOptions value object that specifies the options.</param>
        /// <returns>A WPFMessageBoxResult value that specifies which message box button is clicked by the user.</returns>
        [SecurityCritical]
        public WPFMessageBoxResult Show(object ownerKey, string messageBoxText, string caption, WPFMessageBoxButtons button, WPFMessageBoxImage icon, WPFMessageBoxResult defaultResult, WPFMessageBoxOptions options)
        {
            if (ownerKey != null && _owner.ContainsKey(ownerKey))
                return WPFMessageBox.Show(_owner[ownerKey], messageBoxText, caption, button, icon, defaultResult, options);
            return WPFMessageBox.Show(messageBoxText, caption, button, icon, defaultResult, options);
        }
    }
}