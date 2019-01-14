// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif
using NLog;
using TEMS.InventoryModel.entity.db;


namespace TEMS_Inventory.views
{
    public class ManageVendorsViewModel : ItemListToAddEditDeleteViewModel
    {
        // anything that needs initializing for MSVC designer
        public ManageVendorsViewModel() : base(delegate () { return (ItemBase)Activator.CreateInstance(typeof(VendorDetail)); })
        {
            // listen for property changes to ourself
            this.PropertyChanged += new PropertyChangedEventHandler(ManageVendorsViewModel_PropertyChanged);

            // load initial/all the vendor information
            DoSearchVendorsCommand();

            // if user edits vendor collection then we need be sure to reflect changes in our reference data cache
            items.CollectionChanged += new NotifyCollectionChangedEventHandler(ManageVendorsViewModel_VendorsChanged);
        }

        private void ManageVendorsViewModel_PropertyChanged(Object sender, PropertyChangedEventArgs eventArgs)
        {
            // on currentItem change update items for vendor
            if (eventArgs.PropertyName == "currentItem")
            {
                UpdateVendorItemList();
            }
        }

        private void ManageVendorsViewModel_VendorsChanged(Object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            try
            {
                logger.Debug("Vendor list changed.");
                // handle individual change types if (eventArgs.action==???) ...
                // for now just refresh the whole list
                // Note this will happen even when the vendor list is searched for, not just actual changes
                // but not an issue since we don't refresh any displayed lists on each invalidation
                DataRepository.GetDataRepository?.ReferenceData?.RefreshData(nameof(VendorDetail));
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error updating cached vendor list - {e.Message}");
                throw;
            }
        }


        #region vendor items

        public ObservableCollection<string> VisibleColumnNames
        {
            get { return _VisibleColumnNames; }
            set { SetProperty(ref _VisibleColumnNames, value, nameof(VisibleColumnNames)); }
        }
        private ObservableCollection<string> _VisibleColumnNames = new ObservableCollection<string>();

        public ObservableCollection<ItemBase> VendorItems
        {
            get { return _VendorItems; }
            set { SetProperty(ref _VendorItems, value, nameof(VendorItems)); }
        }
        private ObservableCollection<ItemBase> _VendorItems = new ObservableCollection<ItemBase>();

        private void UpdateVendorItemList()
        {
            if (currentItem != null)
            {
                VendorItems = DataRepository.GetDataRepository.GetVendorItemTypes((Guid)currentItem.PrimaryKey);
            }
            else
            {
                VendorItems = new ObservableCollection<ItemBase>(); // empty list
            }
        }

        #endregion // vendor items

        #region Search for Vendor

        public string SearchVendorText
        {
            get { return _SearchVendorText; }
            set { SetProperty(ref _SearchVendorText, value, nameof(SearchVendorText)); }
        }
        private string _SearchVendorText;

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand SearchVendorsCommand
        {
            get { return InitializeCommand(ref _OpenManageVendorsWindowCommand, param => DoSearchVendorsCommand(), null); }
        }
        private ICommand _OpenManageVendorsWindowCommand;

        #endregion // Commands

        #region ICommand Actions

        private void DoSearchVendorsCommand()
        {
            logger.Debug("Loading item types - DoSearch:\n" + SearchVendorText);

            items = new ObservableCollection<ItemBase>((DataRepository.GetDataRepository.ReferenceData[nameof(VendorDetail)]).Where(x => (x as VendorDetail).name.ToUpper().Contains((SearchVendorText ?? "").ToUpper())));
            // auto select if only 1 item type returned
            //if (items.Count == 1) selectedListItem = items[0];
        }

        #endregion // Search for Vendor
    }
}
