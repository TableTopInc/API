using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TableTopInc.API.Engine.Models.Base;

namespace TableTopInc.API.Engine.Services.Base
{
    public interface IEntityService<T> where T : IEntityModel
    {
        Task<IEnumerable<T>> SaveAsync(params T[] entities);
        Task DeleteByIdsAsync(params string[] ids);
        
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetByIdsAsync(params string[] ids);
    }
}