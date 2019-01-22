using System;
using AutoMapper;
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
                cfg.CreateMap<IGameModel, GameTableEntity>();
                cfg.CreateMap<IGameDesignerRoleModel, GameDesignerRoleTableEntity>();
            });

            return config.CreateMapper();
        });

        internal static T ToTableEntity<T>(this IEntityModel model)
        {
            return ObjMapper.Value.Map<T>(model);
        }
    }
}