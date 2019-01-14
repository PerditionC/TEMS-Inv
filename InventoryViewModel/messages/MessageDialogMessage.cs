// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// message argument passed to ShowMessage Dialog via Mediator.InvokeCallbacks
    /// </summary>
    public class MessageDialogMessage
    {
        public string caption;
        public string message;
    }
}
