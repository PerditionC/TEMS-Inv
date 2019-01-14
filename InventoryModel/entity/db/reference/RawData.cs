// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using SQLite;

using TEMS.InventoryModel.util;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Basic properties used for file [document/image/etc] storage]
    /// Actual storage done in more specific tables based on subclass name
    /// </summary>
    public class RawData : ReferenceData
    {
        public RawData(Guid id) : base(id)
        {
        }

        public RawData() : this(Guid.NewGuid())
        {
        }

        // id (Primary Key) and name (DisplayNameProperty) are derived from ReferenceData
        // name is the filename or other short description to reference this item in SQLite archive table
        // Note: ReferenceData limits name to 128, however DB has no hard limit and table value corresponds
        // with has no suggested limit - if modified by external means, names longer than 128 will be ignored


        // longer description of what document is or represents
        [MaxLength(128)]
        public string description { get { return _description; } set { SetProperty(ref _description, value, nameof(description)); } }

        private string _description = null;

        // raw document, image, etc file data (txt, pdf, doc, ..., png, bmp, jpg, ...)
        // loaded as needed
        [SQLite.Ignore]
        public byte[] data { get { return _data; } set { SetProperty(ref _data, value, nameof(data)); } }

        private byte[] _data = new byte[0];
    }

    /// <summary>
    /// Specific image from images table
    /// Image are stored separate to avoid overhead of loading data when not
    /// required (e.g. for reports, etc).  Also allows sharing images more easily among items.
    /// </summary>
    public class Image : RawData
    {
        public Image() : base()
        {
        }

        public Image(Guid id) : base(id)
        {
        }
    }

    /// <summary>
    /// Documentation such as user manuals for items
    /// Document are stored separate to avoid overhead of loading data when not
    /// required (e.g. for reports, etc).  Also allows sharing docs more easily among items.
    /// </summary>
    public class Document : RawData
    {
        public Document() : base()
        {
        }

        public Document(Guid id) : base(id)
        {
        }
    }

    public class SqlAr : NotifyPropertyChanged
    {
        // filename or other short description of this item
        [NotNull, MaxLength(32)]
        public string name { get { return _name; } set { SetProperty(ref _name, value, nameof(name)); } }

        private string _name = null;

        // access permissions, not used
        public long mode { get { return _mode; } set { SetProperty(ref _mode, value, nameof(mode)); } }

        private long _mode = 0;

        // last modification time
        public long mtime { get { return _mtime; } set { SetProperty(ref _mtime, value, nameof(mtime)); } }

        private long _mtime = 0;

        // original file size
        public long sz { get { return _sz; } set { SetProperty(ref _sz, value, nameof(sz)); } }

        private long _sz = 0;

        // raw document, image, etc file data (txt, pdf, doc, ..., png, bmp, jpg, ...)
        [NotNull]
        public byte[] data { get { return _data; } set { SetProperty(ref _data, value, nameof(data)); } }

        private byte[] _data = new byte[0];
    }
}