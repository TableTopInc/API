using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.AzureStorage.Models.General;
using TableTopInc.API.Engine.AzureStorage.Services.Base;

namespace TableTopInc.API.Engine.AzureStorage.Services.General
{
    public class TagService : EntityServiceBase<TagTableEntity>
    {
        public TagService(CloudTable table)
            : base(table)
        {
        }
    }
}
