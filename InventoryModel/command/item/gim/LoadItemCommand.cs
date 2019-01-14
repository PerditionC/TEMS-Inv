// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;

using TEMS.InventoryModel.entity.db;
using TEMS.InventoryModel.entity.db.query;
using TEMS.InventoryModel.util;

namespace TEMS.InventoryModel.command.action
{
    /// <summary>
    /// Retrieves given item using primary key from db then passes to callback.
    /// </summary>
    public class LoadItemCommand : RelayCommand
    {
        public LoadItemCommand() : base()
        {
            // can only be passed to base if static, we need instance data for callback so set here
            _execute = LoadItem;
            _canExecute = IsValidParameters;
        }

        /// <summary>
        /// the most recently loaded item
        /// </summary>
        public ItemBase ItemLoaded { get { return _itemLoaded; } set { SetProperty(ref _itemLoaded, value, nameof(ItemLoaded)); } }

        private ItemBase _itemLoaded = null;

        /// <summary>
        /// returns true if results of prior search are passed in as parameter for which entity to load
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static bool IsValidParameters(object parameters)
        {
            if (parameters is SearchResult searchResult)
            {
                return (searchResult.id != null) && (searchResult.id != Guid.Empty) && (!string.IsNullOrEmpty(searchResult.entityType));
            }
            logger.Warn($"{nameof(LoadItemCommand.IsValidParameters)} invoked with invalid parameter, must pass in SearchResult object!");
            return false;
        }

        private void LoadItem(object parameters)
        {
            if (parameters is SearchResult searchResult)
            {
                // perform the load, note may throw Exception if unable to load value
                ItemLoaded = DataRepository.GetDataRepository.Load(searchResult.id, searchResult.entityType) as ItemBase;
            }
        }
    }
}