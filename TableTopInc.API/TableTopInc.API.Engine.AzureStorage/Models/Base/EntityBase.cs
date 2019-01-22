using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.Models.Base;

namespace TableTopInc.API.Engine.AzureStorage.Models.Base
{
    public abstract class EntityBase : TableEntity, IEntityModel
    {
        public const string DefaultPartitionKey = "__shared";
        
        [IgnoreProperty]
        public string Id
        {
            get => RowKey;
            set => RowKey = value;
        }

        protected EntityBase()
        {
            PartitionKey = DefaultPartitionKey;
        }
    }
}