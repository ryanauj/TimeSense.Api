using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using TimeSense.Models;
using TimeSense.Repository.Interfaces;
using TimeSense.Serialization;

namespace TimeSense.Repository.Abstractions
{
    public abstract class BaseRepository<TEntityInput, TEntity> : IRepository<string, string, TEntityInput, TEntity>
        where TEntity : class, IEntity<string, string>
    {
        private const string UserIdKey = "UserId";
        private const string UserIdValueKey = ":userId";
        private const string IdKey = "Id";
        private readonly string _tableName;
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly ISerializer _serializer;

        protected abstract TEntity Build(IEntity<string, string> commonData, TEntityInput input);

        protected BaseRepository(
            string tableName,
            IAmazonDynamoDB dynamoDb,
            ISerializer serializer)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(tableName));
            _tableName = tableName;
            _dynamoDb = dynamoDb ?? throw new ArgumentNullException(nameof(dynamoDb));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task<TEntity> Get(string userId, string id)
        {
            var request = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    [UserIdKey] = new AttributeValue {S = userId},
                    [IdKey] = new AttributeValue {S = id}
                }
            };
            var response = await _dynamoDb.GetItemAsync(request);
            var document = Document.FromAttributeMap(response.Item);
            var serializedItem = document.ToJson();
            
            return _serializer.Deserialize<TEntity>(serializedItem);
        }

        public async Task<string> Create(string userId, TEntityInput input)
        {
            var id = Guid.NewGuid().ToString();
            var baseEntity = new BaseEntity<string, string>
            {
                Id = id,
                UserId = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var item = Build(baseEntity, input);
            var serializedItem = _serializer.Serialize(item);
            var document = Document.FromJson(serializedItem);
            
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = document.ToAttributeMap()
            };
            
            await _dynamoDb.PutItemAsync(request);

            return id;
        }

        public async Task Update(string userId, string id, TEntityInput input)
        {
            var baseEntity = await Get(userId, id);
            baseEntity.UpdatedAt = DateTime.Now;
            var item = Build(baseEntity, input);
            var serializedItem = _serializer.Serialize(item);
            var document = Document.FromJson(serializedItem);
            
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = document.ToAttributeMap()
            };
            
            await _dynamoDb.PutItemAsync(request);
        }

        public Task Delete(string userId, string id)
        {
            var request = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    [UserIdKey] = new AttributeValue {S = userId},
                    [IdKey] = new AttributeValue {S = id}
                }
            };
            
            return _dynamoDb.DeleteItemAsync(request);
        }

        public async Task<IEnumerable<TEntity>> List(string userId)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = $"{UserIdKey} = {UserIdValueKey}",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [UserIdValueKey] = new AttributeValue {S = userId},
                }
            };
            var response = await _dynamoDb.QueryAsync(request);
            
            return response.Items.Select(AttributeMapToType<TEntity>);
        }

        private T AttributeMapToType<T>(Dictionary<string, AttributeValue> attributeMap)
        {
            var json = Document.FromAttributeMap(attributeMap).ToJson();
            return _serializer.Deserialize<T>(json);
        }
    }
}
