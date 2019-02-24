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
    public class TagGroupFunction
    {
        private const string Prefix = "TagGroups";
        private const string TagTableName = TagAzureService.TableName;
        private const string TagGroupTableName = TagGroupAzureService.TableName;
        
        public static Func<CloudTable, ITagService> TagService = table => new TagAzureService(table);
        public static Func<CloudTable, ITagGroupService> TagGroupService = table => new TagGroupAzureService(table);
        
        [FunctionName(Prefix + "-GetAll")]
        public static async Task<IEnumerable<TagGroupDtoExtended>> GetAllAsync(
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
            
            var entities = tagGroupsTask.Result
                .Select(x =>
                {
                    var tagGroup = x.ToDto<TagGroupDtoExtended>();
                    
                    tagGroup.Tags = tagsTask.Result
                        .Where(i => i.TagGroupId == tagGroup.Id)
                        .Select(DtoMappingHelper.ToDto<TagDto>)
                        .ToList();

                    return tagGroup;
                });

            return entities;
        }
        
        [FunctionName(Prefix + "-GetById")]
        public static async Task<TagGroupDtoExtended> GetByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Prefix + "/{id}")]
            HttpRequest req,
            string id,
            [Table(TagTableName, Connection = Const.StorageAccountConnectionName)]CloudTable tagTable,
            [Table(TagGroupTableName, Connection = Const.StorageAccountConnectionName)]CloudTable tagGroupTable,
            ILogger log)
        {
            var tagGroupService = TagGroupService(tagGroupTable);

            var entity = (await tagGroupService.GetByIdsAsync(id))
                .SingleOrDefault();

            if (entity != null)
            {
                var tagService = TagService(tagTable);

                var tagGroup = entity.ToDto<TagGroupDtoExtended>();

                tagGroup.Tags = (await tagService.GetByTagGroupIdAsync(id))
                    .Select(DtoMappingHelper.ToDto<TagDto>)
                    .ToList();

                return tagGroup;
            }

            return null;
        }
        
        [FunctionName(Prefix + "-Save")]
        public static async Task<TagGroupDto> SaveAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Prefix)]
            [FromBody]TagGroupDto model,
            [Table(TagGroupTableName, Connection = Const.StorageAccountConnectionName)]CloudTable table,
            ILogger log)
        {
            var service = TagGroupService(table);

            var entity = (await service.SaveAsync(TagGroupTableStorageEntity.Create(model)))
                .Single();

            return entity.ToDto<TagGroupDto>();
        }
        
        [FunctionName(Prefix + "-DeleteById")]
        public static async Task DeleteByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Prefix + "/{id}")]
            HttpRequest req,
            string id,
            [Table(TagTableName, Connection = Const.StorageAccountConnectionName)]CloudTable tagTable,
            [Table(TagGroupTableName, Connection = Const.StorageAccountConnectionName)]CloudTable tagGroupTable,
            ILogger log)
        {
            var tagService = TagService(tagTable);
            var tagGroupService = TagGroupService(tagGroupTable);

            var tags = (await tagService.GetByTagGroupIdAsync(id))
                .Select(x => x.Id)
                .ToArray();

            await Task.WhenAll(
                tagGroupService.DeleteByIdsAsync(id),
                tagService.DeleteByIdsAsync(tags));
        }
    }
}