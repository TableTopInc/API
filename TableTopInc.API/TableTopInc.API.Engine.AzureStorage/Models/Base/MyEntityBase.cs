using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.Models.Base;

namespace TableTopInc.API.Engine.AzureStorage.Models.Base
{
    public abstract class MyEntityBase : EntityBase, IMyEntityModel
    {
        [IgnoreProperty]
        public string OwnerId
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        protected MyEntityBase()
        {
        }
    }
}