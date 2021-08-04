using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;

namespace TimeSense.Repository.Interfaces
{
    public interface IQueryableRepository<in TUserId, TId, in TEntityInput, TEntity> : IRepository<TUserId, TId, TEntityInput, TEntity>
    {
        Task<IEnumerable<TEntity>> Query(QueryRequest queryRequest);
    }
}