// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows;
using NLog;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for Basic Overview/History Windows
    /// </summary>
    public abstract partial class BasicWindowBase : Window
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public BasicWindowBase() : base() { }

        /// <summary>
        /// This must be called after InitializeComponents to initialize ViewModel
        /// (or subclass can directly call ViewModel.Initialize(...); after InitializeComponents)
        /// Note: ViewModel not instantiated until components are initialized as it is declared as XAML resource
        /// </summary>
        /// <param name="GetNewItem"></param>
        protected void InitializeViewModel()
        {
            this.Activated += DoInitialSearch;
        }

        /// <summary>
        /// Called only after 1st time Window is activated to load initial values.
        /// Assumes initial SearchFilter parameters have been established by time loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void DoInitialSearch(object sender, EventArgs e)
        {
            this.Activated -= DoInitialSearch;
            // TODO load full item????
        }

        public ViewModelBase ViewModel { get; set; }
    }
}
