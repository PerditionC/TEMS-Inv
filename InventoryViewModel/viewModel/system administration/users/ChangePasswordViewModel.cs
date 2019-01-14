// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Security;
#if NET40
using System.Windows.Input;  // ICommand in .Net4.0 is in PresentationCore.dll, while in .Net4.5+ it moved to System.dll
#endif
using NLog;
using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.user;
using TEMS.InventoryModel.userManager;
using TEMS.InventoryModel.userManager.extension;

namespace TEMS_Inventory.views
{
    public class ChangePasswordViewModel : ViewModelBase
    {
        /// <summary>
        /// Initialize view and set user object we are changing password hash for
        /// </summary>
        /// <param name="user">user to set new password for</param>
        public ChangePasswordViewModel(UserDetail user) : base()
        {
            this.user = user;
        }

        // Note: this is passed in during construction, we DO NOT use global user
        // as that would changed the logged in user always, not selected user in case
        // admin is changing password for another user
        private UserDetail user;


        /// <summary>
        /// Command to add an item
        /// </summary>
        public ICommand SetPasswordCommand
        {
            get { return InitializeCommand(ref _SetPasswordCommand, param => this.SetPassword(param), null); }
        }
        private ICommand _SetPasswordCommand;


        /// <summary>
        /// If admin or for current user then change password hash to match provided password
        /// Password change is logged.  
        /// </summary>
        /// <param name="param"></param>
        private void SetPassword(object param)
        {
            // validate password provided
            var password = param as SecureString;
            if (password == null)
            {
                logger.Warn("Attempt to set password to null or using object other than SecureString.");
                return;
            }

            // validate user and log attempt
            if (user != null)
            {
                var userManager = UserManager.GetUserManager;
                var currentUser = userManager.CurrentUser();
                if (currentUser != user)
                {
                    if (currentUser.isAdmin)
                    {
                        userManager.LogUserAction(currentUser, UserActivity.UserAction.PasswordChange, $"Set new password for user '{user.userId}'.");
                    }
                    else
                    {
                        logger.Warn($"NonAdmin user {currentUser.userId} attempted to set password for user {user.userId}!");
                        userManager.LogUserAction(currentUser, UserActivity.UserAction.PasswordChange, $"NonAdmin user - failed attempt to set password for user {user.userId}!");
                        return;
                    }
                }
                else
                {
                    userManager.LogUserAction(userManager.CurrentUser(), UserActivity.UserAction.PasswordChange, $"Set new password for self '{user.userId}'.");
                }

                // update password hash via new passphrase (and salt)
                user.SetPasswordHash(password, mustChange: false);
                // save the [possibly] updated hash
                DataRepository.GetDataRepository.Save(user);
            }
        }
    }
}
