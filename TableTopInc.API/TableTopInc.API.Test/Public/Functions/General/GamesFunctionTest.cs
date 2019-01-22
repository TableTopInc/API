using System.Linq;
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

            GamesFunction.ResolveService = table => mock;                        
            
            // when
            var entities = await GamesFunction.GetAllAsync(null, null, null);

            // then
            Assert.Equal(1, entities.Count());
        }
        
        [Fact]
        public async Task SaveTest()
        {
            // given
            var mock = new GameServiceMock();

            GamesFunction.ResolveService = table => mock;                        
            
            // when 1
            var entities = await GamesFunction.GetAllAsync(null, null, null);

            // then 1
            Assert.Equal(0, entities.Count());
            
            // when 2
            var entity = await GamesFunction.SaveAsync(new GameDto(), null, null);
            entities = await GamesFunction.GetAllAsync(null, null, null);
            
            // then 2
            Assert.False(string.IsNullOrWhiteSpace(entity.Id));
            Assert.Equal(1, entities.Count());
        }
    }
}