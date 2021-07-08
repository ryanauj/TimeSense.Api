using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimeSense.Repository.Interfaces
{
    public interface IRepository<in TUserId, TId, in TItemInput, TItem>
    {
        Task<TItem> Get(TUserId userId, TId id);
        Task<TId> Create(TUserId userId, TItemInput input);
        Task Update(TItem input);
        Task Delete(TUserId userId, TId id);
        Task<IEnumerable<TItem>> List(TUserId userId);
    }
}
