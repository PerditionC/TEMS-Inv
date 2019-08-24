// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for GeneralInventoryManagementWindow.xaml
    /// </summary>
    public partial class ItemInstanceManagementWindow : BasicWindowBase
    {
        public ItemInstanceManagementWindow(ItemInstanceManagementViewModel ViewModel) : base(ViewModel)
        {
            InitializeComponent();
        }
    }
}
