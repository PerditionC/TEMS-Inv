// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// based on https://github.com/RSuter/MyToolkit
// license: MS Public License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace InventoryViewWPF
{
    public class FileOpenPicker : FilePickerBase
    {
        protected override void SelectFile()
        {
            var dlg = new OpenFileDialog
            {
                DefaultExt = DefaultExtension,
                Filter = Filter
            };
            if (dlg.ShowDialog() == true)
                FilePath = dlg.FileName;
        }
    }
}
