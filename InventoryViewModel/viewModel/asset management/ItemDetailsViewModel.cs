// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
using InventoryViewModel;
#endif

using TEMS.InventoryModel.command.action;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.userManager;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    public class ItemDetailsViewModel : DetailsViewModelBase
    {
        public ItemDetailsViewModel() : base()
        {
        }

        /// <summary>
        /// internal DB primary key, unique per item
        /// Note: GUID used for replication purposes
        /// </summary>
        public Guid guid { get { return _guid; } set { SetProperty(ref _guid, value, nameof(guid)); } }
        private Guid _guid = Guid.Empty;


        /// <summary>
        /// allow binding to reference items that are cached
        /// </summary>
        public ReferenceDataCache cache { get; private set; } = DataRepository.GetDataRepository.ReferenceData;
        
        /// <summary>
        /// Initialize to nothing selected to display details of
        /// </summary>
        public override void clear()
        {
            base.clear();
            guid = Guid.Empty;
        }

        /// <summary>
        /// returns true only if current (selected) search result item is not null and it is an editable item (i.e. represents actual item not header)
        /// </summary>
        public override bool IsCurrentItemEditable { get { return base.IsCurrentItemEditable && (CurrentItem.entityType != null); } }

        /// <summary>
        /// ICommand to persist information to our backing store pPerform the actual save to DB)
        /// SaveItemCommand will ensure Item and ItemInstances correctly added together
        /// (does DB insert / update as required)
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                return InitializeCommand(
                    ref _SaveCommand,
                    param =>
                    {
                        try
                        {
                            //var item = Mapper.GetMapper().Map<Item>(this);
                            ItemBase item = (ItemBase)Mapper.GetMapper().Map(this, this.GetType(), Type.GetType(CurrentItem.entityType));
                            if (saveItemCommand.CanExecute(item)) saveItemCommand.Execute(item);

                            // after saving update tree (will reload from db hence must be done after saving)
                            // *** TODO SENDMSG VM.Search(id) to rerun current search criteria, but then set selected item to one with matching id
                        }
                        catch (Exception e)
                        {
                            // don't throw
                            StatusMessage = $"Failed to remove Item - {e.Message}";
                        }
                    },
                    param => { return IsCurrentItemNotNull && (guid != Guid.Empty); }
                );
            }
        }
        private ICommand _SaveCommand;
        private ICommand saveItemCommand = new SaveItemCommand();


        /// <summary>
        /// Command to remove an item (and associated ItemInstances)
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return InitializeCommand(
                    ref _DeleteCommand,
                    param =>
                    {
                        try
                        {
                            ItemBase item = (ItemBase)Mapper.GetMapper().Map(this, this.GetType(), Type.GetType(CurrentItem.entityType));
                            if (deleteItemCommand.CanExecute(item)) DoDelete(item);

                            // after saving update tree (will reload from db hence must be done after deletion)
                            // *** TODO SENDMSG VM.Search(null) to rerun current search criteria, but with nothing selected
                        }
                        catch (Exception e)
                        {
                            // don't throw
                            StatusMessage = $"Failed to remove Item - {e.Message}";
                        }
                    },
                    param => { return IsCurrentItemNotNull && (guid != Guid.Empty); }
                );
            }
        }
        private ICommand _DeleteCommand;
        private ICommand deleteItemCommand = new DeleteItemCommand();

        /// <summary>
        /// Removes the selected item from list of items including marking deleted in database
        /// </summary>
        private void DoDelete(ItemBase item)
        {
            Mediator.InvokeCallback(nameof(YesNoDialogMessage),
                new YesNoDialogMessage
                {
                    caption = "Delete",
                    message = "Do you want to delete this item?",
                    NoAction = (x) => { /* do nothing */ },
                    YesAction = (x) =>
                    {
                        try
                        {
                            // actually remove from DB
                            if (CurrentItem != null) deleteItemCommand.Execute(item);
                            // initiate a new search and selection to update search pane
                        }
                        catch (Exception e)
                        {
                            logger.Error(e, $"Failed to delete selected item! {guid}");
                            //throw; swallow error, todo - show an error!
                        }
                    },

                });
        }
    }
}
