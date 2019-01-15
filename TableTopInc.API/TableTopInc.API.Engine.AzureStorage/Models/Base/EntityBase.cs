using System;
using Microsoft.Azure.CosmosDB.Table;
using TableTopInc.API.Engine.AzureStorage.Azure;
using TableTopInc.API.Engine.Models.Base;

namespace TableTopInc.API.Engine.AzureStorage.Models.Base
{
    public abstract class EntityBase : TableEntity, IEntityModel
    {
        public const string DefaultPartitionKey = "__shared";

        [IgnoreProperty]
        public Guid Id
        {
            get => Guid.Parse(RowKey);
            set => RowKey = AzureTableService<TableEntity>.ToRowKey(value);
        }

        protected EntityBase()
        {
            PartitionKey = DefaultPartitionKey;
        }
    }
}