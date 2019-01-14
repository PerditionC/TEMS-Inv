// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Security;

using NLog;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.user;

namespace TEMS.InventoryModel.userManager
{
    /// <summary>
    /// Provides routines to create and modify user details including persisting to DB.
    /// </summary>
    public sealed class UserManager : IUserManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private DataRepository db;

        public UserManager(DataRepository dataRepository)
        {
            logger = LogManager.GetCurrentClassLogger();
            logger.Trace(nameof(UserManager));

            db = dataRepository;

            // store as Singleton so we can access as needed by various ICommands
            GetUserManager = this;
        }

        /// <summary>
        /// Returns the current IUserManager which maintains users.
        /// Note this will return null until the UserManager is created
        /// by the application - WARNING only 1 UserManager should be
        /// created per application instance (last one instantiated is returned).
        /// </summary>
        public static IUserManager GetUserManager { get; private set; } = null;

        /// <summary>
        /// maintains most recent logged in user
        /// !only supports a single user logged in at a time!
        /// </summary>
        private UserDetail currentUser = null;

        /// <summary>
        /// Returns details for requested userId.
        /// </summary>
        /// <param name="userId">The user name (userId) of user to query about</param>
        /// <returns>UserDetail for requested user; null on errors</returns>
        private UserDetail GetUserDetailsFor(string userId)
        {
            logger.Trace(nameof(GetUserDetailsFor));
            try
            {
                // get user details from DB, if error (not found, etc) then return null immediately
                var user = db.Load<UserDetail>(userId.Trim().ToLowerInvariant());
                if (string.IsNullOrWhiteSpace(user?.userId)) return null;

                logger.Info("User information loaded from DB.");
                logger.Debug($"user: {user}");
                return user;
            }
            catch (Exception e)
            {
                logger.Warn(e, $"Exception getting user details for - {userId} - {e.Message}");
                return null; // don't re-throw, just fail getting details
            }
        }

