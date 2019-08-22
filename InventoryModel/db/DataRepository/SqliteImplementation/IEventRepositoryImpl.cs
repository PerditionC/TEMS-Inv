// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using TEMS.InventoryModel.entity.db.query;

namespace TEMS.InventoryModel.entity.db
{
    public static class DataRepositoryExtensionMethods
    {
        /// <summary>
        /// Extension method to convert a Collection into a comma separated list, e.g. to 
        /// convert a List of ItemInstances into a comma separated string of primary keys for
        /// SQL query ... $".id IN ({Items.Select(item => item.PrimaryKey).JoinString();})" ...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static string JoinString<T>(this IEnumerable<T> source, string seperator = ", ")
        {
            return string.Join(seperator, source);
        }

        /// <summary>
        /// Returns a comma separated list of T items, using T.ToString() to get T's value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string PrimaryKeysToCommaSeparatedList<T>(this IEnumerable<T> items)
        {
            return items.Select(item => "'" + item.ToString() + "'").JoinString();
        }

        /// <summary>
        /// Returns a comma separated list of primary keys for a collection of ItemBase items
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string PrimaryKeysToCommaSeparatedList(this IEnumerable<ItemBase> items)
        {
            return items.Select(item => "'" + item.PrimaryKey.ToString() + "'").JoinString();
        }

        /// <summary>
        /// Returns a comma separated list of primary keys for a collection of SearchResult items,
        /// ignores SearchResults without a primary key (e.g. a GroupHeader)
        /// Assumes PK are represented as strings, as such returns them quoted 'pk1 value','pk2 value', ...
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string PrimaryKeysToCommaSeparatedList(this IEnumerable<SearchResult> items)
        {
            return items.Where(item => item.id != Guid.Empty).Select(item => "'" + item.id.ToString() + "'").JoinString();
        }
    }

    /// <summary>
    /// Interface to database entities - all queries, updates, etc. handled via an instance of this class.
    /// Uses Database class to perform actual interaction with back-end database.
    /// </summary>
    public sealed partial class DataRepository : IEventRepository
    {
        #region deployment

        public IList<DeployEvent> GetDeploymentEventsFor(Guid itemInstanceId)
        {
            var queryParamsList = new List<string>
            {
                itemInstanceId.ToString()
            };
            var x = db.LoadRows<DeployEvent>("WHERE itemInstanceId=? ORDER BY DeployDate DESC, recoverDate DESC, DeployBy ASC;", queryParamsList.ToArray());
            return x;
        }

        /// <summary>
        /// Always returned the most recent deployment event (recoverDate should be checked to determine if still deployed or has been restored)
        /// </summary>
        /// <param name="itemInstanceId"></param>
        /// <returns></returns>
        public DeployEvent GetLatestDeploymentEventFor(Guid itemInstanceId)
        {
            return db.QueryAsync<DeployEvent>("SELECT DeployEvent.* FROM DeployEvent WHERE itemInstanceId=? ORDER BY DeployDate DESC, recoverDate DESC LIMIT 1;", itemInstanceId.ToString()).Result.FirstOrDefault();
        }

        /// <summary>
        /// return a list of all deployment events based on provided search criteria
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IList<DeployEvent> GetDeploymentEvents(SearchResult item)
        {
            var pkList = new List<string>() { DatabaseHelper.GetPrimaryKey(item) };
            if (item.childCount > 0) item.children.GetValues(DatabaseHelper.GetPrimaryKey, ref pkList);

            return db.LoadRows<DeployEvent>($"WHERE itemInstanceId IN ({pkList.PrimaryKeysToCommaSeparatedList()})");
        }

        #endregion deployment

        #region damageAndMissing

        /// <summary>
        /// return a list of all damage or missing events based on provided search criteria
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IList<DamageMissingEvent> GetDamageMissingEvents(SearchResult item)
        {
            var pkList = new List<string>() { DatabaseHelper.GetPrimaryKey(item) };
            if (item.childCount > 0) item.children.GetValues(DatabaseHelper.GetPrimaryKey, ref pkList);

            return db.LoadRows<DamageMissingEvent>($"WHERE itemInstanceId IN ({pkList.PrimaryKeysToCommaSeparatedList()})");
        }

        #endregion damageAndMissing

        #region service

        /// <summary>
        /// return a list of all service events based on provided search criteria
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public IList<ItemServiceHistory> GetItemServiceEvents(SearchResult item)
        {
            var pkList = new List<string>() { DatabaseHelper.GetPrimaryKey(item) };
            if (item.childCount > 0) item.children.GetValues(DatabaseHelper.GetPrimaryKey, ref pkList);

            return db.LoadRows<ItemServiceHistory>($"WHERE serviceId IN (SELECT id FROM ItemService WHERE itemInstanceId IN ({pkList.PrimaryKeysToCommaSeparatedList()}))");
        }

        #endregion service
    }
}