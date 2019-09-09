// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.command.action;
using TEMS.InventoryModel.entity.db;

namespace TEMS_Inventory.views
{
    public class EventDetailsViewModel : DetailsViewModelBase
    {
        public EventDetailsViewModel(ItemBase Event) : base()
        {
            this.Event = Event;
        }

        private ItemBase _Event = null;
        public ItemBase Event { get { return _Event; } set { SetProperty(ref _Event, value, nameof(Event)); } }

        public ICommand SaveCommand = new SaveItemCommand();
    }
}
