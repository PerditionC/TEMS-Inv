// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// message argument passed to ShowYesNoDialog via Mediator.InvokeCallbacks
    /// </summary>
    public class OpenFileDialogMessage : MessageDialogMessage
    {
        //public string caption;
        //public string message;
        public string InitialDirectory;
        public string DefaultExt;
        public string Filter;
        public bool CheckFileExists;
        public bool ShowReadOnly;
        public bool DereferenceLinks;
        public bool Multiselect;

        public Action<object> SelectedAction;
        public Action<object> CanceledAction;
    }
}
