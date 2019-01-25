using TableTopInc.API.Engine.AzureStorage.Helpers;
using TableTopInc.API.Engine.AzureStorage.Models.Base;
using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Engine.AzureStorage.Models.General
{
    public class GameDesignerRoleTableStorageEntity : TableStorageEntityBase, IGameDesignerRoleModel
    {
        public static GameDesignerRoleTableStorageEntity Create(IGameDesignerRoleModel model)
        {
            return model.ToStorageModel<GameDesignerRoleTableStorageEntity>();
        }
        
        public string Title { get; set; }
    }
}
