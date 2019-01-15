namespace TableTopInc.API.Engine.Models.Base
{
    public interface IMyEntityModel : IEntityModel
    {
        string OwnerId { get; set; }
    }
}