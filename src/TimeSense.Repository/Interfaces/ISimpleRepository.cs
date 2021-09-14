using System.Threading.Tasks;

namespace TimeSense.Repository.Interfaces
{
    public interface ISimpleRepository<in TId, in TInput, T>
    {
        Task<T> Get(TId id);
        Task<T> Create(TId id, TInput input);
        Task<T> Update(TId id, TInput input);
        Task Delete(TId id);
    }
}