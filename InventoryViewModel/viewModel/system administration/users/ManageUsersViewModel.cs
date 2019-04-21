// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.user;
using TEMS.InventoryModel.util;

namespace TEMS_Inventory.views
{
    public class Site : NotifyPropertyChanged
    {
        public Guid pk { get { return _pk; } set { SetProperty(ref _pk, value, nameof(pk)); } }
        public Guid _pk;
        public string name { get { return _name; } set { SetProperty(ref _name, value, nameof(name)); } }
        public string _name;
        public string location { get { return _location; } set { SetProperty(ref _location, value, nameof(location)); } }
        public string _location;

        public bool IsSelected { get { return _IsSelected; } set { SetProperty(ref _IsSelected, value, nameof(IsSelected)); } }
        public bool _IsSelected;

        public override string ToString()
        {
            return $"{name} ({location})";
        }
    }


    public class ManageUsersViewModel : ItemListToAddEditDeleteViewModel
    {
        // anything that needs initializing for MSVC designer
        public ManageUsersViewModel(Func<ItemBase> GetNewItem) : base(GetNewItem)
        {
            // listen for property changes to ourself
            this.PropertyChanged += new PropertyChangedEventHandler(ManageUsersViewModel_PropertyChanged);

            // load initial/all the users
            DoSearchUsersCommand();
        }


        private void ManageUsersViewModel_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(selectedItem):
                    //if (EditCommand.CanExecute(null)) EditCommand.Execute(null);  // this is done automatically by parent class
                    break;
                case nameof(currentItem):
                    //RaisePropertyChanged("isDetailViewInActive");
                    RaisePropertyChanged(nameof(userStatus));
                    RaisePropertyChanged(nameof(passwordStatus));
                    RaisePropertyChanged(nameof(SuspendUserCommandText));
                    updateCurrentUserSites();
                    // on currentUser change update users for vendor
                    UpdateUserLogList();
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// override default implementation to ensure related rows also removed first
        /// </summary>
        /// <param name="item"></param>
        protected override void DoDeleteItem(ItemBase item)
        {
            var db = DataRepository.GetDataRepository;
            if (db != null) db.DeleteUser(selectedItem as UserDetail);
        }


        #region user logged actions

        public ObservableCollection<UserActivity> userActivities
        {
            get { return _userActivities; }
            set { SetProperty(ref _userActivities, value, nameof(userActivities)); }
        }
        private ObservableCollection<UserActivity> _userActivities = new ObservableCollection<UserActivity>();

        private void UpdateUserLogList()
        {
            var currentUser = currentItem as UserDetail;
            if (currentUser?.userId != null)
            {
                userActivities = new ObservableCollection<UserActivity>(DataRepository.GetDataRepository.GetUserActivities(currentUser.userId));
            }
            else
            {
                userActivities = new ObservableCollection<UserActivity>(); // empty list
            }
        }

        #endregion // user logged actions

        public List<string> Roles
        {
            get
            {
                if (_Roles == null)
                {
                    _Roles = new List<string>(2)
                    {
                        UserDetail.USER,
                        UserDetail.ADMIN
                    };
                }

                return _Roles;
            }
        }
        private List<string> _Roles = null;


        public ObservableCollection<Site> Sites
        {
            get
            {
                if (_Sites == null)
                {
                    try
                    {
                        //Sites = new List<SiteLocation>(((IEnumerable)(DataRepository.GetDataRepository.ReferenceData[nameof(SiteLocation)])).Cast<SiteLocation>());
                        //_Sites = DataRepository.GetDataRepository.GetAllSites();
                        _Sites = new ObservableCollection<Site>();
                        foreach (var element in DataRepository.GetDataRepository.ReferenceData[nameof(SiteLocation)])
                        {
                            var site = element as SiteLocation;
                            _Sites.Add(new Site() {
                                pk = site.id,
                                name = site.name,
                                location = site.locSuffix,
                                IsSelected = false
                            });
                        }
                        logger.Debug("Sites has " + Sites?.Count + " elements.");
                    }
                    catch (Exception e)
                    {
                        logger.Error(e, "Failed to obtain list of all sites!");
                    }
                }

                return _Sites;
            }
            set { SetProperty(ref _Sites, value, nameof(Sites)); }
        }
        private ObservableCollection<Site> _Sites = null;


        // sets Sites' IsSelected to match sites available for current user
        private void updateCurrentUserSites()
        {
            if (Sites == null) return;

            if (currentItem is UserDetail currentUser)
            {
                foreach (var site in Sites)
                {
                    site.IsSelected = currentUser.availableSites.Exists(x => string.Equals(x.id, site.pk));
                }
            }
            else
            {
                foreach (var site in Sites)
                {
                    site.IsSelected = false;
                }
            }

            RaisePropertyChanged(nameof(Sites));
        }


        public ICommand SelectedSitesChanged
        {
            get { return InitializeCommand(ref _SelectedSitesChanged, param => setAvailableSitesFromSelectedSites(), null); }
        }
        private ICommand _SelectedSitesChanged;

