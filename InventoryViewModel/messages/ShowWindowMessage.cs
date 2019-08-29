// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// message argument passed to ShowYesNoDialog via Mediator.InvokeCallbacks
    /// </summary>
    public class ShowWindowMessage
    {
        public string windowName;        // which window to open
        public bool modal;               // true = dialog or false = non-modal window
        public bool childWindow;         // true to set as child of current window
        public Action<object> callback;  // callback to invoke with window result (may be null if unused)
        public object args;              // optional arguments to pass to new window (may be null if unused)
        public string searchText;        // usually null, set to specific value to open a search window & preset Search Filter SearchText to this value & invoke search
    }
}
