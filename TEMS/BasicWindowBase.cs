// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows;
using NLog;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Base Interaction logic for Windows
    /// </summary>
    public abstract partial class BasicWindowBase : Window
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public BasicWindowBase(ViewModelBase ViewModel) : base()
        {
            logger.Debug($"Creating BasicWindowBase for view model {typeof(ViewModelBase).Name}");
            if (ViewModel is null) throw new ArgumentNullException("ViewModel", "ViewModel cannot be null");
            this.ViewModel = ViewModel;
            this.DataContext = this.ViewModel;
        }

        public ViewModelBase ViewModel { get; set; }
    }
}
