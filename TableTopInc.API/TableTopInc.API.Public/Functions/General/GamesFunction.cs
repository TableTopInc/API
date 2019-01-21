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
using TableTopInc.API.Engine.AzureStorage.Azure;
using TableTopInc.API.Engine.AzureStorage.Models.General;
using TableTopInc.API.Engine.AzureStorage.Services.General;
using TableTopInc.API.Public.Helpers;
using TableTopInc.API.Public.Models;

namespace TableTopInc.API.Public.Functions.General
{
    public static class GamesFunction
    {
        private const string Prefix = "games";
        
        [FunctionName("Games-GetAll")]
        public static async Task<IEnumerable<GameDto>> GetAllAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Prefix)]
            HttpRequest req,
            [Table(GameService.TableName, Connection = "Storage")]CloudTable gamesTable,
            ILogger log)
        {
            var service = new GameService(gamesTable);
            
            var games = (await service.GetAllAsync())
                .Select(DtoMappingHelper.ToDto);

            return games;
        }
        
        [FunctionName("Games-GetById")]
        public static async Task<GameDto> GetByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Prefix + "/{gameId}")]
            HttpRequest req,
            string gameId,
            [Table(GameService.TableName, Connection = "Storage")]CloudTable gamesTable,
            ILogger log)
        {
            if (!Guid.TryParse(gameId, out var id))
            {
                throw new ArgumentException(nameof(gameId));
            }
            
            var service = new GameService(gamesTable);

            var game = (await service.GetByIdsAsync(id))
                .SingleOrDefault();
            
            return game.ToDto();
        }
        
        [FunctionName("Games-Save")]
        public static async Task<GameDto> SaveAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Prefix)]
            [FromBody]GameDto model,
            [Table(GameService.TableName, Connection = "Storage")]CloudTable gamesTable,
            ILogger log)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = AzureTableService<TableEntity>.ToRowKey(Guid.NewGuid());
            }
            
            var service = new GameService(gamesTable);

            await service.SaveAsync(GameTableEntity.Create(model));

            return model;
        }
        
        [FunctionName("Games-DeleteById")]
        public static async Task DeleteByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Prefix + "/{gameId}")]
            HttpRequest req,
            string gameId,
            [Table(GameService.TableName, Connection = "Storage")]CloudTable gamesTable,
            ILogger log)
        {
            if (!Guid.TryParse(gameId, out var id))
            {
                throw new ArgumentException(nameof(gameId));
            }
            
            var service = new GameService(gamesTable);

            await service.DeleteByIdsAsync(id);
        }
    }
}
