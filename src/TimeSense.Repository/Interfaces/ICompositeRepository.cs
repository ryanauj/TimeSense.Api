using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeSense.Repository.Interfaces
{
    public interface ICompositeRepository<in TUserId, in TId, in TEntityInput, TEntity>
    {
        Task<TEntity> Get(TUserId userId, TId id);
        Task<TEntity> Create(TUserId userId, TEntityInput input);
        Task<TEntity> Update(TUserId userId, TId id, TEntityInput input);
        Task Delete(TUserId userId, TId id);
        Task<IEnumerable<TEntity>> List(TUserId userId);
        Task<IEnumerable<TEntity>> ListWithOrder<TKey>(
            TUserId userId,
            bool descending,
            Func<TEntity, TKey> orderResultsKeySelector);
    }
}
