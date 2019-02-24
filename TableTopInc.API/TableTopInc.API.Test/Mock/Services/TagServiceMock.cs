using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Engine.Services.General;
using TableTopInc.API.Test.Mock.Services.Base;

namespace TableTopInc.API.Test.Mock.Services
{
    public class TagServiceMock : ServiceBaseMock<ITagModel>, ITagService
    {
        public async Task<IEnumerable<ITagModel>> GetByTagGroupIdAsync(string id)
        {
            return _storage
                .Where(x => x.TagGroupId == id)
                .ToList();
        }
    }
}