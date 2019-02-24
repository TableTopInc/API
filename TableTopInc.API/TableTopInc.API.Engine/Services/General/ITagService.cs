using System.Collections.Generic;
using System.Threading.Tasks;
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Engine.Services.Base;

namespace TableTopInc.API.Engine.Services.General
{
    public interface ITagService : IEntityService<ITagModel>
    {
        Task<IEnumerable<ITagModel>> GetByTagGroupIdAsync(string id);
    }
}