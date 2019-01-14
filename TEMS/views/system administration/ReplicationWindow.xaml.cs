// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows;
using NLog;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for ReplicationWindow.xaml
    /// </summary>
    public partial class ReplicationWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ReplicationWindow()
        {
            InitializeComponent();
        }

        public ReplicationViewModel ViewModel
        {
            get { return (ReplicationViewModel)Resources["ViewModel"]; }
        }
    }
}
