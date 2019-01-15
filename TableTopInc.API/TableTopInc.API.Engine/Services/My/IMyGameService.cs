using System.Collections.Generic;
using System.Threading.Tasks;
using TableTopInc.API.Engine.Models.My;
using TableTopInc.API.Engine.Services.Base;

namespace TableTopInc.API.Engine.Services.My
{
    public interface IMyGameService : IEntityService<IMyGameModel>
    {
        Task<IEnumerable<IMyGameModel>> GetGamesByOwnersAsync(params string[] owners);
    }
}