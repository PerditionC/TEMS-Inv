// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for DamagedMissingDetailsWindow.xaml
    /// </summary>
    public partial class DamagedMissingDetailsWindow : BasicWindowBase
    {
        public DamagedMissingDetailsWindow(DetailsDamagedMissingViewModel ViewModel) : base(ViewModel)
        {
            InitializeComponent();
        }
    }
}
