// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NLog;
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
