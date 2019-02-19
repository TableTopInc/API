using System.Collections.Generic;
using System.Threading.Tasks;
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Engine.Services.Base;

namespace TableTopInc.API.Engine.Services.General
{
    public interface IGameService : IEntityService<IGameModel>
    {
        Task<IEnumerable<IGameModel>> GetGamesByTagsAsync(params string[] tags);
        Task<IEnumerable<IGameModel>> GetGamesByGameDesignersAsync(params string[] authors);
    }
}