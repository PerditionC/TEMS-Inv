// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS.InventoryModel.entity.db
{
    [Serializable]
    public class DatabaseFormatException : ApplicationException
    {
        public DatabaseFormatException() : base()
        {
        }

        public DatabaseFormatException(string message) : base(message)
        {
        }

        public DatabaseFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}