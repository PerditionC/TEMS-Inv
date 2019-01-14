// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using TEMS.InventoryModel.entity.db.user;

namespace TEMS_Inventory.views
{
    /// <summary>
    /// message argument passed to indicate current User has changed
    /// </summary>
    public class CurrentUserChangedMessage
    {
        public UserDetail CurrentUser;
    }
}
