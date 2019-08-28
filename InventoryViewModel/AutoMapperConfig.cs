// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using TEMS.InventoryModel.entity.db;
using TEMS_Inventory.views;

namespace InventoryViewModel
{
    public static class Mapper
    {
        private static MapperConfiguration config = new MapperConfiguration(cfg => {
            cfg.CreateMap<ItemType, ItemTypeManagementViewModel>();
            cfg.CreateMap<Item, ItemManagementViewModel>();
            cfg.CreateMap<ItemInstance, ItemInstanceManagementViewModel>();
            cfg.CreateMap<ItemInstance, GeneralInventoryManagementViewModel>();
        });

        private static IMapper mapper = null;

        public static IMapper GetMapper()
        {
            if (mapper == null) mapper = config.CreateMapper();
            return mapper;
        }
    }
}
