// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Reflection;

using TEMS.InventoryModel.util;
using TEMS.InventoryModel.util.attribute;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Base class for items in item collections
    /// allows for some shared logic of use and any future common logic
    /// </summary>
    public abstract class ItemBase : NotifyPropertyChanged
    {
        public ItemBase(PropertyInfo pkProp) : base()
        {
            _pkProp = pkProp;
        }
        public ItemBase() : this(null)
        {
        }

        // return a copy (clone) of this instance of derived type
        // assumes derived class implements a constructor that takes an instance
        // of same type and copies values into newly created instance
        // e.g. ItemBase(ItemBase initializeFrom) { this.<fields> = initializeFrom.<fields>; }
        public ItemBase GetClonedItem()
        {
            try
            {
                // first try to create a clone via copy constructor
                return (ItemBase)Activator.CreateInstance(this.GetType(), new Object[] { this });
            }
            catch (MissingMemberException e)
            {
                // if we failed to implement copy constructor attempt shallow clone via
                logger.Debug(e, $"Does class {this.GetType().Name} implement copy constructor (initialized from copy of same Type)?");
                return (ItemBase)this.MemberwiseClone();
            }
            catch (Exception e)
            {
                logger.Error(e, "Unexpected error trying to get a cloned item.");
                throw;
            }
        }

        #region obtain Primary Key

        // cache property info for retrieving primary key value
        private PropertyInfo _pkProp = null;

        private PropertyInfo pkProp
        {
            get
            {
                // on first call attempt to get Primary Key
                // we expect derived items to define a primary key, however, if no pk then will search for pk property on each invocation
                if (_pkProp == null)
                {
                    _pkProp = this.GetType().GetPrimaryKey();
                }

                return _pkProp;
            }
        }

        // returns value of primary key if defined, null otherwise (e.g. auto-number not yet inserted)
        [SQLite.Ignore]
        [HideProperty]
        [DisplayNameProperty] // if not otherwise specified on subclass then item identified by PK
        public Object PrimaryKey
        {
            get { return pkProp?.GetValue(this, null); }
            set
            {
                if (pkProp != null)
                {
                    pkProp.SetValue(this, value, null);
                    RaisePropertyChanged(nameof(PrimaryKey)); /// why? do it here not in subclasses but not both - Note: the underlying Property does this to signal PK changed, so skip it here.
                }
            }
        }

        #endregion obtain Primary Key

        #region obtain display name (pretty name and/or other identifier)

        // cache property info for retrieving name value
        private PropertyInfo _nameProp = null;

        private PropertyInfo nameProp
        {
            get
            {
                // on first call attempt to get name property meta info
                // we expect derived items to define a name property, however, if none then will search for on each invocation
                if (_nameProp == null)
                {
                    // get this instance's Type, then get list of all potential pk Properties (note, not fields, must be public and settable)
                    var type = this.GetType();
                    PropertyInfo[] propInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    // search through and find one flagged with name key attribute [DisplayName("Error color")]
                    foreach (var propInfo in propInfos)
                    {
                        var attrs = propInfo.GetCustomAttributes(typeof(DisplayNamePropertyAttribute), true);
                        if (attrs.Length > 0)
                        {
                            _nameProp = propInfo;

                            // found primary key, no need to keep looking so terminate loop
                            break;
                        }
                    }
                }

                return _nameProp;
            }
        }

        // returns value of primary key if defined, null otherwise (e.g. auto-number not yet inserted)
        // !!! warning: only valid value returned if primary key is a string!!!
        [SQLite.Ignore]
        [HideProperty]
        public string displayName
        {
            get { return nameProp?.GetValue(this, null) as string ?? "<unnamed>"; }
        }

        #endregion obtain display name (pretty name and/or other identifier)

        #region // check persistence (DB) constraints

        // returns true if all Non Null constraints satisfied
        public virtual bool AreNonNullConstraintsSatisfied()
        {
            // get this instance's Type, then get list of all Properties (note, not fields, must be public and settable)
            var type = this.GetType();
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);

            // search through and find ones flagged with NonNull attribute
            foreach (var propertyInfo in propertyInfos)
            {
                var attrs = propertyInfo.GetCustomAttributes(typeof(SQLite.NotNullAttribute), true);
                if (attrs.Length > 0)
                {
                    // found a property marked as must not be null
                    // so see if it's value is null (or blank for strings) then constraints are NOT satisfied
                    var propertyValue = propertyInfo.GetValue(this, null);

                    // if this property is a string, additionally check if it is empty or only whitespace
                    if (typeof(string) == propertyInfo.PropertyType)
                    {
                        if (String.IsNullOrWhiteSpace(propertyValue as string)) return false;
                    }
                    else
                    {
                        // otherwise just determine if null or not
                        if (propertyValue == null) return false;
                    }
                }
            }

            // assume satisfied if no properties, none marked NonNull, or all marked ones have values
            return true;
        }

        #endregion // check persistence (DB) constraints

        #region Equals

        // override object.Equals
        // this is required for DB objects as the same value may
        // be loaded by different calls thus creating different
        // instances of the same data.  If the same data does not
        // Equal then there are issues with binding and other
        // comparisons where we only care if the data is the same
        // not if they are exactly the same object.
        // Note: this could create subtle issues if two objects
        // are assumed to be the same, changing one may not change
        // the other - for best practice all data loaded from the
        // DB should be treated as read-only or revalidated/loaded
        // before using if changes allowed.
        // WARNING: only checks primary key, TODO other values
        public override bool Equals(object obj)
        {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (!(obj is ItemBase ib) || GetType() != obj.GetType())
            {
                return false;
            }

            return this.PrimaryKey.Equals(ib.PrimaryKey);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // WARNING: may require changing!

            return this.PrimaryKey.GetHashCode();
        }

        #endregion Equals
    }
}