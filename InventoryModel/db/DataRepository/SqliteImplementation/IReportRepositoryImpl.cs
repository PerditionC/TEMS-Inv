// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Data;
using System.Linq;
using System.Reflection;

using TEMS.InventoryModel.util.attribute;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Interface to database entities - all queries, updates, etc. handled via an instance of this class.
    /// Uses Database class to perform actual interaction with back-end database.
    /// </summary>
    public sealed partial class DataRepository : IReportRepository  // report queries
    {
        /// <summary>
        /// Given a specific property, returns a pretty formated title (short description)
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static string GetColumnName(PropertyInfo propertyInfo)
        {
            // see if field annotated with FieldLabel("prettyName") attribute, otherwise use Type name
            return (propertyInfo.PrettyName() ?? propertyInfo.Name[0].ToString().ToUpperInvariant() + propertyInfo.Name.Substring(1)) + ":";
            // return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(propertyInfo.Name)+":";
        }

        /// <summary>
        /// Extension method that can convert any IEnumerable collection
        /// of type T to a DataTable where table Columns are named same as
        /// each public property of type T
        /// Optionally, properties with a FieldLable(PrettyName) attribute will use that instead of property name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(/*this*/ System.Collections.IEnumerable data, Type T)
        {
            // get all public properties
            PropertyInfo[] properties = T.GetProperties(BindingFlags.Public);

            // create the DataTable and setup Columns based on T's properties (supporting Nullable types as well)
            var dataTable = new DataTable();
            dataTable.Columns.AddRange(
              properties.Select(property => new DataColumn(property.PrettyName() ?? property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType)).ToArray()
            );

            // add each rows data, 1 row for each element in data collection
            foreach (object item in data)
            {
                object[] rowValues = new object[properties.Length];
                for (int i = 0; i < rowValues.Length; i++)
                {
                    rowValues[i] = properties[i].GetValue(item, null) ?? DBNull.Value;
                }
                dataTable.Rows.Add(rowValues);
            }

            return dataTable;
        }

        /// <summary>
        /// Extension method that can convert any IEnumerable collection
        /// of type T to a DataTable where table Columns are named same as
        /// each public property of type T
        /// Optionally, properties with a FieldLable(PrettyName) attribute will use that instead of property name
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(/*this*/ System.Collections.Generic.IEnumerable<T> data)
        {
            return ToDataTable(data, typeof(T));
        }

        public System.Collections.IEnumerable getReportItems(WhichReport whichReport, string siteLocationId, string equipment)
        {
            var query = string.Empty;
            switch (whichReport)
            {
                case WhichReport.InventorySummary:
                    query = "SELECT DISTINCT ItemType.name as description, (SELECT SUM(count) FROM Item AS I2 Where Item.id=I2.id) AS quantity, VehicleLocation.name AS location, IfNULL((SELECT IT2.name FROM Item AS I2 INNER JOIN ItemType AS IT2 ON I2.itemTypeId=IT2.id WHERE I2.id=Item.parentId), '') as binOrModule, expirationDate FROM (Item INNER JOIN ItemType ON Item.itemTypeId=ItemType.id) LEFT JOIN VehicleLocation ON Item.vehicleLocationId=VehicleLocation.id ORDER BY binOrModule, description;";
                    break;

                case WhichReport.ItemStatus:
                    query = "SELECT DISTINCT ItemStatus.name as status, ItemType.name as description, " +
                        "(SELECT SUM(count) FROM Item AS I2 Where Item.id = I2.id) AS quantity, " +
                        "IfNULL((SELECT IT2.name FROM Item AS I2 INNER JOIN ItemType AS IT2 ON I2.itemTypeId = IT2.id WHERE I2.id = Item.parentId), '') as binOrModule, " +
                        "ItemInstance.itemNumber " +
                        "FROM (ItemStatus INNER JOIN(ItemInstance INNER JOIN(Item INNER JOIN ItemType ON Item.itemTypeId = ItemType.id) ON ItemInstance.itemId = Item.id) ON ItemStatus.id = ItemInstance.statusId) " +
                        $"WHERE ItemInstance.siteLocationId = '{siteLocationId}' AND " +
                        $"Item.unitTypeName = '{equipment}' ORDER BY binOrModule, status, description;";
                    break;

                case WhichReport.VendorCost:
                    break;

                case WhichReport.Weight:
                    query =
                        "SELECT DISTINCT VehicleLocation.name AS location, ItemType.name as col1, NULL as col2, NULL as description, ItemType.weight " +
                            "FROM (VehicleLocation INNER JOIN(ItemInstance INNER JOIN(Item INNER JOIN ItemType ON Item.itemTypeId = ItemType.id) ON ItemInstance.itemId = Item.id) ON VehicleLocation.id = Item.vehicleLocationId) " +
                            $"WHERE ItemInstance.siteLocationId = '{siteLocationId}' AND Item.unitTypeName = '{equipment}' AND Item.parentId IS null AND (isBin=1) UNION " +
                        "SELECT DISTINCT VehicleLocation.name AS location, NULL as col1, ItemType.name as col2, NULL as description, ItemType.weight " +
                            "FROM (VehicleLocation INNER JOIN(ItemInstance INNER JOIN(Item INNER JOIN ItemType ON Item.itemTypeId = ItemType.id) ON ItemInstance.itemId = Item.id) ON VehicleLocation.id = Item.vehicleLocationId) " +
                            $"WHERE ItemInstance.siteLocationId = '{siteLocationId}' AND Item.unitTypeName = '{equipment}' AND Item.parentId IS null AND (isModule=1) UNION " +
                        "SELECT DISTINCT VehicleLocation.name AS location, NULL as col1, NULL as col2, ItemType.name as description, ItemType.weight " +
                            "FROM (VehicleLocation INNER JOIN(ItemInstance INNER JOIN(Item INNER JOIN ItemType ON Item.itemTypeId = ItemType.id) ON ItemInstance.itemId = Item.id) ON VehicleLocation.id = Item.vehicleLocationId) " +
                            $"WHERE ItemInstance.siteLocationId = '{siteLocationId}' AND Item.unitTypeName = '{equipment}' AND Item.parentId IS null AND (isBin<>1 AND isModule<>1) " +
                        "ORDER BY location, description, col2, col1;";
                    break;

                case WhichReport.Expiration:
                    query = "SELECT DISTINCT ItemType.name as description, (SELECT SUM(count) FROM Item AS I2 Where Item.id=I2.id) AS quantity, VehicleLocation.name AS location, expirationDate FROM (Item INNER JOIN ItemType ON Item.itemTypeId=ItemType.id) LEFT JOIN VehicleLocation ON Item.vehicleLocationId=VehicleLocation.id WHERE expirationDate IS NOT NULL ORDER BY expirationDate, description;";
                    break;

                case WhichReport.Service:
                    break;

                case WhichReport.Deployment:
                    break;

                case WhichReport.DamagedOrMissing:
                    break;

                case WhichReport.MainBoxContents:
                default:
                    //MessageBox.Show("Unsupported report selected, please report.", "Error - unknown report:", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }

            if (query == string.Empty)
            {
                return Enumerable.Empty<ReportItemsView>();
            }
            else
            {
                return db.QueryAsync<ReportItemSummaryView>(query).Result;
            }
        }
    }
}