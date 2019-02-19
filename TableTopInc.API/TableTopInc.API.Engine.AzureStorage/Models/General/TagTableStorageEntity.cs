using TableTopInc.API.Engine.AzureStorage.Helpers;
using TableTopInc.API.Engine.AzureStorage.Models.Base;
using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Engine.AzureStorage.Models.General
{
    public class TagTableStorageEntity : TableStorageEntityBase, ITagModel
    {
        public static TagTableStorageEntity Create(ITagModel model)
        {
            return model.ToStorageModel<TagTableStorageEntity>();
        }
        
        public string Title { get; set; }
        
        public string TagGroupId { get; set; }
    }
}
