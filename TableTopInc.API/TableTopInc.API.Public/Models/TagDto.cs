using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Public.Models
{
    public class TagDto : ITagModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string TagGroupId { get; set; }
    }
}