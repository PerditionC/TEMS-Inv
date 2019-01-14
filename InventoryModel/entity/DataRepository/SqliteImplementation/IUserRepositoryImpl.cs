// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TEMS.InventoryModel.entity.db.user;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Interface to database entities - all queries, updates, etc. handled via an instance of this class.
    /// Uses Database class to perform actual interaction with back-end database.
    /// </summary>
    public sealed partial class DataRepository : IUserRepository
    {
        #region user details

        /// <summary>
        /// removes a user and all associated records
        /// Warning! use with caution, will have cascaded effects!
        /// </summary>
        /// <param name="user"></param>
        public void DeleteUser(UserDetail user)
        {
            logger.Trace("DeleteUser");
            try
            {
                // must delete all related rows first to avoid fk constraint violation
                db.DeleteAll("UserActivity", "userId=?", user.userId);
                db.DeleteAll("DeployEvent", "deployBy=? OR recoverBy=?", new object[] { user.userId, user.userId });
                db.DeleteAll("DamageMissingEvent", "inputBy=?", user.userId);
                db.Delete(user);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error occurred when removing user - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// return a list of all user activities based on provided search criteria
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <returns></returns>
        public IList<UserActivity> GetUserActivities(string userId)
        {
            try
            {
                return db.LoadRows<UserActivity>("WHERE userId=?;", userId);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to load activities for user {userId} - {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// return a list of all users based on provided search query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public IList<UserDetail> GetUsers(string query, params object[] args)
        {
            try
            {
                return db.LoadRows<UserDetail>(query, args);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to load users for query [{query ?? "null"}] - {e.Message}");
                throw;
            }
        }

        #endregion user details

        #region sites

        /// <summary>
        /// returns the suffix to use for given site name
        /// </summary>
        /// <param name="siteName">site to lookup suffix for</param>
        /// <returns>the suffix to use for item instances at this location</returns>
        public string GetSiteSuffix(string siteName)
        {
            try
            {
                return db.ExecuteScalar<string>("SELECT locSuffix FROM SiteLocation WHERE name=?;", siteName);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Failed to get location suffix for site {siteName} - {e.Message}");
                throw;
            }
        }

        #endregion sites
    }
}