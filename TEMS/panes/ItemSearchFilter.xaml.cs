// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows.Controls;
using NLog;


namespace TEMS_Inventory.UserControls
{
    /// <summary>
    /// Interaction logic for ItemSearchFilter.xaml
    /// Set DataContext to a SearchFilterOptionsViewModel instance
    /// </summary>
    public partial class ItemSearchFilter : UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ItemSearchFilter()
        {
            InitializeComponent();
        }
    }
}
