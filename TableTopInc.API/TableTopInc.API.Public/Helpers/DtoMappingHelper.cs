using System;
using AutoMapper;
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Public.Models;

namespace TableTopInc.API.Public.Helpers
{
    internal static class DtoMappingHelper
    {
        private static readonly Lazy<IMapper> ObjMapper = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IGameModel, GameDto>();
                cfg.CreateMap<IGameDesignerModel, GameDesignerDto>();
                cfg.CreateMap<IGameDesignerRoleModel, GameDesignerRoleDto>();
            });

            return config.CreateMapper();
        });

        internal static GameDto ToDto(this IGameModel model)
        {
            return ObjMapper.Value.Map<GameDto>(model);
        }
        
        internal static GameDesignerDto ToDto(this IGameDesignerModel model)
        {
            return ObjMapper.Value.Map<GameDesignerDto>(model);
        }
        
        internal static GameDesignerRoleDto ToDto(this IGameDesignerRoleModel model)
        {
            return ObjMapper.Value.Map<GameDesignerRoleDto>(model);
        }
    }
}