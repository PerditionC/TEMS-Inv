// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif
using NLog;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    public class ViewModelBase : NotifyPropertyChanged
    {
        public ViewModelBase() : base() { }

        /// <summary>
        /// helper used to indicate should show extra debug information
        /// </summary>
        /// <returns></returns>
        public bool IsDebugMode
        {
            get { return _IsDebugMode; }
            set { SetProperty(ref _IsDebugMode, value, nameof(IsDebugMode)); }
        }
        private bool _IsDebugMode =
#if DEBUG
            true;
#else
            false;
#endif

        /// <summary>
        /// Helper to initialize ICommand objects on first use, otherwise returns existing item.
        /// </summary>
        /// <param name="cmd">the ICommand object to initialize</param>
        /// <param name="initialText">text displayed on view to activate this command</param>
        /// <param name="execute">delegate to perform the command</param>
        /// <param name="canExecute">delegate to indicate if command should be active, ie can perform command</param>
        /// <returns></returns>
        protected ICommand InitializeCommand(ref ICommand cmd, Action<object> execute, Predicate<object> canExecute)
        {
            if (cmd == null)
            {
                cmd = new RelayCommand(execute, canExecute);
            }
            return cmd;
        }


        /// <summary>
        /// Used by subclasses to open another Window.  WindowOpts determines
        /// how window is to be displayed (modal or non-modal).
        /// ViewModel's SearchFilter should be initialized prior to this call.
        /// ? Modal ?
        /// </summary>
        /// <param name="newWin"></param>
        protected void ShowChildWindow(ShowWindowMessage windowOpts)
        {
            StatusMessage = $"Opening window {windowOpts.windowName}";
            Mediator.InvokeCallback(nameof(ShowWindowMessage), windowOpts);
            StatusMessage = string.Empty;
        }

        /// <summary>
        /// Current status, e.g. for display on a status line
        /// </summary>
        private string _StatusMessage = string.Empty;
        public string StatusMessage { get { return _StatusMessage; } set { SetProperty(ref _StatusMessage, value, nameof(StatusMessage)); } }
    }
}
