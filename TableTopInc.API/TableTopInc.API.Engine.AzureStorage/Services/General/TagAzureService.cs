using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.AzureStorage.Models.General;
using TableTopInc.API.Engine.AzureStorage.Services.Base;
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Engine.Services.General;

namespace TableTopInc.API.Engine.AzureStorage.Services.General
{
    public class TagAzureService : AzureTableStorageServiceBase<TagTableStorageEntity, ITagModel>, ITagService
    {
        public const string TableName = "Tags";
        
        public TagAzureService(CloudTable table)
            : base(table)
        {
        }

        public async Task<IEnumerable<ITagModel>> GetByTagGroupIdAsync(string id)
        {
            var filter = TableQuery.GenerateFilterCondition(
                nameof(TagTableStorageEntity.TagGroupId), 
                QueryComparisons.Equal,
                id);
            
            return await GetByFilterAsync(filter);
        }
    }
}
