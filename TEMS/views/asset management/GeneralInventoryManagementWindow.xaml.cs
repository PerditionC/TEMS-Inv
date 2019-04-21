// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using TEMS_Inventory.views;

namespace TEMS_Inventory
{
    /// <summary>
    /// Interaction logic for GeneralInventoryManagementWindow.xaml
    /// </summary>
    public partial class GeneralInventoryManagementWindow : BasicSearchWindowBase
    {
        public GeneralInventoryManagementWindow() : base()
        {
            ViewModel = new GeneralInventoryManagementViewModel();
            this.DataContext = ViewModel;
            InitializeComponent();
            InitializeViewModel();
        }
    }
}