        // sets current user's sites available to match Sites' IsSelected sites
        private void setAvailableSitesFromSelectedSites()
        {
            if (Sites == null) return;

            if (currentItem is UserDetail currentUser)
            {
                currentUser.availableSites.Clear();
                foreach (var site in Sites)
                {
                    // for each selected site, load full site information from DB
                    if (site.IsSelected) currentUser.availableSites.Add(DataRepository.GetDataRepository.ReferenceData[nameof(SiteLocation)].ById<SiteLocation>(site.pk));
                }
                // force is changed as we have updated available sites, but since changes to list values and not list itself not automatically done
                currentUser.IndicateChanges();

                // update default user's site in case no longer available
                if (!currentUser.availableSites.Contains(currentUser.currentSite))
                    currentUser.currentSite = currentUser.availableSites.FirstOrDefault();
                RaisePropertyChanged(nameof(Sites));
                RaisePropertyChanged(nameof(currentItem));
            }
        }

        public string userStatus => currentItem is UserDetail currentUser ? (currentUser.isActive ? "active" : "disabled") : "?";

        public string passwordStatus => (currentItem is UserDetail currentUser) && currentUser.isPasswordExpired
                    ? "Password is expired!  User must reset on next successful login."
                    : "";


        #region Search for User

        public string SearchUsersText
        {
            get { return _SearchUsersText; }
            set { SetProperty(ref _SearchUsersText, value, nameof(SearchUsersText)); }
        }
        private string _SearchUsersText;

        /// <summary>
        /// Command to open edit item window with this item selected so can be modified/viewed
        /// </summary>
        public ICommand SearchUsersCommand
        {
            get { return InitializeCommand(ref _SearchUsersCommand, param => DoSearchUsersCommand(), null); }
        }
        private ICommand _SearchUsersCommand;

        private void DoSearchUsersCommand()
        {
            logger.Debug("Loading users - DoSearch:\n" + SearchUsersText);

            var query = "WHERE (userId LIKE ?) OR (firstName LIKE ?) OR (lastName LIKE ?) or (email LIKE ?)";

            var queryParams = new List<string>(3);
            var searchText = (string.IsNullOrWhiteSpace(SearchUsersText)) ? "%" : "%" + SearchUsersText + "%";
            // add for each different field we are searching
            queryParams.Add(searchText); queryParams.Add(searchText); queryParams.Add(searchText); queryParams.Add(searchText);
            items = new ObservableCollection<ItemBase>(DataRepository.GetDataRepository.GetUsers(query, queryParams.ToArray()).Cast<ItemBase>());
            // autoselect if only 1 item type returned
            if (items.Count == 1) selectedItem = items[0];
        }

        #endregion // Search for User


        /// <summary>
        /// Returns text indicating whether command will suspend or restore user account
        /// </summary>
        public string SuspendUserCommandText
        {
            get
            {
                // we only show Restore option if valid user and they are suspended, otherwise just show Suspend option (even if not a valid user)
                return (currentItem is UserDetail currentUser) && (!currentUser.isActive) ? "_Restore User" : "_Suspend User";
            }
        }

        /// <summary>
        /// Command to suspend/activate a user
        /// </summary>
        public ICommand SuspendUserCommand
        {
            get { return InitializeCommand(ref _SuspendUserCommand, param => DoSuspendUser(), param => CanSuspendUser()); }
        }
        private ICommand _SuspendUserCommand;

        private bool CanSuspendUser()
        {
            return (currentItem != null); // && currentUser.isActive;
        }

        /// <summary>
        /// toggles user suspension/active state
        /// </summary>
        private void DoSuspendUser()
        {
            if (currentItem is UserDetail currentUser)
            {
                currentUser.isActive = !currentUser.isActive;
                RaisePropertyChanged(nameof(userStatus));
                RaisePropertyChanged(nameof(SuspendUserCommandText));
            }
        }


        /// <summary>
        /// Command to force expire a user's password
        /// </summary>
        public ICommand ExpirePasswordCommand
        {
            get { return InitializeCommand(ref _ExpirePasswordCommand, param => DoExpirePassword(), param => CanExpirePassword()); }
        }
        private ICommand _ExpirePasswordCommand;

        private bool CanExpirePassword()
        {
            return (currentItem is UserDetail currentUser) && !currentUser.isPasswordExpired;
        }

        /// <summary>
        /// forces a user's password into expired state - mandatory change on next login
        /// </summary>
        private void DoExpirePassword()
        {
            if (currentItem is UserDetail currentUser)
            {
                currentUser.isPasswordExpired = true;
                RaisePropertyChanged(nameof(passwordStatus));
            }
        }

        /// <summary>
        /// Command to initialize or set (change) user's password
        /// </summary>
        public ICommand SetPasswordCommand
        {
            get { return InitializeCommand(ref _SetPasswordCommand, param => DoSetPassword(), param => currentItem != null); }
        }
        private ICommand _SetPasswordCommand;

        /// <summary>
        /// show a dialog where user can set new password value and reset expired password flag
        /// </summary>
        private void DoSetPassword()
        {
            if (currentItem is UserDetail currentUser)
            {
                var viewModel = new ChangePasswordViewModel(currentUser);
                ShowChildWindow(new ShowWindowMessage { modal = true, childWindow = true, viewModel = viewModel });
                RaisePropertyChanged(nameof(passwordStatus));
            }
        }

    }
}
