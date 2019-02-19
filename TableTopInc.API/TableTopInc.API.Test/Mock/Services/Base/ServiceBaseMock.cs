using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TableTopInc.API.Engine.Models.Base;
using TableTopInc.API.Engine.Services.Base;

namespace TableTopInc.API.Test.Mock.Services.Base
{
    public abstract class ServiceBaseMock<T> : IEntityService<T> where T : IEntityModel
    {
        private List<T> _storage = new List<T>();
        
        public async Task<IEnumerable<T>> SaveAsync(params T[] entities)
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

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Task.FromResult(_storage);
        }

        public async Task<IEnumerable<T>> GetByIdsAsync(params string[] ids)
        {
            var data = _storage.Where(x => ids.Contains(x.Id));
            
            return await Task.FromResult(data);
        }
    }
}