// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.ObjectModel;
using System.Linq;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.entity.db;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// base class for detail pane view models that show a list of events
    /// subclasses may add additional ICommand actions to perform on selected event
    /// </summary>
    public abstract class EventHistoryViewModelBase : DetailsViewModelBase
    {
        /// <summary>
        /// maintains results from last Search (or empty list initially)
        /// we use ItemBase so we can ensure all nonNull constraints are satisfied prior to saving
        /// </summary>
        public ObservableCollection<ItemBase> EventList
        {
            get { return _EventList; }
            set
            {
                if (value == null) value = new ObservableCollection<ItemBase>();
                SetProperty(ref _EventList, value, nameof(EventList));
                SelectedEvent = EventList.FirstOrDefault();
            }
        }
        private ObservableCollection<ItemBase> _EventList = new ObservableCollection<ItemBase>();

        /// <summary>
        /// maintains currently selected item from last search (or null if nothing currently selected)
        /// Note: this value may be set by update to search results, updated from binding to user list and new item
        /// selected by user, or set explicitly (e.g. new item created and marked as selected)
        /// </summary>
        public ItemBase SelectedEvent
        {
            get { return _SelectedEvent; }
            set
            {
                // clear status on new selection
                StatusMessage = string.Empty;

                SetProperty(ref _SelectedEvent, value, nameof(SelectedEvent));
                RaisePropertyChanged(nameof(IsEventSelected));
            }
        }
        private ItemBase _SelectedEvent;


        /// <summary>
        /// read-only property indicating if an event is currently selected or not
        /// </summary>
        /// <returns>true if SelectedEvent has a value; false if nothing selected i.e. SelectedEvent is null</returns>
        public bool IsEventSelected
        {
            get { return SelectedEvent != null; }
        }


        /// <summary>
        /// Command to open event selected so can be modified/viewed
        /// </summary>
        public ICommand UpdateCommand
        {
            get { return InitializeCommand(ref _UpdateCommand, param => DoUpdateCommand(), param => { return IsEventSelected; }); }
        }
        private ICommand _UpdateCommand;

        /// <summary>
        /// Must implement this, it should do whatever is appropriate
        /// e.g. create a new window, initialize its SearchFilter, and then call ShowWindow(newWin);
        /// </summary>
        protected abstract void DoUpdateCommand();
    }
}
