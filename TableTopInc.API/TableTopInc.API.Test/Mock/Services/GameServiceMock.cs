using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableTopInc.API.Engine.Models.General;
using TableTopInc.API.Engine.Services.General;

namespace TableTopInc.API.Test.Mock.Services
{
    internal class GameServiceMock : IGameService
    {
        private List<IGameModel> _storage = new List<IGameModel>();

        public async Task<IEnumerable<IGameModel>> SaveAsync(params IGameModel[] entities)
        {
            foreach (var entity in entities.Where(x => string.IsNullOrWhiteSpace(x.Id)))
            {
                entity.Id = Guid.NewGuid().ToString("N");
            }
            
            _storage.AddRange(entities);

            return entities;
        }

        public async Task DeleteByIdsAsync(params string[] ids)
        {
            _storage = _storage
                .Where(x => !ids.Contains(x.Id))
                .ToList();
        }

        public async Task<IEnumerable<IGameModel>> GetAllAsync()
        {
            return await Task.FromResult(_storage);
        }

        public async Task<IEnumerable<IGameModel>> GetByIdsAsync(params string[] ids)
        {
            var data = _storage.Where(x => ids.Contains(x.Id));
            
            return await Task.FromResult(data);
        }

        public Task<IEnumerable<IGameModel>> GetGamesByTagsAsync(params string[] tags)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IGameModel>> GetGamesByGameDesignersAsync(params string[] authors)
        {
            throw new NotImplementedException();
        }
    }
}