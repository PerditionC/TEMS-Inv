// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif
using NLog;

namespace TEMS_Inventory.views
{
    public class ReferenceDataViewModel : ViewModelBase
    {
        // anything that needs initializing for MSVC designer
        public ReferenceDataViewModel() : base() { }
    }
}
