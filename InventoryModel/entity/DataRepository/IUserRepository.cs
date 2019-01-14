// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;

using TEMS.InventoryModel.entity.db.user;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Interface to database entities - all queries, updates, etc. handled via an instance of this class.
    /// Uses Database class to perform actual interaction with back-end database.
    /// </summary>
    public interface IUserRepository
    {
        #region user details

        /// <summary>
        /// removes user from known users - deletes user account
        /// WARNING: this will also remove any information linked to this
        /// including user activities, damaged/missing events, etc.
        /// </summary>
        /// <param name="user"></param>
        void DeleteUser(UserDetail user);

        /// <summary>
        /// return a list of all user activities based on provided search criteria
        /// </summary>
        /// <param name="searchFilter"></param>
        /// <returns></returns>
        IList<UserActivity> GetUserActivities(string userId);

        /// <summary>
        /// return a list of all users based on provided search query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        IList<UserDetail> GetUsers(string query, params object[] args);

        #endregion user details

        #region sites

        /// <summary>
        /// returns the suffix to use for given site name
        /// </summary>
        /// <param name="siteName">site to lookup suffix for</param>
        /// <returns>the suffix to use for item instances at this location</returns>
        string GetSiteSuffix(string siteName);

        #endregion sites
    }
}