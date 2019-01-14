// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using SQLite;

namespace SQLiteNetSessionModule.Sync
{
    [Serializable]
    public class ReplicationException : ApplicationException
    {
        public ReplicationException() : base() { }
        public ReplicationException(SQLite3.Result errorCode) : base() { this.errorCode = errorCode; }

        public ReplicationException(string message) : base(message) { }
        public ReplicationException(string message, SQLite3.Result errorCode) : base(message) { this.errorCode = errorCode; }

        public ReplicationException(string message, Exception innerException) : base(message, innerException) { }
        public ReplicationException(string message, Exception innerException, SQLite3.Result errorCode) : base(message, innerException) { this.errorCode = errorCode; }

        public SQLite3.Result errorCode { get; set; } = SQLite3.Result.OK;

        public override string ToString()
        {
            return base.ToString() + ", SQLite3.Result=" + errorCode.ToString();
        }
    }
}
