using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DotKsuid;
using MongoDB.Driver;
using Newtonsoft.Json;
using TimeSense.Models;
using TimeSense.Repository.Configuration;

namespace TimeSense.Repository.Abstractions
{
    public abstract class BaseMongoUserCentricRepository<TEntityInput, TEntity>
        where TEntity : class, ICompositeEntity<string, string>
    {
        protected readonly IMongoCollection<TEntity> EntityCollection;

        protected BaseMongoUserCentricRepository(IMongoCollection<TEntity> entityCollection)
        {
            EntityCollection = entityCollection ?? throw new ArgumentNullException(nameof(entityCollection));
        }

        private static bool EntityHasUserIdAndId(TEntity entity, string userId, string id)
            => entity.Id == id && entity.UserId == userId;

        public async Task<TEntity> Get(string userId, string id)
        {
            var entities = await EntityCollection.FindAsync(
                entity => EntityHasUserIdAndId(entity, userId, id));
            return entities.FirstOrDefault();
        }

        public async Task<TEntity> Create(string userId, TEntityInput input)
        {
            var now = DateTimeOffset.Now;
            var baseEntity = new BaseCompositeEntity<string, string>
            {
                Id = Ksuid.NewKsuid().ToString(),
                UserId = userId,
                CreatedAt = now,
                UpdatedAt = now
            };

            var entity = Build(baseEntity, input);

            await EntityCollection.InsertOneAsync(entity);

            return entity;
        }

        public async Task Update(string userId, string id, TEntityInput input)
        {
            var currentEntity = await Get(userId, id);
            currentEntity.UpdatedAt = DateTimeOffset.Now;
            var updatedEntity = Build(currentEntity, input);

            await EntityCollection.ReplaceOneAsync(entity => EntityHasUserIdAndId(entity, userId, id), updatedEntity);
        }

        public Task Delete(string userId, string id)
        {
            return EntityCollection.DeleteOneAsync(entity => EntityHasUserIdAndId(entity, userId, id));
        }

        public async Task<IEnumerable<TEntity>> List(Expression<Func<TEntity, bool>> filter)
        {
            var entities = await EntityCollection.FindAsync<TEntity>(filter);
            return entities.ToEnumerable();
        }
        
        protected abstract TEntity Build(ICompositeEntity<string, string> entity, TEntityInput input);
    }
}