// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

using NLog;

using TEMS.InventoryModel.util;

namespace TEMS.InventoryModel.entity.db
{
    public struct ReferenceDataType
    {
        public string Text { get; set; }
        public string TypeName { get; set; }
    }

    /// <summary>
    /// Maintains collections of reference data, lazily queried and cached
    /// Note: the cache is maintained per ReferenceDataCache, however, the
    /// same meta-data ReferenceDataTypes is used by all instances
    /// </summary>
    public class ReferenceDataCache : NotifyPropertyChanged
    {
        private const string ReferenceDataNameSpace = "TEMS.InventoryModel.entity.db.";
        private const string ReferenceDataAssembly = ",InventoryModel";

        // maintain a copy so we an load the data on demand
        private Database db;

        public ReferenceDataCache(Database db)
        {
            logger = LogManager.GetCurrentClassLogger();
            logger.Trace(nameof(ReferenceDataCache));

            this.db = db;
        }

        private const int DEFAULT_REFERENCEDATATYPE_LIST_COUNT = 12;

        /// <summary>
        /// returns a collection of all the reference data types and a descriptive name
        /// </summary>
        public static IList<ReferenceDataType> ReferenceDataTypes
        {
            get
            {
                if (_ReferenceDataTypes == null)
                {
                    _ReferenceDataTypes = new List<ReferenceDataType>(DEFAULT_REFERENCEDATATYPE_LIST_COUNT)
                    {
                        // TODO get PrettyName from attribute
                        new ReferenceDataType() { Text = "Battery Type", TypeName = nameof(BatteryType) },
                        new ReferenceDataType() { Text = "Equipment Unit Type", TypeName = nameof(EquipmentUnitType) }, // note: PK is name not id
                        new ReferenceDataType() { Text = "Item Category", TypeName = nameof(ItemCategory) },
                        new ReferenceDataType() { Text = "Item Status", TypeName = nameof(ItemStatus) },
                        new ReferenceDataType() { Text = "Item Type", TypeName = nameof(ItemType) },
                        new ReferenceDataType() { Text = "Service Category", TypeName = nameof(ServiceCategory) },
                        new ReferenceDataType() { Text = "Site Location", TypeName = nameof(SiteLocation) }, // note: PK is name not id
                        new ReferenceDataType() { Text = "Unit Of Measure", TypeName = nameof(UnitOfMeasure) },
                        new ReferenceDataType() { Text = "Vehicle Location", TypeName = nameof(VehicleLocation) },
                        new ReferenceDataType() { Text = "Vendors", TypeName = nameof(VendorDetail) },
                        // these two are not complete tables, only those used by Item Type
                        new ReferenceDataType() { Text = "Image", TypeName = nameof(Image) },
                        new ReferenceDataType() { Text = "Document", TypeName = nameof(Document) },
                    };
                }

                return _ReferenceDataTypes;
            }
        }

        private static IList<ReferenceDataType> _ReferenceDataTypes = null;

        /// <summary>
        /// maintains our lazily loaded cache of all reference data
        /// </summary>
        private Dictionary<string, ObservableCollection<ItemBase>> referenceData = new Dictionary<string, ObservableCollection<ItemBase>>(DEFAULT_REFERENCEDATATYPE_LIST_COUNT);

        /// <summary>
        /// our secondary cache which has values stored in a dictionary for direct access via primary key
        /// Note: second level Dictionary is only valid if our primary cache ReferenceData contains data for given type
        /// i.e. don't access referenceDataById[refTypeName][pk] unless ReferenceData.ContainsKey(refTypeName)
        /// </summary>
        private readonly Dictionary<string, Dictionary<object, ItemBase>> referenceDataById = new Dictionary<string, Dictionary<object, ItemBase>>(DEFAULT_REFERENCEDATATYPE_LIST_COUNT);

        /// <summary>
        /// force list data for typeName to be refreshed (retrieved again), i.e. re-sync with DB values
        /// Note: any changes not saved to DB will be lost
        /// </summary>
        /// <param name="typeName"></param>
        public void RefreshData(string typeName)
        {
            logger.Trace(nameof(RefreshData));
            try
            {
                logger.Debug($"Forcing refresh of {typeName}.");
                if (referenceData.ContainsKey(typeName))
                {
                    // simply remove from our cache, future retrievals will then reload from DB - i.e. lazy refresh
                    referenceData.Remove(typeName);
                    // notify any listeners that we may have changed contents, this may trigger the retrieval from DB
                    RaisePropertyChanged("Item[]");
                }
            }
            catch (Exception e)
            {
                logger.Error(e, $"Unable to properly refresh reference data list {typeName} - {e.Message}");
            }
        }

