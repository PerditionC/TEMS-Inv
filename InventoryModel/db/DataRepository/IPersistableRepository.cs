// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using TEMS.InventoryModel.entity.db;

namespace TEMS.InventoryModel.entity
{
    /// <summary>
    /// General application exception when an error causes save to fail, if caused by exception will be innerException
    /// </summary>
    [Serializable]
    public class SavedFailedException : ApplicationException
    {
        public SavedFailedException() : base()
        {
        }

        public SavedFailedException(string message) : base(message)
        {
        }

        public SavedFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// General application exception when an error causes load to fail, if caused by exception will be innerException
    /// </summary>
    [Serializable]
    public class LoadFailedException : ApplicationException
    {
        public LoadFailedException() : base()
        {
        }

        public LoadFailedException(string message) : base(message)
        {
        }

        public LoadFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public interface IPersistableRepository : IDisposable
    {
        /// <summary>
        /// Returns true if entity exists.
        /// Note: For db this merely indicates that primary key already exists
        /// in corresponding entity table, it does not imply any other information exists or matches.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Exists<T>(T entity) where T : ItemBase;

        /// <summary>
        /// Returns true if entity exists.
        /// Note: For db this merely indicates that primary key already exists
        /// in corresponding entity table, it does not imply any other information exists or matches.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Exists(string tableName, string pkName, Guid primaryKey);

            /// <summary>
        /// Persist entity of type T to somewhere,
        /// Only stores contents of entity, any nested items must be
        /// Save'd prior to calling to ensure foreign key constraints met
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void Save<T>(T entity) where T : class;

        /// <summary>
        /// Loads entity of type T using primary key
        /// </summary>
        /// <param name="entity"></param>
        T Load<T>(object primaryKey) where T : class, new();

        /// <summary>
        /// Removes entity,will no longer exist
        /// Note: entity may be re-added via Save(entity) call if user object still available
        /// </summary>
        /// <param name="entity"></param>
        void Delete<T>(T entity) where T : class;
    }
}