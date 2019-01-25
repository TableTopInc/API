using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Public.Models
{
    public class GameDesignerDto : IGameDesignerModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string PhotoUrl { get; set; }
    }
}