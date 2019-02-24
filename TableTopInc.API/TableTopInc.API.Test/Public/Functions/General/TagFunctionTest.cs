using System;
using System.Threading.Tasks;
using TableTopInc.API.Public.Functions.General;
using TableTopInc.API.Public.Models;
using TableTopInc.API.Test.Mock.Services;
using Xunit;

namespace TableTopInc.API.Test.Public.Functions.General
{
    public class TagFunctionTest
    {
        [Fact]
        public async Task SaveTestAsync()
        {
            // given
            var tagServiceMock = new TagServiceMock();
            var tagGroupServiceMock = new TagGroupServiceMock();

            TagFunction.TagService = table => tagServiceMock;
            TagFunction.TagGroupService = table => tagGroupServiceMock;
            
            // when
            Func<Task> action = () => TagFunction.SaveAsync(new TagDto(), null, null, null);
            
            // then
            await Assert.ThrowsAsync<ArgumentException>(action);
        }
    }
}