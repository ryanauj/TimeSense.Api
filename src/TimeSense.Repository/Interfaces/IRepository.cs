using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeSense.Repository.Interfaces
{
    public interface IRepository<in TUserId, TId, in TEntityInput, TEntity>
    {
        Task<TEntity> Get(TUserId userId, TId id);
        Task<TId> Create(TUserId userId, TEntityInput input);
        Task Update(TUserId userId, TId id, TEntityInput input);
        Task Delete(TUserId userId, TId id);
        Task<IEnumerable<TEntity>> List(TUserId userId);
    }
}
