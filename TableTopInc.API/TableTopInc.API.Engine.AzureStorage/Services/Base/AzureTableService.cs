using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.AzureStorage.Helpers;
using TableTopInc.API.Engine.AzureStorage.Models.Base;
using TableTopInc.API.Engine.Models.Base;
using TableTopInc.API.Engine.Services.Base;

namespace TableTopInc.API.Engine.AzureStorage.Services.Base
{
    public abstract class AzureTableService<StorageModel, EntityModel> : IEntityService<EntityModel>
        where StorageModel : TableStorageEntityBase, ITableEntity, new()
        where EntityModel : class, IEntityModel
    {
        public const int BatchMaxSize = 100;
        public const int QueryMaxParams = 15;
        public const int QueryMaxParamsForSinglePartition = QueryMaxParams - 1;
        
        public const string PartitionKeyPropertyName = nameof(TableEntity.PartitionKey);
        public const string RowKeyPropertyName = nameof(TableEntity.RowKey);
        
        protected readonly CloudTable Table;
        protected readonly string EntitiesOwnerId;

        internal AzureTableService(CloudTable table, string entitiesOwnerId = TableStorageEntityBase.DefaultPartitionKey)
        {
            Table = table;
            EntitiesOwnerId = entitiesOwnerId;
        }

        #region public
        
        public async Task<IEnumerable<EntityModel>> SaveAsync(params EntityModel[] entities)
        {
            foreach (var entity in entities.Where(x => string.IsNullOrWhiteSpace(x.Id)))
            {
                entity.Id = ToRowKey(Guid.NewGuid());
            }
            
            await ExecuteBatchAsync(entities.Select(x => x.ToStorageModel<StorageModel>()), TableOperation.InsertOrReplace);

            return entities;
        }

        public async Task DeleteByIdsAsync(params string[] ids)
        {
            var keys = ids
                .Select(x => new KeyValuePair<string, string>(EntitiesOwnerId, x))
                .ToArray();

            await DeleteByKeysAsync(keys);
        }

        public async Task<IEnumerable<EntityModel>> GetAllAsync()
        {
            return (await GetByFilterAsync(string.Empty))
                .Select(x => x.ToEntityModel<EntityModel>());
        }

        public async Task<IEnumerable<EntityModel>> GetByIdsAsync(params string[] ids)
        {
            var keys = ids
                .Select(x => new KeyValuePair<string, string>(EntitiesOwnerId, x))
                .ToArray();

            return (await GetByKeysAsync(keys))
                .Select(x => x.ToEntityModel<EntityModel>());
        }

        #endregion
        
        #region protected
        
        protected static string ToRowKey(Guid id)
        {
            return id.ToString("N");
        }
        
        protected async Task ExecuteBatchAsync(IEnumerable<StorageModel> entities, Func<StorageModel, TableOperation> func)
        {
            var groups = entities
                .GroupBy(x => x.PartitionKey, (key, items) => items.ToList());
            
            var tasks = new List<Task>();
            foreach (var entityGroup in groups)
            {
                var chunks = Math.Ceiling((decimal)entityGroup.Count / BatchMaxSize);
                for (var i = 0; i < chunks; i++)
                {
                    var tableBatch = new TableBatchOperation();
                    foreach (var entity in entityGroup.Skip(BatchMaxSize * i).Take(BatchMaxSize))
                    {
                        tableBatch.Add(func(entity));
                    }
                    tasks.Add(Table.ExecuteBatchAsync(tableBatch));
                }
            }

            await Task.WhenAll(tasks);
        }

        protected async Task DeleteByKeysAsync(params KeyValuePair<string, string>[] keys)
        {
            var entities = await GetByKeysAsync(keys);

            await ExecuteBatchAsync(entities, TableOperation.Delete);
        }
        
        protected async Task<IEnumerable<StorageModel>> GetByKeysAsync(params KeyValuePair<string, string>[] keys)
        {
            var partitions = keys
                .GroupBy(x => x.Key, (key, values) => values.ToList());

            var tasks = new List<Task<IEnumerable<StorageModel>>>();
            foreach (var rowKeys in partitions)
            {
                var chunks = Math.Ceiling((decimal)rowKeys.Count / QueryMaxParamsForSinglePartition);
                for (var i = 0; i < chunks; i++)
                {
                    var queryString = TableQuery.GenerateFilterCondition(
                        PartitionKeyPropertyName,
                        QueryComparisons.Equal,
                        rowKeys.First().Key);

                    queryString = rowKeys
                        .Skip(QueryMaxParamsForSinglePartition * i)
                        .Take(QueryMaxParamsForSinglePartition)
                        .Aggregate(queryString,
                            (currentFilter, newFilter) => TableQuery.CombineFilters(
                                currentFilter,
                                TableOperators.And,
                                TableQuery.GenerateFilterCondition(
                                    RowKeyPropertyName,
                                    QueryComparisons.Equal,
                                    newFilter.Value)));
                    
                    tasks.Add(GetByFilterAsync(queryString));
                }
            }

            await Task.WhenAll(tasks);

            return tasks
                .SelectMany(x => x.Result);
        }
        
        protected async Task<IEnumerable<StorageModel>> GetByFilterAsync(string filter)
        {
            var query = new TableQuery<StorageModel>().Where(filter);
            var results = new List<StorageModel>();
            
            TableContinuationToken token = null;
            do
            {
                var segment = await Table.ExecuteQuerySegmentedAsync(query, token);
                token = segment.ContinuationToken;
                results.AddRange(segment.Results);
            }
            while (token != null);

            return results;
        }
        
        #endregion
        
    }
}