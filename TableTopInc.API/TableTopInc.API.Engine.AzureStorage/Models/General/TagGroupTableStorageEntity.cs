using TableTopInc.API.Engine.AzureStorage.Helpers;
using TableTopInc.API.Engine.AzureStorage.Models.Base;
using TableTopInc.API.Engine.Models.General;

namespace TableTopInc.API.Engine.AzureStorage.Models.General
{
    public class TagGroupTableStorageEntity : TableStorageEntityBase, ITagGroupModel
    {
        public static TagGroupTableStorageEntity Create(ITagGroupModel model)
        {
            return model.ToStorageModel<TagGroupTableStorageEntity>();
        }
        
        public string Title { get; set; }
    }
}