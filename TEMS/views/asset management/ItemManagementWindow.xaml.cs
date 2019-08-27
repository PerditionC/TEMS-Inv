// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for ItemManagementWindow.xaml
    /// </summary>
    public partial class ItemManagementWindow : BasicWindowBase
    {
        public ItemManagementWindow(SearchDetailWindowViewModel ViewModel) : base(ViewModel)
        {
            InitializeComponent();
        }
    }
}
