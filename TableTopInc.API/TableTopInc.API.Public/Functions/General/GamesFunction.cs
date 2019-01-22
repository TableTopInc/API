using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using TableTopInc.API.Engine.AzureStorage.Models.General;
using TableTopInc.API.Engine.AzureStorage.Services.General;
using TableTopInc.API.Engine.Services.General;
using TableTopInc.API.Public.Helpers;
using TableTopInc.API.Public.Models;

namespace TableTopInc.API.Public.Functions.General
{
    public static class GamesFunction
    {
        private const string Prefix = "Games";
        
        public static Func<CloudTable, IGameService> ResolveService = table => new GameService(table);
        
        [FunctionName(Prefix + "-GetAll")]
        public static async Task<IEnumerable<GameDto>> GetAllAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Prefix)]
            HttpRequest req,
            [Table(GameService.TableName, Connection = Const.StorageAccountConnectionName)]CloudTable table,
            ILogger log)
        {
            var service = ResolveService(table);
            
            var entities = (await service.GetAllAsync())
                .Select(DtoMappingHelper.ToDto);

            return entities;
        }
        
        [FunctionName(Prefix + "-GetById")]
        public static async Task<GameDto> GetByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Prefix + "/{id}")]
            HttpRequest req,
            string id,
            [Table(GameService.TableName, Connection = Const.StorageAccountConnectionName)]CloudTable table,
            ILogger log)
        {
            var service = ResolveService(table);

            var entity = (await service.GetByIdsAsync(id))
                .SingleOrDefault();
            
            return entity.ToDto();
        }
        
        [FunctionName(Prefix + "-Save")]
        public static async Task<GameDto> SaveAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Prefix)]
            [FromBody]GameDto model,
            [Table(GameService.TableName, Connection = Const.StorageAccountConnectionName)]CloudTable table,
            ILogger log)
        {
            var service = ResolveService(table);

            var entity = (await service.SaveAsync(GameTableEntity.Create(model)))
                .Single();

            return entity.ToDto();
        }
        
        [FunctionName(Prefix + "-DeleteById")]
        public static async Task DeleteByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Prefix + "/{id}")]
            HttpRequest req,
            string id,
            [Table(GameService.TableName, Connection = Const.StorageAccountConnectionName)]CloudTable table,
            ILogger log)
        {
            var service = ResolveService(table);

            await service.DeleteByIdsAsync(id);
        }
    }
}
