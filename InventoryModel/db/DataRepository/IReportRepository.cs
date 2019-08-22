// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS.InventoryModel.entity.db
{
    public enum WhichReport
    {
        InventorySummary,
        ItemStatus,
        VendorCost,
        Weight,
        Expiration,
        Service,
        Deployment,
        DamagedOrMissing,
        MainBoxContents
    }

    /// <summary>
    /// Interface to database entities - all queries, updates, etc. handled via an instance of this class.
    /// Uses Database class to perform actual interaction with back-end database.
    /// </summary>
    public interface IReportRepository
    {
        /// <summary>
        /// returns data for specified report
        /// </summary>
        /// <param name="whichReport"></param>
        /// <param name="siteLocationId"></param>
        /// <param name="equipment"></param>
        /// <returns></returns>
        System.Collections.IEnumerable getReportItems(WhichReport whichReport, string siteLocationId, string equipment);
    }

    // base class for report items to display
    public class ReportItemsView { }

    public class ReportItemSummaryView : ReportItemsView
    {
        [SQLite.MaxLength(128), SQLite.NotNull]
        public string description { get; set; }

        [SQLite.NotNull]
        public int quantity { get; set; }

        [SQLite.MaxLength(16)]
        public string location { get; set; }

        [SQLite.MaxLength(128)]
        public string binOrModule { get; set; }

        public DateTime? expirationDate { get; set; }

        public string status { get; set; }
        public string itemNumber { get; set; }
        public double weight { get; set; }
        public string col1 { get; set; }
        public string col2 { get; set; }
    }
}