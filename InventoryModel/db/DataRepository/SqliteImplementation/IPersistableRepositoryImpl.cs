// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Interface to database entities - all queries, updates, etc. handled via an instance of this class.
    /// Uses Database class to perform actual interaction with back-end database.
    /// </summary>
    public sealed partial class DataRepository : IPersistableRepository, IDisposable
    {
        #region IPersistableRepository

        /// <summary>
        /// Returns true if entity exists.
        /// Note: For db this merely indicates that primary key already exists
        /// in corresponding entity table, it does not imply any other information exists or matches.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Exists<T>(T entity) where T : ItemBase
        {
            logger.Trace("Exists<T>");
            try
            {
                var pk = entity.PrimaryKey;
                // TODO: this works as our DB only has id or name for primary key, depending on type; update if that changes!
                var pkName = (typeof(Guid).IsAssignableFrom(pk.GetType())) ? "id" : "name";
                /* Note: we get table name via entity.GetType() instead of typeof(T) as the latter is usually an ItemBase not actual Type */
                var entityCount = db.ExecuteScalar<int>($"SELECT COUNT(*) FROM {entity.GetType().GetTableName()} WHERE {pkName}=?;", entity.PrimaryKey);
                return entityCount > 0;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error occurred when updating database - {e.Message}");
                throw new SavedFailedException("Error occurred when updating database.", e);
            }
        }

        /// <summary>
        /// Returns true if entity exists.
        /// Note: For db this merely indicates that primary key already exists
        /// in corresponding entity table, it does not imply any other information exists or matches.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Exists(string tableName, string pkName, Guid primaryKey)
        {
            logger.Trace($"Exists({tableName},{primaryKey}");
            try
            {
                var entityCount = db.ExecuteScalar<int>($"SELECT COUNT(*) FROM `{tableName}` WHERE `{pkName}`=?;", primaryKey);
                return entityCount > 0;
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error occurred when updating database - {e.Message}");
                throw new SavedFailedException("Error occurred when updating database.", e);
            }
        }

        /// <summary>
        /// Persist entity of type T to database.
        /// Will ensure any [changed] contained items are saved first to ensure
        /// foreign key constraints are met
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void Save<T>(T entity) where T : class
        {
            logger.Trace("Save<T>");
            try
            {
                db.Save(entity);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error occurred when updating database - {e.Message}");
                throw new SavedFailedException("Error occurred when updating database.", e);
            }
        }

        /// <summary>
        /// Loads entity of type T using primary key
        /// </summary>
        /// <param name="entity"></param>
        public T Load<T>(object primaryKey) where T : class, new()
        {
            logger.Trace("Load<T>");
            try
            {
                return db.Load<T>(primaryKey);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error occurred when loading {nameof(T)}, pk=[{primaryKey.ToString()}] - {e.Message}");
                throw new LoadFailedException($"Error occurred when loading {nameof(T)} from database.", e);
            }
        }

        public object Load(object primaryKey, string TableName)
        {
            logger.Trace($"Load(pk,{TableName}");
            try
            {
                return db.Load(primaryKey, TableName);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error occurred when loading {TableName}, pk=[{primaryKey.ToString()}] - {e.Message}");
                throw new LoadFailedException($"Error occurred when loading {TableName} from database.", e);
            }
        }

        /// <summary>
        /// Removes entity,will no longer exist
        /// Note: entity may be re-added via Save(entity) call if user object still available
        /// </summary>
        /// <param name="entity"></param>
        public void Delete<T>(T entity) where T : class
        {
            logger.Trace("Delete<T>");
            try
            {
                db.Delete(entity);
            }
            catch (Exception e)
            {
                logger.Error(e, $"Error occurred when removing {nameof(T)} - {e.Message}");
                throw;
            }
        }

        #endregion IPersistableRepository
    }
}