// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;

namespace TEMS_Inventory.views
{
    public class HistoryDamagedMissingViewModel : EventHistoryViewModelBase
    {
        public HistoryDamagedMissingViewModel() : base() { }

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        protected override void DoUpdateCommand()
        {
            var viewModel = new DetailsDamagedMissingViewModel(SelectedEvent as DamageMissingEvent);
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
        }


        /// <summary>
        /// Command to create an event record of item as damaged
        /// </summary>
        public ICommand DamagedCommand
        {
            get { return InitializeCommand(ref _DamagedCommand, param => DoDamagedCommand(), param => !IsCurrentItemNull); }
        }
        private ICommand _DamagedCommand;

        private void DoDamagedCommand()
        {
            /*
            var newWin = new DamagedMissingDetailsWindow();
            //searchFilter.SearchText = (currentItem as ItemInstance)?.itemNumber?.ToString() ?? "";
            ShowChildWindow(newWin);
            */
        }


        /// <summary>
        /// Command to create an event record of item as missing
        /// </summary>
        public ICommand MissingCommand
        {
            get { return InitializeCommand(ref _MissingCommand, param => DoMissingCommand(), param => !IsCurrentItemNull); }
        }
        private ICommand _MissingCommand;

        private void DoMissingCommand()
        {
            /*
            var newWin = new DamagedMissingDetailsWindow();
            ShowChildWindow(newWin);
            */
        }

    }
}
