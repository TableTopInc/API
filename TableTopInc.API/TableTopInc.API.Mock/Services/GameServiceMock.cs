using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Engine.Services.General;

namespace TableTopInc.API.Mock.Services
{
    public class GameServiceMock : IGameService
    {
        private readonly List<IGameModel> _storage = new List<IGameModel>();
        
        public async Task SaveAsync(params IGameModel[] entities)
        {
            _storage.AddRange(entities);
        }

        public Task DeleteByIdsAsync(params Guid[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IGameModel>> GetAllAsync()
        {
            return await Task.FromResult(_storage);
        }

        public Task<IEnumerable<IGameModel>> GetByIdsAsync(params Guid[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IGameModel>> GetGamesByTagsAsync(params string[] tags)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IGameModel>> GetGamesByGameDesignersAsync(params Guid[] authors)
        {
            throw new NotImplementedException();
        }
    }
}