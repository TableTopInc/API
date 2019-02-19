using System;
using AutoMapper;
using TableTopInc.API.Engine.Models.Base;
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
                cfg.CreateMap<ITagModel, TagDto>();
                cfg.CreateMap<ITagModel, TagDtoExtended>();
                cfg.CreateMap<ITagGroupModel, TagGroupDto>();
                cfg.CreateMap<ITagGroupModel, TagGroupDtoExtended>();
            });

            return config.CreateMapper();
        });

        internal static T ToDto<T>(this IEntityModel model)
        {
            return ObjMapper.Value.Map<T>(model);
        }
    }
}