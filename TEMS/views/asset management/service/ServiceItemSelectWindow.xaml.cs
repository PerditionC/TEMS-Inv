﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for ServiceItemSelectWindow.xaml
    /// </summary>
    public partial class ServiceItemSelectWindow : BasicListAndDetailWindowBase
    {
        public ServiceItemSelectWindow()
        {
            InitializeComponent();
            InitializeViewModel();
        }
    }
}
