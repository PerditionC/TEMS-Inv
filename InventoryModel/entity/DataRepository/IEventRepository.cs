// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TEMS.InventoryModel.entity.db.query;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Interface to database entities - all queries, updates, etc. handled via an instance of this class.
    /// Uses Database class to perform actual interaction with back-end database.
    /// </summary>
    public interface IEventRepository
    {
        #region deployment

        /// <summary>
        ///
        /// </summary>
        /// <param name="itemInstanceId"></param>
        /// <returns></returns>
        IList<DeployEvent> GetDeploymentEventsFor(Guid itemInstanceId);

        /// <summary>
        /// Always returned the most recent deployment event (recoverDate should be checked to determine if still deployed or has been restored)
        /// </summary>
        /// <param name="itemInstanceId"></param>
        /// <returns></returns>
        DeployEvent GetLatestDeploymentEventFor(Guid itemInstanceId);

        /// <summary>
        /// return a list of all deployment events based on provided search criteria
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        IList<DeployEvent> GetDeploymentEvents(SearchResult item);

        #endregion deployment

        #region damageAndMissing

        /// <summary>
        /// return a list of all damage or missing events based on provided search criteria
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        IList<DamageMissingEvent> GetDamageMissingEvents(SearchResult item);

        #endregion damageAndMissing

        #region service

        /// <summary>
        /// return a list of all service events based on provided search criteria
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        IList<ItemServiceHistory> GetItemServiceEvents(SearchResult item);

        #endregion service
    }
}