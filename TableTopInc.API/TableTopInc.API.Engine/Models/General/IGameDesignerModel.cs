using TableTopInc.API.Engine.Models.Base;

namespace TableTopInc.API.Engine.Models.General
{
    public interface IGameDesignerModel : IEntityModel
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Bio { get; set; }

        string PhotoUrl { get; set; }
    }
}