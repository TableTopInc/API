using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosDB.Table;
using TableTopInc.API.Engine.AzureStorage.Azure;
using TableTopInc.API.Engine.AzureStorage.Models.Base;
using TableTopInc.API.Engine.Services.Base;

namespace TableTopInc.API.Engine.AzureStorage.Services.Base
{
    public abstract class EntityServiceBase<T> : IEntityService<T> where T : EntityBase, new()
    {
        internal AzureTableService<T> AzureTableService;

        protected EntityServiceBase(CloudTable table)
        {
            AzureTableService = new AzureTableService<T>(table, EntityBase.DefaultPartitionKey);
        }
        
        public async Task SaveAsync(params T[] entities)
        {
            await AzureTableService.SaveAsync(entities);
        }

        public async Task DeleteByIdsAsync(params Guid[] ids)
        {
            await AzureTableService.DeleteByIdsAsync(ids);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await AzureTableService.GetAllAsync();
        }

        public async Task<IEnumerable<T>> GetByIdsAsync(params Guid[] ids)
        {
            return await AzureTableService.GetByIdsAsync(ids);
        }
    }
}