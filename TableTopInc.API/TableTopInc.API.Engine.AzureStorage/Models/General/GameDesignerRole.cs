using TableTopInc.API.Engine.AzureStorage.Models.Base;
using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Engine.AzureStorage.Models.General
{
    public class GameDesignerRole : EntityBase, IGameDesignerRoleModel
    {
        public string Title { get; set; }
    }
}
