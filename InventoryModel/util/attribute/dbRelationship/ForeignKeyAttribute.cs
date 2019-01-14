// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using SQLite;

namespace TEMS.InventoryModel.util.attribute
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : IndexedAttribute
    {
        public ForeignKeyAttribute(string entityPropertyName = null, Type foreignTableType = null)
        {
            EntityPropertyName = entityPropertyName;
            ForeignTableType = foreignTableType;
        }

        /// <summary>
        /// the property name of the related entity (the foreign key refers to)
        /// Note: may be null if only foreign key is used and not entity itself
        /// </summary>
        public string EntityPropertyName { get; private set; }

        /// <summary>
        /// the Type used to hold an entity of the table this foreign key refers to
        /// Note: only used for ManyToMany mapping tables, otherwise null
        /// </summary>
        public Type ForeignTableType { get; private set; }
    }
}