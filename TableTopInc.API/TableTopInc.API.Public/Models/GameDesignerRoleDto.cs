using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Public.Models
{
    public class GameDesignerRoleDto : IGameDesignerRoleModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}