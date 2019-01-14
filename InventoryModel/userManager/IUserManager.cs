// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Security;

using TEMS.InventoryModel.entity.db.user;

namespace TEMS.InventoryModel.userManager
{
    public interface IUserManager
    {
        /// <summary>
        /// Given a userId and passphrase, validates proper passphrase was provided
        /// If valid then returns true and updates user with UserDetails
        /// Otherwise returns false and sets user to null
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passphrase"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        bool ValidateUser(string userId, SecureString passphrase, out UserDetail user);

        /// <summary>
        /// returns a newly created UserDetail
        /// Note: user is not persisted until SaveUser(user) is called
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hashedPassphrase"></param>
        /// <returns></returns>
        UserDetail CreateUser(string userId, string hashedPassphrase);

        /// <summary>
        /// returns a newly created UserDetail initialized from another user
        /// Note: user is not persisted until SaveUser(user) is called
        /// </summary>
        /// <param name="fromUser"></param>
        /// <returns></returns>
        UserDetail CloneUser(string userId, UserDetail fromUser);

        /// <summary>
        /// Persists user, if new user then adds, if existing user then updates
        /// </summary>
        /// <param name="user"></param>
        void SaveUser(UserDetail user);

        /// <summary>
        /// Removes user, user will no longer exist
        /// Note: user may be readded via SaveUser(user) call if user object still available
        /// </summary>
        /// <param name="user"></param>
        void RemoveUser(UserDetail user);

        #region log user activities

        /// <summary>
        /// Record (log) specified activity in UserActivity table of DB.
        /// Used for debugging and auditing purposes.
        /// </summary>
        /// <param name="action">What action was done</param>
        /// <param name="details">Optional, additional information about the activity</param>
        void LogUserAction(UserDetail user, UserActivity.UserAction action, string details = null);

        /// <summary>
        /// Log out user - sets user to null and records activity in UserActivity table
        /// Ok to call if no user currently logged in (user == null), does nothing.
        /// </summary>
        /// <param name="details">Additional notes about logout, e.g. Program exit ...</param>
        void LogoutUser(string details);

        /// <summary>
        /// Consider user logged in until program exits or they logout.
        /// store UserDetail for rest of application to use
        /// </summary>
        /// <param name="user"></param>
        void LoginUser(UserDetail user);

        /// <summary>
        /// Returns the user from the most recent LoginUser() call
        /// assuming no LogoutUser since then, null otherwise
        /// </summary>
        /// <returns></returns>
        UserDetail CurrentUser();

        #endregion log user activities
    }
}