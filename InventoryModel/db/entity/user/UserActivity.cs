// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using SQLite;

using TEMS.InventoryModel.util;

namespace TEMS.InventoryModel.entity.db.user
{
    public class UserActivity : NotifyPropertyChanged
    {
        public UserActivity()
        {
            _id = Guid.NewGuid();
            _when = System.DateTime.Now;
        }

        public UserActivity(UserDetail user, UserAction action, string details = null) : this()
        {
            _userId = user?.userId;  // this should be a valid userid or NULL (FK constraint)
            _action = action;
            _details = details;
        }

        // internal DB primary key, unique per record
        // Note: GUID used for replication purposes
        [PrimaryKey]
        public Guid id { get { return _id; } set { SetProperty(ref _id, value, nameof(id)); RaisePropertyChanged(nameof(id)); } }

        private Guid _id;

        // when it was performed
        [NotNull]
        public DateTime when { get { return _when; } set { SetProperty(ref _when, value, nameof(when)); } }

        private DateTime _when;

        // who performed action, UserDetail.userId
        [MaxLength(32), NotNull]
        public string userId { get { return _userId; } set { SetProperty(ref _userId, value, nameof(userId)); } }

        private string _userId;

        // what [generically] was done (explicit values in case updated via other means, not a FLAG)
        public enum UserAction { Unknown = 0, Login = 1, Logout = 2, PasswordChange = 4, Update = 8, Delete = 16, Other = 32 }

        [NotNull]
        public UserAction action { get { return _action; } set { SetProperty(ref _action, value, nameof(action)); } }

        private UserAction _action = UserAction.Unknown;

        // notes or additional details about action
        [MaxLength(1024)]
        public string details { get { return _details; } set { SetProperty(ref _details, value, nameof(details)); } }

        private string _details;
    }
}