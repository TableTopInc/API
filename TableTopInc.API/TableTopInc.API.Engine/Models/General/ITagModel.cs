using TableTopInc.API.Engine.Models.Base;

namespace TableTopInc.API.Engine.Models.General
{
    public interface ITagModel : IEntityModel
    {
        string Title { get; set; }
    }
}