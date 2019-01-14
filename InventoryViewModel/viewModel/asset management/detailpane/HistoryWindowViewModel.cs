// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.ObjectModel;
using System.Linq;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    public abstract class HistoryWindowViewModel : MasterDetailItemWindowViewModelBase
    {
        // anything that needs initializing for MSVC designer
        public HistoryWindowViewModel(SearchFilterItems SearchFilter) : base(QueryResultEntitySelector.ItemInstance, SearchFilter)
        {
            Mediator.Register(nameof(CurrentItemChangedMessage), (msg) =>
            {
                var selItem = ((CurrentItemChangedMessage)msg).SelectedItem;
                if (selItem != null)
                    UpdateEvents(selItem);
                else
                    Events = new ObservableCollection<ItemBase>();
            });
        }

        /// <summary>
        /// Invoked when the current SelectedItem changes to update corresponding Events collection for newly selected item(s)
        /// </summary>
        /// <param name="SelectedItem"></param>
        public abstract void UpdateEvents(SearchResult SelectedItem);

        /// <summary>
        /// Events for currently selected item (not to be confused with Items list)
        /// we use ItemBase so we can ensure all nonNull constraints are satisfied prior to saving
        /// </summary>
        public ObservableCollection<ItemBase> Events
        {
            get { return _Events; }
            set
            {
                SetProperty(ref _Events, value, nameof(Events));
                SelectedEvent = Events.FirstOrDefault();
            }
        }
        private ObservableCollection<ItemBase> _Events = new ObservableCollection<ItemBase>();

        /// <summary>
        /// list item selected
        /// </summary>
        public ItemBase SelectedEvent
        {
            get { return _SelectedEvent; }
            set { SetProperty(ref _SelectedEvent, value, nameof(SelectedEvent)); RaisePropertyChanged(nameof(IsEventSelected)); }
        }
        private ItemBase _SelectedEvent = null;

        /// <summary>
        /// read-only property indicating if an item is currently selected or not
        /// </summary>
        /// <returns>true if SelectedItem valid, false if nothing selected, SelectedEvent is null</returns>
        protected bool IsEventSelected()
        {
            return SelectedEvent != null;
        }

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand UpdateCommand
        {
            get { return InitializeCommand(ref _UpdateCommand, param => DoUpdateCommand(), param => IsEventSelected()); }
        }
        private ICommand _UpdateCommand;

        /// <summary>
        /// Must implement this, it should do whatever is appropriate
        /// e.g. create a new window, initialize its SearchFilter, and then call ShowWindow(newWin);
        /// </summary>
        protected abstract void DoUpdateCommand();
    }
}
