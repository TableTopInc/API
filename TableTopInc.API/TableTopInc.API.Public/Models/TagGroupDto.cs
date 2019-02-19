using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Public.Models
{
    public class TagGroupDto : ITagGroupModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }
}