// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util.attribute;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// Administration CRUD view model for updating ItemInstance table
    /// </summary>
    public class ItemInstanceManagementViewModel : ItemDetailsViewModel
    {
        public ItemInstanceManagementViewModel() : base() { }

        /// <summary>
        /// Initialize to nothing selected to display details of
        /// </summary>
        public override void clear()
        {
            base.clear();

            item = null;
            siteLocation = null;
            serialNumber = null;
            grantNumber = null;
            status = null;
            inServiceDate = DateTime.MinValue;
            removedServiceDate = null;
            isSealBroken = false;
            hasBarcode = false;
            notes = null;
        }

        public bool HasRemovedFromServiceDate
        {
            get { return IsCurrentItemEditable && string.Equals("Removed From Inventory", status?.name, StringComparison.InvariantCultureIgnoreCase); }
        }

        public bool CanHaveSerialNumber
        {
            get { return IsCurrentItemEditable && (item?.count == 1); }
        }


        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand OpenEditItemWindowCommand
        {
            get { return InitializeCommand(ref _OpenEditItemWindowCommand, param => DoOpenEditItemWindowCommand(), param => { return IsCurrentItemNotNull && IsAdmin; }); }
        }
        private ICommand _OpenEditItemWindowCommand;

        private void DoOpenEditItemWindowCommand()
        {
            ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, windowName = "ManageItem", searchText = (CurrentItem == null) ? null : itemNumber?.ToString() });
        }


        // primary external id; as used in barcode, e.g. D236-19807-NFR
        [DisplayNameProperty]
        public string itemNumber
        {
            get
            {
                if (CurrentItem is null)
                {
                    return "No selection";
                }
                else if (CurrentItem is GenericItemResult)
                {
                    return $"{item?.itemNumber}-{siteLocation?.locSuffix}";
                }
                else if (CurrentItem is GroupHeader)
                {
                    if (CurrentItem is BinGroupHeader)
                        return "Bin";
                    else if (CurrentItem is ModuleGroupHeader)
                        return "Module";
                    else if (CurrentItem is ItemGroupHeader)
                        return "Item";
                    else
                        return "...";
                }
                else
                {
                    return "";
                }
            }
        }

        // item specific details for a the specific equipment unit this item instance is part of
        public Item item
        {
            get { return _item; }
            set
            {
                SetProperty(ref _item, value, nameof(item));
                RaisePropertyChanged(nameof(itemNumber));
                RaisePropertyChanged(nameof(item.itemType));
                RaisePropertyChanged(nameof(CanHaveSerialNumber));
            }
        }
        private Item _item;

        // jurisdiction this specific item is located at
        public SiteLocation siteLocation
        {
            get { return _siteLocation; }
            set
            {
                SetProperty(ref _siteLocation, value, nameof(siteLocation));
                RaisePropertyChanged(nameof(itemNumber));
            }
        }
        private SiteLocation _siteLocation;


        // unique serial # if applicable
        public string serialNumber
        {
            get { return _serialNumber; }
            set
            {
                SetProperty(ref _serialNumber, value, nameof(serialNumber));
                //RaisePropertyChanged(nameof(CanHaveSerialNumber));
            }
        }
        private string _serialNumber = null;

        // grant #, external reference # for grant purposes
        public string grantNumber { get { return _grantNumber; } set { SetProperty(ref _grantNumber, value, nameof(grantNumber)); } }
        private string _grantNumber = null;

        // item status, Available, Damaged, etc.
        public ItemStatus status
        {
            get { return _status; }
            set
            {
                SetProperty(ref _status, value, nameof(status));
                RaisePropertyChanged(nameof(HasRemovedFromServiceDate));
            }
        }
        private ItemStatus _status;

        // initial service date
        public DateTime inServiceDate { get { return _inServiceDate; } set { SetProperty(ref _inServiceDate, value, nameof(inServiceDate)); } }
        private DateTime _inServiceDate = DateTime.MinValue;

        // when removed from service, null if still in service
        public DateTime? removedServiceDate { get { return _removedServiceDate; } set { SetProperty(ref _removedServiceDate, value, nameof(removedServiceDate)); } }
        private DateTime? _removedServiceDate = null;

        // has seal been broken
        public bool isSealBroken { get { return _isSealBroken; } set { SetProperty(ref _isSealBroken, value, nameof(isSealBroken)); } }
        private bool _isSealBroken = false;

        // has barcode been affixed to item
        // barcode is itemNumber
        public bool hasBarcode { get { return _hasBarcode; } set { SetProperty(ref _hasBarcode, value, nameof(hasBarcode)); } }
        private bool _hasBarcode = false;

        // additional remarks about item
        public string notes { get { return _notes; } set { SetProperty(ref _notes, value, nameof(notes)); } }
        private string _notes = null;
    }
}
