﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for GeneralInventoryManagementWindow.xaml
    /// </summary>
    public partial class GeneralInventoryManagementWindow : Window
    {
        public GeneralInventoryManagementWindow(GeneralInventoryManagementViewModel ViewModel) : base()
        {
            InitializeComponent();
        }
    }
}
