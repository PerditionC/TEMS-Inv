﻿// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using AutoMapper;
using TEMS.InventoryModel.entity.db;
using TEMS_Inventory.views;

namespace InventoryViewModel
{
    public static class Mapper
    {
        private static MapperConfiguration config = new MapperConfiguration(cfg => {
            cfg.CreateMap<ItemType, ItemTypeManagementViewModel>(MemberList.Source).ForMember(dest => dest.guid, opt => opt.MapFrom(src => src.id)).ReverseMap();
            cfg.CreateMap<Item, ItemManagementViewModel>(MemberList.Source).ForMember(dest => dest.guid, opt => opt.MapFrom(src => src.id)).ReverseMap();
            cfg.CreateMap<ItemInstance, GeneralInventoryManagementViewModel>(MemberList.Source).ForMember(dest => dest.guid, opt => opt.MapFrom(src => src.id)).ReverseMap();
        });

        private static IMapper mapper = null;

        public static IMapper GetMapper()
        {
            if (mapper == null) mapper = config.CreateMapper();
            return mapper;
        }
    }
}
