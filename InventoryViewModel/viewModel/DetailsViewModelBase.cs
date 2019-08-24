// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// base class for all detail pane view models
    /// used for strong typing only
    /// </summary>
    public abstract class DetailsViewModelBase : ViewModelBase
    {
        public DetailsViewModelBase() : base()
        {
            //Mediator.Register(nameof(CurrentItemChangedMessage), (msg) => this.CurrentItem = ((CurrentItemChangedMessage)msg).CurrentItem);
            //Mediator.Register(nameof(CurrentUserChangedMessage), (msg) => { RaisePropertyChanged(nameof(IsAdmin)); });
        }

        public SearchResult CurrentItem
        {
            get { return _CurrentItem; }
            set { SetProperty(ref _CurrentItem, value, nameof(CurrentItem)); RaisePropertyChanged(nameof(IsCurrentItemNull)); }
        }
        private SearchResult _CurrentItem = null;

        public bool IsCurrentItemNull { get { return _CurrentItem == null; } }
    }
}
