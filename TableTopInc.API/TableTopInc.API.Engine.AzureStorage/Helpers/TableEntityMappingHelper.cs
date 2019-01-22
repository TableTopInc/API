using AutoMapper;
using TableTopInc.API.Engine.AzureStorage.Models.General;
using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Engine.AzureStorage.Helpers
{
    public static class TableEntityMappingHelper
    {
        static TableEntityMappingHelper()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<IGameModel, GameTableEntity>();
                config.CreateMap<IGameDesignerRoleModel, GameDesignerRoleTableEntity>();
            });
        }

        public static GameTableEntity ToTableEntity(this IGameModel model)
        {
            return Mapper.Map<GameTableEntity>(model);
        }
        
        public static GameDesignerRoleTableEntity ToTableEntity(this IGameDesignerRoleModel model)
        {
            return Mapper.Map<GameDesignerRoleTableEntity>(model);
        }
    }
}