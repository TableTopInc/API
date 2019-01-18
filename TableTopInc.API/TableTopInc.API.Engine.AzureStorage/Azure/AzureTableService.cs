using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableTopInc.API.Engine.AzureStorage.Azure
{
    public class AzureTableService<T> where T : TableEntity, new()
    {
        public const int BatchMaxSize = 100;
        public const int QueryMaxParams = 15;
        public const int QueryMaxParamsForSinglePartition = QueryMaxParams - 1;
        
        public const string PartitionKeyPropertyName = nameof(TableEntity.PartitionKey);
        public const string RowKeyPropertyName = nameof(TableEntity.RowKey);
        
        protected readonly CloudTable Table;
        protected readonly string EntitiesOwnerId;

        internal AzureTableService(CloudTable table, string entitiesOwnerId)
        {
            Table = table;
            EntitiesOwnerId = entitiesOwnerId;
        }

        #region public
        
        public static string ToRowKey(Guid id)
        {
            return id.ToString("N");
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await GetByFilterAsync(string.Empty);
        }
        
        public virtual async Task<IEnumerable<T>> GetByIdsAsync(params Guid[] ids)
        {
            var keys = ids
                .Select(x => new KeyValuePair<string, string>(
                    EntitiesOwnerId,
                    ToRowKey(x)))
                .ToArray();

            return await GetByKeysAsync(keys);
        }
        
        public virtual async Task DeleteByIdsAsync(params Guid[] ids)
        {
            var keys = ids
                .Select(x => new KeyValuePair<string, string>(
                    EntitiesOwnerId,
                    ToRowKey(x)))
                .ToArray();

            await DeleteByKeysAsync(keys);
        }
        
        public async Task SaveAsync(params T[] entities)
        {
            await ExecuteBatchAsync(entities, TableOperation.InsertOrReplace);
        }
        
        #endregion
        
        #region protected
        
        protected async Task ExecuteBatchAsync(IEnumerable<T> entities, Func<T, TableOperation> func)
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
        
        protected async Task<IEnumerable<T>> GetByKeysAsync(params KeyValuePair<string, string>[] keys)
        {
            var partitions = keys
                .GroupBy(x => x.Key, (key, values) => values.ToList());

            var tasks = new List<Task<IEnumerable<T>>>();
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

            return tasks.SelectMany(x => x.Result);
        }
        
        protected async Task<IEnumerable<T>> GetByFilterAsync(string filter)
        {
            var query = new TableQuery<T>().Where(filter);
            var results = new List<T>();
            
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