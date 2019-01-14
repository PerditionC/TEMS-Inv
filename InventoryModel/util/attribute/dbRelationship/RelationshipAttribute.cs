// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using SQLite;

namespace TEMS.InventoryModel.util.attribute
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class RelationshipAttribute : IgnoreAttribute
    {
        protected RelationshipAttribute(string foreignKeyPropertyName)
        {
            ForeignKeyPropertyName = foreignKeyPropertyName;
        }

        /// <summary>
        /// the property name of the foreign key for the related entity (value actually stored in db)
        /// </summary>
        public string ForeignKeyPropertyName { get; private set; }
    }

    // many rows have a foreign key that references one row in another table
    public class ManyToOneAttribute : RelationshipAttribute
    {
        public ManyToOneAttribute(string foreignKeyPropertyName = null) : base(foreignKeyPropertyName)
        {
        }
    }

    // an entity references multiple rows in another table, those items also referenced by multiple
    // different rows in same table as entity, must use intermediate table
    public class ManyToManyAttribute : RelationshipAttribute
    {
        public ManyToManyAttribute(Type intermediateType, string foreignKeyPropertyName = null) : base(foreignKeyPropertyName)
        {
            IntermediateType = intermediateType;
        }

        public Type IntermediateType { get; private set; }
    }
}