        /// <summary>
        /// Retrieve user information after validating provided userId and passphrase.
        /// If valid then returns true and updates user with UserDetails
        /// Otherwise returns false and sets user to null
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passphrase"></param>
        /// <param name="user">creates new instance and returns validated user information</param>
        /// <returns>true if user input valid and user object container user information; false on any error validating</returns>
        public bool ValidateUser(string userId, SecureString passphrase, out UserDetail user)
        {
            logger.Trace(nameof(ValidateUser));
            // ensure we initialize user to null unless a valid userId and passphrase is provided
            user = null;

            // if not a valid value for DB key then fail early
            if (string.IsNullOrWhiteSpace(userId)) return false;

            try
            {
                // retrieve corresponding user information from DB
                // Note: userId [UserDetail DB table key] is stored in lower case (using CultureInvariant form)
                var possibleUser = GetUserDetailsFor(userId);
                // invalid userId or error loading from DB - failed to validate; Note early exit allows determination of usernames
                if (possibleUser == null)
                {
                    logger.Warn($"Failed to obtain possible user details for {userId}.");
                    return false;
                }

                // get attempted password, exit early if obviously invalid passphrase (blank)
                // if (string.IsNullOrWhiteSpace(passphrase.ToString())) return false; -- passphrase is now a SecureString, don't defeat point and convert to string
                if ((passphrase == null) || (passphrase.Length < 1))
                {
                    logger.Warn($"Passphrase provided for {userId} is invalid.");
                    return false;
                }

                // split apart stored hash so we can hash attempted password & compare
                bool isValid;
                try
                {
                    byte[] storedHash = Convert.FromBase64String(possibleUser.hashedPassphrase);
                    byte[] salt = PasswordHashing.ExtractStoredSalt(storedHash);
                    // get hashed version of attempted password
                    byte[] curHash = Convert.FromBase64String(PasswordHashing.EncodePassword(passphrase, salt));

                    // determine if hashed passphrases are a match
                    isValid = PasswordHashing.HashesMatch(storedHash, curHash);
                }
                catch (System.FormatException e) // invalid Base64 string
                {
                    logger.Warn("Error validating credentials, likely stored passphrase corrupted / not base64 encoded!", e);
                    isValid = false;
                }
                // validated supplied password, now confirm user account not currently disabled
                isValid = isValid && possibleUser.isActive;

                // only return any information if user credentials successfully validated
                if (isValid)
                {
                    logger.Info("Valid user.");
                    user = possibleUser;
                }
                else
                {
                    logger.Warn($"Failed to validate credentials for user {userId}.");
                }

                return isValid;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error occured when validating user - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// returns a newly created UserDetail
        /// Note: user is not persisted until SaveUser(user) is called
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hashedPassphrase"></param>
        /// <returns></returns>
        public UserDetail CreateUser(string userId, string hashedPassphrase)
        {
            logger.Trace(nameof(CreateUser));
            try
            {
                var user = new UserDetail
                {
                    userId = userId?.Trim()?.ToLowerInvariant(),
                    hashedPassphrase = hashedPassphrase
                };
                return user;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error occured when creating new user - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// returns a newly created UserDetail initialized from another user
        /// The new user still must set initial passphrase and be activated; also name & email are not copied
        /// Note: user is not persisted until SaveUser(user) is called
        /// </summary>
        /// <param name="fromUser"></param>
        /// <returns></returns>
        public UserDetail CloneUser(string userId, UserDetail fromUser)
        {
            logger.Trace(nameof(CloneUser));
            try
            {
                var user = new UserDetail(userId, hashedPassphrase: null, isActive: false, isPasswordExpired: true, role: fromUser.role, currentSite: fromUser.currentSite, availableSites: new List<SiteLocation>(fromUser.availableSites));
                return user;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error occured when creating cloned user - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Persists user, if new user then adds, if existing user then updates
        /// </summary>
        /// <param name="user"></param>
        public void SaveUser(UserDetail user)
        {
            logger.Trace(nameof(SaveUser));
            try
            {
                // normalize userId
                user.userId = user.userId.Trim().ToLowerInvariant();
                // persist to backend db
                db.Save(user);
                LogUserAction(user, UserActivity.UserAction.Update);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error occurred when updating user - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Removes user, user will no longer exist
        /// Note: user may be re-added via SaveUser(user) call if user object still available
        /// </summary>
        /// <param name="user"></param>
        public void RemoveUser(UserDetail user)
        {
            logger.Trace(nameof(RemoveUser));
            try
            {
                db.DeleteUser(user);
                LogUserAction(user, UserActivity.UserAction.Delete);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error occured when removing user - {e.Message}");
                throw;
            }
        }

        #region log user activities

        /// <summary>
        /// Record (log) specified activity in UserActivity table of DB.
        /// Used for debugging and auditing purposes.
        /// </summary>
        /// <param name="action">What action was done</param>
        /// <param name="details">Optional, additional information about the activity</param>
        public void LogUserAction(UserDetail user, UserActivity.UserAction action, string details = null)
        {
            logger.Trace(nameof(LogUserAction));
            try
            {
                logger.Info($"User {user.userId}: action={action.ToString()} - \"{details}\"");
                var userActivity = new UserActivity(user, action, details);
                db.Save(userActivity);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to write {action.ToString()}:'{details}' user activity to database.");
            }
        }

        /// <summary>
        /// Log out user - sets user to null and records activity in UserActivity table
        /// Ok to call if no user currently logged in (user == null), does nothing.
        /// </summary>
        /// <param name="details">Additional notes about logout, e.g. Program exit ...</param>
        public void LogoutUser(string details)
        {
            logger.Trace(nameof(LogoutUser));
            if (currentUser != null)
            {
                try
                {
                    LogUserAction(currentUser, UserActivity.UserAction.Logout, details);
                }
                catch (Exception e)
                {
                    logger.Warn(e, "Logout not recorded.");
                }
                finally
                {
                    currentUser = null;
                }
            }
        }

        /// <summary>
        /// Consider user logged in until program exits or they logout.
        /// store UserDetail for rest of application to use
        /// </summary>
        /// <param name="user"></param>
        public void LoginUser(UserDetail user)
        {
            logger.Trace(nameof(LoginUser));
            try
            {
                LogUserAction(user, UserActivity.UserAction.Login, "Successfully logged in and validated.");
            }
            catch (Exception e)
            {
                logger.Warn(e, "Login not recorded.");
            }
            finally
            {
                currentUser = user;
            }
        }

        /// <summary>
        /// Returns the user from the most recent LoginUser() call
        /// assuming no LogoutUser since then, null otherwise
        /// </summary>
        /// <returns></returns>
        public UserDetail CurrentUser()
        {
            logger.Trace(nameof(CurrentUser) + " current user is " + (currentUser?.displayName ?? "none"));
            return currentUser;
        }

        #endregion log user activities
    }
}