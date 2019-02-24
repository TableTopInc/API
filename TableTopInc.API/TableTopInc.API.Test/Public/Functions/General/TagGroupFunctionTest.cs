using System.Linq;
using System.Threading.Tasks;
using TableTopInc.API.Public.Functions.General;
using TableTopInc.API.Public.Models;
using TableTopInc.API.Test.Mock.Services;
using Xunit;

namespace TableTopInc.API.Test.Public.Functions.General
{
    public class TagGroupFunctionTest
    {
        [Fact]
        public async Task DeleteTestAsync()
        {
            // given
            var tagServiceMock = new TagServiceMock();
            var tagGroupServiceMock = new TagGroupServiceMock();

            TagGroupFunction.TagService = table => tagServiceMock;
            TagGroupFunction.TagGroupService = table => tagGroupServiceMock;
            
            // when 1
            await tagGroupServiceMock.SaveAsync(new TagGroupDto { Id = "1" });
            await tagServiceMock.SaveAsync(new TagDto { Id = "a", TagGroupId = "1" });
            await tagServiceMock.SaveAsync(new TagDto { Id = "b", TagGroupId = "1" });
            await tagServiceMock.SaveAsync(new TagDto { Id = "c", TagGroupId = "1" });

            // then 1
            var tagGroups = await TagGroupFunction.GetAllAsync(null, null, null, null);
            
            Assert.Single(tagGroups);
            Assert.Equal(3, tagGroups.Single().Tags.Count);
            
            // when 2
            await TagGroupFunction.DeleteByIdAsync(null, "1", null, null, null);
            
            // then 2
            tagGroups = await TagGroupFunction.GetAllAsync(null, null, null, null);
            
            Assert.Empty(tagGroups);
            
            // then 3
            var tags = await tagServiceMock.GetAllAsync();
            
            Assert.Empty(tags);
        }
    }
}