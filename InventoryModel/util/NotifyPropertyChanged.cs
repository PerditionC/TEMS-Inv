// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Collections.Generic;
using System.ComponentModel;

using NLog;

using StatePrinting;
using StatePrinting.FieldHarvesters;

namespace TEMS.InventoryModel.util
{
    public class NotifyPropertyChanged : INotifyPropertyChanged, IChangeTracking
    {
        // this is a base class for most of our items, and we want logging available everywhere,
        // so we are adding this protected logger here even though logically it is not part of INotifyPropertyChanged
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        public NotifyPropertyChanged() : base()
        {
            // limit toString to public values
            printer.Configuration.Add(new PublicFieldsAndPropertiesHarvester());
        }

        #region toString

        protected static readonly Stateprinter printer = new Stateprinter();

        public override string ToString()
        {
            return printer.PrintObject(this);
        }

        #endregion toString

        [SQLite.Ignore]
        /// <summary>
        /// indicates if current instance has been changed
        /// Always set to true if SetProperty is called with a new value
        /// On persisting (saving) it is the callers responsibility to
        /// set false via AcceptChanges() so new changes can be triggered.
        /// </summary>
        public bool IsChanged
        {
            get { return _IsChanged; }
            private set
            {
                // Warning, do not use SetProperty as it will cause a recursive loop as it sets IsChanged=true
                // we purposely always trigger a change when value is set
                _IsChanged = value;
                RaisePropertyChanged(nameof(IsChanged));
            }
        }

        private bool _IsChanged = true;

        /// <summary>
        /// indicates item is unmodified from saved value, ie not changed
        /// </summary>
        public void AcceptChanges()
        {
            IsChanged = false;
        }

        /// <summary>
        /// indicates item is modified from saved value, ie changed
        /// Note: normally this should not be used, a property should be set
        /// which will indirectly set this, however, this may be useful
        /// if list values (not list itself) or nested properties have changed.
        /// </summary>
        public void IndicateChanges()
        {
            IsChanged = true;
        }

        /// <summary>
        /// event handler that can be registered externally to receive change events
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// internal method to notify registered handler of change event
        /// Note: this will also update IsChanged status to true (which will also raise a IsChanged event)
        /// </summary>
        /// <param name="name">the name of property that has changed,
        /// string.Empty may be passed to signal all properties changed
        /// </param>
        protected void RaisePropertyChanged(string name)
        {
            // avoid recursive calls
            if (!nameof(IsChanged).Equals(name, System.StringComparison.InvariantCulture)) IsChanged = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        /// <summary>
        /// convenience method to set & signal change, but only if value actually changed
        /// </summary>
        /// <typeparam name="T">Type for the property to be updated</typeparam>
        /// <param name="field">the backing field for the property</param>
        /// <param name="value">the [new] value for the property</param>
        /// <param name="propertyName">the name of the property to update</param>
        /// <returns>return true if value is new and update occurs, false if value is unchanged & no notification</returns>
        protected bool SetProperty<T>(ref T field, T value, /*[CallerMemberName]*/string propertyName = "")
        {
            // if there is no backing field or value is unchanged then return without signaling change
            // Note: EqualityCompare used to allow types to override Equals from default implementation
            if (EqualityComparer<T>.Default.Equals(field, value)) { return false; }
            // otherwise update with the new value and signal the change, returning change occurred
            field = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}