using System.Linq;
using System.Threading.Tasks;
using TableTopInc.API.Public.Functions.General;
using Xunit;

namespace TableTopInc.API.Test.Public.Functions.General
{
    public class GamesTest
    {
        [Fact]
        public async Task GamesShouldReturnData()
        {
            var data = await Games.RunAsync(null, null);

            Assert.True(data.Any());
        }
    }
}