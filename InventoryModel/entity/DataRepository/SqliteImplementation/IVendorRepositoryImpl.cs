// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TEMS.InventoryModel.entity.db
{
    /// <summary>
    /// Interface to database entities - all queries, updates, etc. handled via an instance of this class.
    /// Uses Database class to perform actual interaction with back-end database.
    /// </summary>
    public sealed partial class DataRepository : IVendorRepository
    {
        #region vendor

        /// <summary>
        /// return a list of all item types for provided vendor (using vendorId)
        /// </summary>
        /// <param name="vendorId"></param>
        /// <returns></returns>
        public ObservableCollection<ItemBase> GetVendorItemTypes(Guid vendorId)
        {
            //return DataRepository.GetDataRepository.db.LoadRows<ItemType>("WHERE vendorId=?;", vendorId.ToString());
            return new ObservableCollection<ItemBase>((GetDataRepository.ReferenceData[nameof(ItemType)]).Where(x => (x as ItemType).vendorId == vendorId));
        }

        #endregion vendor
    }
}