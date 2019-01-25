using System;
using AutoMapper;
using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.AzureStorage.Models.General;
using TableTopInc.API.Engine.Models.Base;
using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Engine.AzureStorage.Helpers
{
    internal static class TableEntityMappingHelper
    {
        private static readonly Lazy<IMapper> ObjMapper = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IGameModel, GameTableStorageEntity>();
                cfg.CreateMap<GameTableStorageEntity, IGameModel>();
                
                cfg.CreateMap<IGameDesignerModel, GameDesignerTableStorageEntity>();
                cfg.CreateMap<GameDesignerTableStorageEntity, IGameDesignerModel>();
                
                cfg.CreateMap<IGameDesignerRoleModel, GameDesignerRoleTableStorageEntity>();
                cfg.CreateMap<GameDesignerRoleTableStorageEntity, IGameDesignerRoleModel>();
            });

            return config.CreateMapper();
        });

        internal static T ToStorageModel<T>(this IEntityModel model)
        {
            return ObjMapper.Value.Map<T>(model);
        }
        
        internal static T ToEntityModel<T>(this ITableEntity model)
        {
            return ObjMapper.Value.Map<T>(model);
        }
    }
}