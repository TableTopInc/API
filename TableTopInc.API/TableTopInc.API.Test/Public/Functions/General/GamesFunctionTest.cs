using System.Threading.Tasks;
using TableTopInc.API.Public.Functions.General;
using TableTopInc.API.Public.Models;
using TableTopInc.API.Test.Mock.Services;
using Xunit;

namespace TableTopInc.API.Test.Public.Functions.General
{
    public class GamesFunctionTest
    {
        [Fact]
        public async Task GetAllTest()
        {
            // given
            var mock = new GameServiceMock();
            await mock.SaveAsync(new GameDto());

            GameFunction.ResolveService = table => mock;                        
            
            // when
            var entities = await GameFunction.GetAllAsync(null, null, null);

            // then
            Assert.Single(entities);
        }
        
        [Fact]
        public async Task SaveTest()
        {
            // given
            var mock = new GameServiceMock();

            GameFunction.ResolveService = table => mock;                        
            
            // when 1
            var entities = await GameFunction.GetAllAsync(null, null, null);

            // then 1
            Assert.Empty(entities);
            
            // when 2
            var entity = await GameFunction.SaveAsync(new GameDto(), null, null);
            entities = await GameFunction.GetAllAsync(null, null, null);
            
            // then 2
            Assert.False(string.IsNullOrWhiteSpace(entity.Id));
            Assert.Single(entities);
        }
    }
}