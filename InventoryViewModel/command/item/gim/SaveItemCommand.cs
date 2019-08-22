// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.ComponentModel;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.util;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// Saves (inserts/updates) entity in the database.
    /// Parameter is a single object, the entity to save
    /// </summary>
    public class SaveItemCommand : RelayCommand
    {
        public SaveItemCommand() : base(UpdateItem, IsValidParameters)
        {
        }

        private static bool IsValidParameters(object entity)
        {
            // by default assume can always save
            var canSave = true;

            // if entity supports change tracking then disable save if no changes
            if (entity is IChangeTracking itemWithChangeTracking) canSave &= itemWithChangeTracking.IsChanged;

            // if entity supports Null constraint checking then disable save if constraint not satisfied
            if (entity is ItemBase itemWithNullConstraintCheck) canSave &= itemWithNullConstraintCheck.AreNonNullConstraintsSatisfied();

            return canSave;
        }

        private static void UpdateItem(object entity)
        {
            // perform the save, note may throw Exception if unable to save
            DataRepository.GetDataRepository.Save(entity);
        }
    }
}