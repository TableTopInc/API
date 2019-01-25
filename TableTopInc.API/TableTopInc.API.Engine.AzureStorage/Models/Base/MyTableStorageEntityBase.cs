using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.Models.Base;

namespace TableTopInc.API.Engine.AzureStorage.Models.Base
{
    public abstract class MyTableStorageEntityBase : TableStorageEntityBase, IMyEntityModel
    {
        [IgnoreProperty]
        public string OwnerId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        protected MyTableStorageEntityBase()
        {
        }
    }
}