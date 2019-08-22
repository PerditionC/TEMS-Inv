// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;

using SQLite;

using StatePrinting.FieldHarvesters;

using TEMS.InventoryModel.util.attribute;

namespace TEMS.InventoryModel.entity.db.user
{
    public class UserDetail : ItemBase
    {
        public static readonly string USER = "User";
        public static readonly string ADMIN = "Admin";

        /// <summary>
        /// Construct and initialize with defaults
        /// no associated user, not active with expired null password
        /// and default to no associated sites
        /// </summary>
        public UserDetail() : this(userId: null, hashedPassphrase: null, isActive: false, isPasswordExpired: true, role: USER, currentSite: null, availableSites: new List<SiteLocation>()) { }

        /// <summary>
        /// Construct and initialize, e.g. when loading from DB
        /// name and email are optional and may be omitted
        /// </summary>
        public UserDetail(string userId, string hashedPassphrase, bool isActive, bool isPasswordExpired, string role, SiteLocation currentSite, List<SiteLocation> availableSites, string lastName = null, string firstName = null, string email = null)
        {
            _userId = userId;
            _hashedPassphrase = hashedPassphrase;
            _isActive = isActive;
            _isPasswordExpired = isPasswordExpired;
            _role = role;
            _lastName = lastName;
            _firstName = firstName;
            _email = email;
            _currentSite = currentSite;
            this.availableSites = availableSites;  // force IsChanged==true, note this is only field that should never be set to null thus default value of null!=initially set value and IsChanged is set to true

            // configure toString to not include password hash
            printer.Configuration.AddHandler(
                t => t == typeof(UserDetail),
                t => new List<SanitizedFieldInfo>((new HarvestHelper()).GetFieldsAndProperties(typeof(UserDetail)).Where(x => !x.SanitizedName.Contains("Passphrase")))
            );
        }

        // user login credentials
        [SQLite.PrimaryKey, SQLite.MaxLength(32)]
        public string userId
        {
            get { return _userId; }
            set { SetProperty(ref _userId, value, nameof(userId)); }
        }

        private string _userId;

        [SQLite.MaxLength(128)]
        public string hashedPassphrase { get { return _hashedPassphrase; } set { SetProperty(ref _hashedPassphrase, value, nameof(hashedPassphrase)); } }

        private string _hashedPassphrase;

        public bool isActive { get { return _isActive; } set { SetProperty(ref _isActive, value, nameof(isActive)); } }  // user has account but may be disabled/active
        private bool _isActive;

        public bool isPasswordExpired { get { return _isPasswordExpired; } set { SetProperty(ref _isPasswordExpired, value, nameof(isPasswordExpired)); } } // can still use current pw to login but need to change it
        private bool _isPasswordExpired;

        // user role [Admin or User]
        [SQLite.MaxLength(32), SQLite.NotNull]
        public string role { get { return _role; } set { SetProperty(ref _role, value, nameof(role)); RaisePropertyChanged("isAdmin"); } }

        private string _role;

        [SQLite.Ignore]
        public bool isAdmin
        {
            get { return ADMIN.Equals(role); /* else "User" */ }
            set { if (value) role = ADMIN; else role = USER; }
        }

        // default/current site location (jurisdiction) for user
        [ManyToOne(nameof(siteId))] // in DB as foreign key for SiteLocation table
        public SiteLocation currentSite
        {
            get { return _currentSite; }
            set
            {
                SetProperty(ref _currentSite, value, nameof(currentSite));
                siteId = _currentSite?.id ?? Guid.Empty;
            }
        }

        private SiteLocation _currentSite;

        [NotNull, ForeignKey(nameof(currentSite))]
        public Guid siteId { get { return _siteId; } set { SetProperty(ref _siteId, value, nameof(siteId)); } }

        private Guid _siteId = Guid.Empty;

        // all sites locations user can access data for
        [ManyToMany(typeof(UserSiteMapping))]
        public List<SiteLocation> availableSites { get { return _availableSites; } set { SetProperty(ref _availableSites, value, nameof(availableSites)); } }

        private List<SiteLocation> _availableSites;

        // administrative information, may be blank
        [SQLite.MaxLength(128)]
        public string lastName { get { return _lastName; } set { SetProperty(ref _lastName, value, nameof(lastName)); RaisePropertyChanged("fullName"); } }

        private string _lastName;

        [SQLite.MaxLength(128)]
        public string firstName { get { return _firstName; } set { SetProperty(ref _firstName, value, nameof(firstName)); RaisePropertyChanged("fullName"); } }

        private string _firstName;

        [SQLite.MaxLength(256)]
        public string email { get { return _email; } set { SetProperty(ref _email, value, nameof(email)); } }

        private string _email;

        // returns full name of user, or if missing then userId
        [SQLite.Ignore]
        public string fullName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    if (!string.IsNullOrWhiteSpace(firstName))
                        return firstName + " " + lastName;
                    else
                        return lastName;
                }
                else
                {
                    return userId;
                }
            }
        }

        // returns true if all Non Null constraints satisfied
        public override bool AreNonNullConstraintsSatisfied()
        {
            // make sure all the default constraint conditions are met first
            if (!base.AreNonNullConstraintsSatisfied()) return false;

            // additionally ensure at least 1 location is set and default site is valid
            return (siteId != Guid.Empty);
        }
    }
}