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
    public static class TagFunction
    {
        private const string Prefix = "Tags";
        private const string TagTableName = TagAzureService.TableName;
        private const string TagGroupTableName = TagGroupAzureService.TableName;
        
        public static Func<CloudTable, ITagService> TagService = table => new TagAzureService(table);
        public static Func<CloudTable, ITagGroupService> TagGroupService = table => new TagGroupAzureService(table);
        
        [FunctionName(Prefix + "-GetAll")]
        public static async Task<IEnumerable<TagDtoExtended>> GetAllAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Prefix)]
            HttpRequest req,
            [Table(TagTableName, Connection = Const.StorageAccountConnectionName)]CloudTable tagTable,
            [Table(TagGroupTableName, Connection = Const.StorageAccountConnectionName)]CloudTable tagGroupTable,
            ILogger log)
        {
            var tagService = TagService(tagTable);
            var tagGroupService = TagGroupService(tagGroupTable);

            var tagsTask = tagService.GetAllAsync();
            var tagGroupsTask = tagGroupService.GetAllAsync();

            await Task.WhenAll(tagsTask, tagGroupsTask);

            var tagGroups = tagGroupsTask.Result
                .Select(DtoMappingHelper.ToDto<TagGroupDto>);

            var tags = tagsTask.Result
                .Select(x =>
                {
                    var tag = x.ToDto<TagDtoExtended>();

                    tag.TagGroup = tagGroups.Single(i => i.Id == tag.TagGroupId);
                    
                    return tag;
                });

            return tags;
        }
        
        [FunctionName(Prefix + "-GetById")]
        public static async Task<TagDtoExtended> GetByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Prefix + "/{id}")]
            HttpRequest req,
            string id,
            [Table(TagTableName, Connection = Const.StorageAccountConnectionName)]CloudTable tagTable,
            [Table(TagGroupTableName, Connection = Const.StorageAccountConnectionName)]CloudTable tagGroupTable,
            ILogger log)
        {
            var tagService = TagService(tagTable);
            
            var entity = (await tagService.GetByIdsAsync(id))
                .SingleOrDefault();

            if (entity != null)
            {
                var tagGroupService = TagGroupService(tagGroupTable);

                var tag = entity.ToDto<TagDtoExtended>();
                
                tag.TagGroup = (await tagGroupService.GetByIdsAsync(entity.TagGroupId))
                    .Single()
                    .ToDto<TagGroupDto>();

                return tag;
            }

            return null;
        }
        
        [FunctionName(Prefix + "-Save")]
        public static async Task<TagDto> SaveAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Prefix)]
            [FromBody]TagDto model,
            [Table(TagTableName, Connection = Const.StorageAccountConnectionName)]CloudTable tagTable,
            [Table(TagGroupTableName, Connection = Const.StorageAccountConnectionName)]CloudTable tagGroupTable,
            ILogger log)
        {
            var tagGroupService = TagGroupService(tagGroupTable);

            var tagGroup = (await tagGroupService.GetByIdsAsync(model.TagGroupId))
                .SingleOrDefault();
            
            if (tagGroup == null)
            {
                throw new ArgumentException();
            }
            
            var tagService = TagService(tagTable);

            var entity = (await tagService.SaveAsync(TagTableStorageEntity.Create(model)))
                .Single();

            return entity.ToDto<TagDto>();
        }
        
        [FunctionName(Prefix + "-DeleteById")]
        public static async Task DeleteByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Prefix + "/{id}")]
            HttpRequest req,
            string id,
            [Table(TagTableName, Connection = Const.StorageAccountConnectionName)]CloudTable tagTable,
            ILogger log)
        {
            var service = TagService(tagTable);

            await service.DeleteByIdsAsync(id);
        }
    }
}