        /// <summary>
        /// Returns the Type for a reference type, it looks for the fully
        /// qualified type allowing the Type to be found even from other assemblies.
        /// i.e. converts a type's string name into a Type object for any ReferenceData
        /// Returns null if not a valid reference type, otherwise the Type
        /// Note: we do not throw an Exception on unsupported/invalid typeName as
        /// we expect that to be a fairly common case (e.g. by db.Load() tries to load here first)
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetReferenceType(string typeName)
        {
            logger.Trace($"{nameof(GetReferenceType)}({typeName})");
            var type = Type.GetType(ReferenceDataNameSpace + typeName + ReferenceDataAssembly);
            // limit to our reference data, otherwise we will cache but not use other types
            if (typeof(ReferenceData).IsAssignableFrom(type) || typeof(EquipmentUnitType).IsAssignableFrom(type))
               return type;
            return null;
        }

        /// <summary>
        /// Secondary indexer allowing direct access to an ItemBase object using primary key
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="pk"></param>
        /// <returns></returns>
        [IndexerName("Item")]
        public ItemBase this[string typeName, object pk]
        {
            get
            {
                logger.Trace($"Item[{typeName},pk]");

                // Note that because all reference items may not yet be loaded, we first check if particular type has been cached,
                // if not then we first load its values and fill our secondary dictionary cache - once loaded
                // we simply return the item from the proper dictionary based on primary key (since we know type we directly use pk property
                // instead of indirectly via PrimarKey method which uses reflection.
                if (!referenceData.ContainsKey(typeName))
                {
                    // load our caches
                    var unused = this[typeName];
                    // if still not found return null
                    if (!referenceData.ContainsKey(typeName)) return null;
                }

                // we do pk null check here as it allows using a null pk to warm cache
                if (pk == null) return null;

                // we also need to check for bad primary keys
                var dict = referenceDataById[typeName];
                return (dict.ContainsKey(pk)) ? dict[pk] : null;
            }
        }

        /// <summary>
        /// allow retrieval of items via nameof(Type) string
        /// e.g. var allBatteryTypes = myObj.referenceDataLists[nameof(BatteryType)];
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        [IndexerName("Item")]
        public ObservableCollection<ItemBase> this[string typeName]
        {
            get
            {
                logger.Trace($"Item[{typeName}]");
                try
                {
                    if (!referenceData.ContainsKey(typeName))
                    {
                        // get object type to load which is used to get TableName to load data from
                        Type objType;
                        if ((objType = GetReferenceType(typeName)) == null) return null;
                        // limit loading of Document and Image to only those referenced by an ItemType
                        string query = null;
                        if (typeName.Equals(nameof(Document)))
                        {
                            // all documents that are linked to an ItemType
                            query = "WHERE Document.id IN (SELECT documentId FROM ItemTypeDocumentMapping)";
                        }
                        if (typeName.Equals(nameof(Image)))
                        {
                            // all images that are linked to an ItemType
                            query = "WHERE Image.id IN (SELECT imageId FROM ItemTypeImageMapping)";
                        }
                        var itemCollection = new ObservableCollection<ItemBase>(db.InvokeLoadRows(objType, query).Cast<ItemBase>());
                        // and add to our cache
                        referenceData.Add(typeName, itemCollection);

                        // we utilize ReferenceData to determine if secondary cache is valid or not
                        // since we are filling ReferenceData we need to also create and fill our secondary cache
                        // which is a Dictionary so we can quickly get items by their primary key
                        var dict = new Dictionary<object, ItemBase>(itemCollection.Count);
                        referenceDataById[typeName] = dict;

                        // add to our secondary cache
                        foreach (var ib in itemCollection)
                        {
                            dict.Add(ib.PrimaryKey, ib);
                        }
                    }

                    return referenceData[typeName];
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Failed to retrieve reference data list! typeName={typeName} - {e.Message}");
                    throw;
                }
            }
        }
    }

    public static class ReferenceDataHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Given an ObservableCollection of ItemBase objects, returns one with indicated primary key
        /// Note should use secondary indexer [type,pk] instead of this extension method
        /// </summary>
        /// <param name="referenceList"></param>
        /// <returns></returns>
        public static T ById<T>(this ObservableCollection<ItemBase> referenceList, object pk) where T : ItemBase
        {
            logger.Trace(nameof(ById));
            if (pk == null) return null;
            return referenceList?.Where(item => pk.Equals(item.PrimaryKey))?.FirstOrDefault() as T;
        }

        /// <summary>
        /// Given an ObservableCollection of ReferenceData objects, returns one with indicated value for name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="referenceList"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T ByName<T>(this ObservableCollection<ItemBase> referenceList, string name) where T : ReferenceData
        {
            logger.Trace(nameof(ByName));
            var item = referenceList.FirstOrDefault(x => ((ReferenceData)x).name == name) as T;
            if (item == null) logger.Debug($"ByName results are null, {name} was not found in list.");
            return item;
        }
    }
}