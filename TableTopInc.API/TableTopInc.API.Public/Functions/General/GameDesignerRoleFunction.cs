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
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Public.Helpers;
using TableTopInc.API.Public.Models;

namespace TableTopInc.API.Public.Functions.General
{
    public class GameDesignerRoleFunction
    {
        private const string Prefix = "GameDesignerRoles";
        
        [FunctionName(Prefix + "-GetAll")]
        public static async Task<IEnumerable<GameDesignerRoleDto>> GetAllAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Prefix)]
            HttpRequest req,
            [Table(GameDesignerRoleService.TableName, Connection = Const.StorageAccountConnectionName)]CloudTable rolesTable,
            ILogger log)
        {
            var service = new GameDesignerRoleService(rolesTable);

            var roles = (await service.GetAllAsync())
                .Select(DtoMappingHelper.ToDto);

            return roles;
        }
        
        [FunctionName(Prefix + "-GetById")]
        public static async Task<GameDesignerRoleDto> GetByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Prefix + "/{id}")]
            HttpRequest req,
            string id,
            [Table(GameDesignerRoleService.TableName, Connection = Const.StorageAccountConnectionName)]CloudTable rolesTable,
            ILogger log)
        {
            if (!Guid.TryParse(id, out var gameId))
            {
                throw new ArgumentException(nameof(id));
            }
            
            var service = new GameDesignerRoleService(rolesTable);

            var game = (await service.GetByIdsAsync(gameId))
                .SingleOrDefault();
            
            return game.ToDto();
        }
        
        [FunctionName(Prefix + "-Save")]
        public static async Task<GameDesignerRoleDto> SaveAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Prefix)]
            [FromBody]GameDesignerRoleDto model,
            [Table(GameDesignerRoleService.TableName, Connection = Const.StorageAccountConnectionName)]CloudTable rolesTable,
            ILogger log)
        {
            if (string.IsNullOrWhiteSpace(model.Id))
            {
                model.Id = AzureTableService<TableEntity>.ToRowKey(Guid.NewGuid());
            }
            
            var service = new GameDesignerRoleService(rolesTable);

            await service.SaveAsync(GameDesignerRoleTableEntity.Create(model));

            return model;
        }
        
        [FunctionName(Prefix + "-DeleteById")]
        public static async Task DeleteByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Prefix + "/{id}")]
            HttpRequest req,
            string id,
            [Table(GameDesignerRoleService.TableName, Connection = Const.StorageAccountConnectionName)]CloudTable rolesTable,
            ILogger log)
        {
            if (!Guid.TryParse(id, out var gameId))
            {
                throw new ArgumentException(nameof(id));
            }
            
            var service = new GameDesignerRoleService(rolesTable);

            await service.DeleteByIdsAsync(gameId);
        }
    }
}