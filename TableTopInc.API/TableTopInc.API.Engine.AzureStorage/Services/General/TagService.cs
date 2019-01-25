using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.AzureStorage.Models.General;
using TableTopInc.API.Engine.AzureStorage.Services.Base;
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Engine.Services.General;

namespace TableTopInc.API.Engine.AzureStorage.Services.General
{
    public class TagService : AzureTableService<TagTableStorageEntity, ITagModel>, ITagService
    {
        public TagService(CloudTable table)
            : base(table)
        {
        }
    }
}
