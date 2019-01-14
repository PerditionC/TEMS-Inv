// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.ObjectModel;

using TEMS.InventoryModel.util;

/// <summary>
/// structures used to return results of various database queries
/// </summary>
namespace TEMS.InventoryModel.entity.db.query
{
    /// <summary>
    /// results of a database search, base class with basic information
    /// needed to display master results and load/edit details
    /// [key master information for master/detail view]
    /// </summary>
    public class SearchResult : NotifyPropertyChanged
    {
        public SearchResult() : base()
        {
        }

        /// <summary>
        /// primary key of main component represented, may be used to retrieve the full db record associated with this result
        /// </summary>
        public Guid id
        {
            get { return _id; }
            set { SetProperty(ref _id, value, nameof(id)); }
        }

        private Guid _id = Guid.Empty;

        /// <summary>
        /// what is the type of and corresponding table to load full entity from
        /// </summary>
        public string entityType
        {
            get { return _entityType; }
            set { SetProperty(ref _entityType, value, nameof(entityType)); }
        }

        private string _entityType = null;

        /// <summary>
        /// Maintains a weak reference to entity referenced by this query result, returns null if non specified or already garbage collected
        /// </summary>
        public ItemBase entity
        {
            /*
            get { return _entity.IsAlive ? _entity.Target as ItemBase : null; }
            set { SetProperty(ref _entity, new WeakReference(value), nameof(entity)); }
            */
            get; set;
        }

        private WeakReference _entity = new WeakReference(null);

        /// <summary>
        /// basic textual description of component, e.g. name, description, title, ...
        /// </summary>
        public string description
        {
            get { return _description; }
            set { SetProperty(ref _description, value, nameof(description)); }
        }

        private string _description = null;

        /// <summary>
        /// null or parent object
        /// </summary>
        public SearchResult parent
        {
            get { return _parent; }
            set { SetProperty(ref _parent, value, nameof(parent)); }
        }

        private SearchResult _parent = null;

        /// <summary>
        /// primary key of parent item
        /// may be used to determine value for parent or unused
        /// </summary>
        public Guid parentId
        {
            get { return _parentId; }
            set
            {
                SetProperty(ref _parentId, value, nameof(parentId));
            }
        }

        private Guid _parentId = Guid.Empty;

        /// <summary>
        /// null or collection of child objects
        /// </summary>
        public ObservableCollection<SearchResult> children
        {
            get { return _children; }
            set
            {
                SetProperty(ref _children, value, nameof(children));
                RaisePropertyChanged(nameof(childCount));
                RaisePropertyChanged(nameof(resultTotal));
            }
        }

        private ObservableCollection<SearchResult> _children = new ObservableCollection<SearchResult>();

        /// <summary>
        /// how many direct descendants we contain, e.g. a group's immediate count
        /// </summary>
        public int childCount
        {
            get { return children?.Count ?? 0; }
        }

        /// <summary>
        /// returns how many results we encapsulate, i.e. # of results from query
        /// </summary>
        public int resultTotal
        {
            get
            {
                var total = 0;
                foreach (var child in children)
                {
                    total += child.resultTotal;
                }

                /* kids total plus ourselves */
                return total + includeSelfInResultTotal();
            }
        }

        /// <summary>
        /// virtual method used to determine if self included in resultsTotal
        /// Should return 0 to not include self, default is 1.
        /// </summary>
        /// <returns></returns>
        protected virtual int includeSelfInResultTotal() { return 1; }

        /// <summary>
        /// quantity associated with this result
        /// for items this may be the quantity of an item contained in a given location
        /// and for header items this may be set the same as childCount
        /// </summary>
        public int quantity { get { return _quantity; } set { SetProperty(ref _quantity, value, nameof(quantity)); } }

        private int _quantity = -1;

        /// <summary>
        /// is this object selected?
        /// </summary>
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { SetProperty(ref _IsSelected, value, nameof(IsSelected)); }
        }

        private bool _IsSelected = false;

        /// <summary>
        /// is this object's branch is expanded?
        /// If set to expanded then will ensure parent's are also expanded; this
        /// allows finding this object and then ensuring whole tree branch expanded down to it
        /// </summary>
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                SetProperty(ref _IsExpanded, value, nameof(IsExpanded));

                // Expand all the way up to the root.
                if (_IsExpanded && _parent != null) _parent.IsExpanded = true;
            }
        }

        private bool _IsExpanded = false;

        /// <summary>
        /// returns if text is a substring (contained within) the description
        /// Used to aid in finding specific matches within tree
        /// </summary>
        /// <param name="text">the string to see if within description</param>
        /// <returns>true if text is a substring (contained within) description, false otherwise</returns>
        public bool DescriptionContainsText(string text)
        {
            // don't consider a match if nothing to match to
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.description))
                return false;

            // is it a substring?
            return this.description.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }
    }

    public class GroupHeader : SearchResult
    {
        // Note: don't set expanded here, header is used at multiple nesting levels and setting this to true has the same
        // effect almost as setting IsExpanded to true by default.
        public GroupHeader() : base()
        {
            id = Guid.Empty;
        }

        /// <summary>
        /// virtual method used to determine if self included in resultsTotal
        /// Do not include header elements in count.
        /// </summary>
        /// <returns></returns>
        protected override int includeSelfInResultTotal() { return 0; }
    }

    // to allow differentiation of what group contains
    public class BinGroupHeader : GroupHeader { }

    public class ModuleGroupHeader : GroupHeader { }

    public class ItemGroupHeader : GroupHeader { }


    public class GenericItemResult : SearchResult
    {
        public GenericItemResult() : base()
        {
        }

        public string itemNumber { get { return _itemNumber; } set { SetProperty(ref _itemNumber, value, nameof(itemNumber)); } }
        private string _itemNumber = string.Empty;

        public bool isModule { get { return _isModule; } set { SetProperty(ref _isModule, value, nameof(isModule)); } }
        private bool _isModule = false;

        public bool isBin { get { return _isBin; } set { SetProperty(ref _isBin, value, nameof(isBin)); } }
        private bool _isBin = false;

        public string unitTypeName { get { return _unitTypeName; } set { SetProperty(ref _unitTypeName, value, nameof(unitTypeName)); } }
        private string _unitTypeName = string.Empty;

        public Guid siteLocationId { get { return _siteLocationId; } set { SetProperty(ref _siteLocationId, value, nameof(siteLocationId)); } }
        private Guid _siteLocationId = Guid.Empty;

        // Note: statusId is only valid on ItemInstance queries (not for all queries filtering based on status)
        public Guid statusId { get { return _statusId; } set { SetProperty(ref _statusId, value, nameof(statusId)); } }
        private Guid _statusId = Guid.Empty;

        // Note: parentItemId is only valid on ItemInstance queries, it is the id of the corresponding Item for parent matching
        public Guid parentItemId { get { return _parentItemId; } set { SetProperty(ref _parentItemId, value, nameof(parentItemId)); } }
        private Guid _parentItemId = Guid.Empty;
    }

    /// <summary>
    /// the equipment unit, note that primary key is a string 'name' not Guid 'id'
    /// </summary>
    public class EquipmentUnitResult : GroupHeader
    {
        public EquipmentUnitResult() : base()
        {
            IsExpanded = true;
        }

        /// <summary>
        /// the short name of equipment, the description is set to the full long name
        /// </summary>
        public string name
        {
            get { return _name; }
            set { SetProperty(ref _name, value, nameof(name)); }
        }

        private string _name = null;
    }
}