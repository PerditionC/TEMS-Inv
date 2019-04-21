using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

using NLog;

using SQLite;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util.attribute;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// extensions used by Database class
    /// </summary>
    public static class DatabaseHelper
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// marks all items in a list as not changed
        /// </summary>
        /// <typeparam name="L">a List of type T or any other IList of type T</typeparam>
        /// <typeparam name="T">the type contained with the List, should support IChangeTracking</typeparam>
        /// <param name="items">the list to iterate over accepting any changes so as to be considered not changed</param>
        /// <returns></returns>
        public static List<T> NotChanged<T>(this List<T> items) { return NotChanged<List<T>, T>(items); }

        public static IList<T> NotChanged<T>(this IList<T> items)
        {
            return NotChanged<IList<T>, T>(items);
        }

        public static L NotChanged<L, T>(this L items) where L : IList<T>
        {
            logger.Trace(nameof(NotChanged));
            if (items == null) return default;
            if (typeof(IChangeTracking).IsAssignableFrom(typeof(T)))
            {
                foreach (var item in items)
                {
                    var tracked = item as IChangeTracking;
                    tracked.AcceptChanges();
                }
            }
            return items;
        }

        /// <summary>
        /// extracts extended error code from ConstraintViolationExceptions,
        /// or returns empty string if not ConstraintViolationException (or lacks extended error).
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetSqliteExtendedError<T>(this T e) where T : Exception
        {
            if (e is ConstraintViolationException constraintException)
            {
                if (constraintException.extendedErrCode != 0)
                    return $"Constraint violation: {constraintException.extendedErrCode.ToString()}";
            }

            return string.Empty;
        }

        public static string GetTableName(this Type type)
        {
            var tableName = type.Name;
            var tableAttribute = type.GetAttribute<TableAttribute>();
            if (tableAttribute != null && tableAttribute.Name != null)
                tableName = tableAttribute.Name;

            return tableName;
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            T attribute = null;
            var attributes = (T[])type.GetCustomAttributes(typeof(T), inherit: true);
            if (attributes.Length > 0)
            {
                attribute = attributes[0];
            }
            return attribute;
        }

        public static T GetAttribute<T>(this PropertyInfo property) where T : Attribute
        {
            T attribute = null;
            var attributes = (T[])property.GetCustomAttributes(typeof(T), true);
            if (attributes.Length > 0)
            {
                attribute = attributes[0];
            }
            return attribute;
        }

        public static PropertyInfo GetPropertyInfo(this Type type, string propertyName)
        {
            return (from property in type.GetPublicInstanceProperties()
                    where string.Equals(property.Name, propertyName, StringComparison.InvariantCulture)
                    select property).FirstOrDefault();
        }

        public static object GetDefault(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        // returns only public instance properties
        public static PropertyInfo[] GetPublicInstanceProperties(this Type type)
        {
            return type?.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public static PropertyInfo GetPrimaryKey(this Type type)
        {
            return (from property in type.GetPublicInstanceProperties()
                    where property.GetAttribute<PrimaryKeyAttribute>() != null
                    select property).FirstOrDefault();
        }

        public static List<PropertyInfo> GetRelationshipProperties(this Type type)
        {
            return (from property in type.GetPublicInstanceProperties()
                    where property.GetAttribute<RelationshipAttribute>() != null
                    select property).ToList();
        }

        public static List<PropertyInfo> GetForeignKeyProperties(this Type type)
        {
            return (from property in type.GetPublicInstanceProperties()
                    where property.GetAttribute<ForeignKeyAttribute>() != null
                    select property).ToList();
        }

        /// <summary>
        /// Invokes callback for each related entity (any objects with a foreign key relationship)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="relatedEntity"></param>
        public static void IterateOverRelatedEntities<T>(this T entity, Action<object> callback) where T : class
        {
            foreach (var relationshipProperty in entity.GetType().GetRelationshipProperties())
            {
                var value = relationshipProperty.GetValue(entity, null);

                if (value is IEnumerable)
                {
                    var enumerable = value as IEnumerable;
                    foreach (var element in enumerable)
                    {
                        callback(element);
                    }
                }
                else if (value != null)
                {
                    callback(value);
                }
            }
        }

        /// <summary>
        /// Invokes callback for each related entity (any objects with a foreign key relationship)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="relatedEntity"></param>
        public static void IterateOverManyToManyRelatedEntities<T>(this T entity, Action<Type, PropertyInfo, Type, PropertyInfo, object, IEnumerable> callback) where T : class
        {
            // cache entity's Type
            var entityType = entity.GetType();

            // get PropertyInfo for each ManyToOne or ManyToMany property
            foreach (var relationshipProperty in entityType.GetRelationshipProperties())
            {
                // we only care about ManyToMany, so ignore others
                var relationshipAttribute = relationshipProperty.GetAttribute<ManyToManyAttribute>();
                if (relationshipAttribute == null) continue;

                // get Type used for mapping
                Type mappingType = relationshipAttribute.IntermediateType;
                PropertyInfo entityPkProp = null;
                PropertyInfo foreignPkProp = null;
                Type foreignEntityType = null;

                // get foreign key props
                foreach (var fkProp in mappingType.GetForeignKeyProperties())
                {
                    var fkAttribute = fkProp.GetAttribute<ForeignKeyAttribute>();
                    if (fkAttribute.ForeignTableType == entityType)
                    {
                        entityPkProp = fkProp;
                    }
                    else //if (fkAttribute.ForeignTableType == relationshipProperty.PropertyType) -- collection of T, need T
                    {
                        foreignEntityType = fkAttribute.ForeignTableType;
                        foreignPkProp = fkProp;
                    }
                }

                // ManyToMany must be IEnumerable collection, iterate through all and ensure
                // a mapping exists between entity and items in collection
                var value = relationshipProperty.GetValue(entity, null);
                if (value is IEnumerable)
                {
                    // cache entity's primary key value
                    var entityPk = entityType.GetPrimaryKey().GetValue(entity, null);

                    callback(mappingType, entityPkProp, foreignEntityType, foreignPkProp, entityPk, value as IEnumerable);
                }
            }
        }

        /// <summary>
        /// Returns first item found that returns true for matchingFn
        /// Note: this is O(N) where N is sum of all items and their children
        /// </summary>
        /// <param name="items"></param>
        /// <param name="matchingFn"></param>
        /// <returns></returns>
        public static SearchResult FindItem(this ObservableCollection<SearchResult> items, Func<SearchResult, bool> matchingFn)
        {
            //return Items.Where(matchingFn).FirstOrDefault<SearchResult>();
            foreach (var item in items)
            {
                // have we found a matching item, then return it
                if (matchingFn(item))
                {
                    return item;
                }

                // handle children recursively
                if (item.childCount > 0)
                {
                    var selectedItem = FindItem(item.children, matchingFn);
                    if (selectedItem != null)
                        return selectedItem;
                }
            }

            // if no items in initial collection or any in any child collection then return not found
            return null;
        }

        /// <summary>
        /// Returns a list of all extracted string values from SearchResults
        /// </summary>
        /// <param name="items">search results to obtain list of values from</param>
        /// <param name="GetValueFn">should return either null or string value</param>
        /// <param name="values">maintains the list, if null then new list created</param>
        /// <returns></returns>
        public static IList<string> GetValues(this ObservableCollection<SearchResult> items, Func<SearchResult, string> GetValueFn, ref List<string> values)
        {
            if (values == null) values = new List<string>();

            foreach (var item in items)
            {
                var value = GetValueFn(item);
                if (!string.IsNullOrEmpty(value)) values.Add(value); // ignore headers or other items without needed value
                if (item.childCount > 0)
                    GetValues(item.children, GetValueFn, ref values);
            }

            return values;
        }

        /// <summary>
        /// GetValueFn to return Primary Key from search results
        /// </summary>
        /// <param name="searchResult"></param>
        /// <returns></returns>
        public static string GetPrimaryKey(SearchResult searchResult)
        {
            return searchResult is GenericItemResult item ? item.id.ToString() : null;
        }
    }
}