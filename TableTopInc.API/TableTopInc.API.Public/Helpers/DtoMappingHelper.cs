using AutoMapper;
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Public.Models;

namespace TableTopInc.API.Public.Helpers
{
    public static class DtoMappingHelper
    {
        static DtoMappingHelper()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<IGameModel, GameDto>();
            });
        }

        public static GameDto ToDto(this IGameModel model)
        {
            return Mapper.Map<GameDto>(model);
        }
    }
}