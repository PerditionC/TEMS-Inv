// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Interaction logic for Basic Overview/History Windows
    /// </summary>
    public abstract partial class BasicSearchWindowBase : BasicWindowBase
    {
        public BasicSearchWindowBase() : base() { }
        /// <summary>
        /// Called only after 1st time Window is activated to load initial values.
        /// Assumes initial SearchFilter parameters have been established by time loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void DoInitialSearch(object sender, EventArgs e)
        {
            this.Activated -= DoInitialSearch;
            // should trigger a searchFilter.SearchCommand.Execute() to do initial data load
            // [but only once, avoiding load of data when Window created & then reloaded as part of initialization]
            var vm = ViewModel as SearchWindowViewModelBase;
            vm.SearchFilter.SearchFilterEnabled = true;
        }
    }
}
