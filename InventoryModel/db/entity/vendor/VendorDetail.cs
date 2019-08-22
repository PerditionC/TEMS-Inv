// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using SQLite;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Information about business (vendor) for acquiring/servicing items
    /// </summary>
    public class VendorDetail : ReferenceData
    {
        public VendorDetail() : base()
        {
        }

        public VendorDetail(Guid id) : base(id)
        {
        }

        public VendorDetail(VendorDetail copyOfObj) : base(copyOfObj)
        {
            _category = copyOfObj._category;
            _notes = copyOfObj._notes;
            _addressLine1 = copyOfObj._addressLine1;
            _addressLine2 = copyOfObj._addressLine2;
            _city = copyOfObj._city;
            _state = copyOfObj._state;
            _zipcode = copyOfObj._zipcode;
            _phoneNumber = copyOfObj._phoneNumber;
            _faxNumber = copyOfObj._faxNumber;
            _website = copyOfObj._website;
            _isActive = copyOfObj._isActive;
            _contactName = copyOfObj._contactName;
            _contactPhoneNumber = copyOfObj._contactPhoneNumber;
            _contactEmail = copyOfObj._contactEmail;
            _accountReference = copyOfObj._accountReference;
        }

        // optional, tag (category) for this vendor
        [MaxLength(64)]
        public string category { get { return _category; } set { SetProperty(ref _category, value, nameof(category)); } }

        private string _category = null;

        // additional information about vendor
        [MaxLength(256)]
        public string notes { get { return _notes; } set { SetProperty(ref _notes, value, nameof(notes)); } }

        private string _notes = null;

        // address
        [MaxLength(64)]
        public string addressLine1 { get { return _addressLine1; } set { SetProperty(ref _addressLine1, value, nameof(addressLine1)); } }

        private string _addressLine1 = null;

        [MaxLength(64)]
        public string addressLine2 { get { return _addressLine2; } set { SetProperty(ref _addressLine2, value, nameof(addressLine2)); } }

        private string _addressLine2 = null;

        [MaxLength(64)]
        public string city { get { return _city; } set { SetProperty(ref _city, value, nameof(city)); } }

        private string _city = null;

        [MaxLength(32)]
        public string state { get { return _state; } set { SetProperty(ref _state, value, nameof(state)); } }

        private string _state = null;

        [MaxLength(16)]
        public string zipcode { get { return _zipcode; } set { SetProperty(ref _zipcode, value, nameof(zipcode)); } }

        private string _zipcode = null;

        // primary general business phone#
        [MaxLength(16)]
        public string phoneNumber { get { return _phoneNumber; } set { SetProperty(ref _phoneNumber, value, nameof(phoneNumber)); } }

        private string _phoneNumber = null;

        // primary general business fax#
        [MaxLength(16)]
        public string faxNumber { get { return _faxNumber; } set { SetProperty(ref _faxNumber, value, nameof(faxNumber)); } }

        private string _faxNumber = null;

        // URL for additional vendor information
        [MaxLength(256)]
        public string website { get { return _website; } set { SetProperty(ref _website, value, nameof(website)); } }

        private string _website = null;

        // is this still an active vendor or historical
        public bool isActive { get { return _isActive; } set { SetProperty(ref _isActive, value, nameof(isActive)); } }

        private bool _isActive = true;

        #region contact/account details

        // contact name
        [MaxLength(32)]
        public string contactName { get { return _contactName; } set { SetProperty(ref _contactName, value, nameof(contactName)); } }

        private string _contactName = null;

        // contact primary phone#
        [MaxLength(16)]
        public string contactPhoneNumber { get { return _contactPhoneNumber; } set { SetProperty(ref _contactPhoneNumber, value, nameof(contactPhoneNumber)); } }

        private string _contactPhoneNumber = null;

        // contact primary email
        [MaxLength(64)]
        public string contactEmail { get { return _contactEmail; } set { SetProperty(ref _contactEmail, value, nameof(contactEmail)); } }

        private string _contactEmail = null;

        // account #s
        [MaxLength(64)]
        public string accountReference { get { return _accountReference; } set { SetProperty(ref _accountReference, value, nameof(accountReference)); } }

        private string _accountReference = null;

        #endregion contact/account details
    }
}