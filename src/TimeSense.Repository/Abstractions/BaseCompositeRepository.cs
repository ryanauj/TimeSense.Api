using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using DotKsuid;
using TimeSense.Models;
using TimeSense.Repository.Interfaces;
using TimeSense.Serialization;

namespace TimeSense.Repository.Abstractions
{
    public abstract class BaseCompositeRepository<TEntityInput, TEntity> : ICompositeRepository<string, string, TEntityInput, TEntity>
        where TEntity : class, ICompositeEntity<string, string>
    {
        private const string UserIdKey = "UserId";
        private const string UserIdValueKey = ":userId";
        private const string IdKey = "Id";
        private readonly string _tableName;
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly ISerializer _serializer;

        protected abstract TEntity Build(ICompositeEntity<string, string> commonData, TEntityInput input);

        protected BaseCompositeRepository(
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

        public async Task<TEntity> Create(string userId, TEntityInput input)
        {
            var baseEntity = new BaseCompositeEntity<string, string>
            {
                Id = Ksuid.NewKsuid().ToString(),
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

            return item;
        }

        public async Task<TEntity> Create(string userId, string id, TEntityInput input)
        {
            var baseEntity = new BaseCompositeEntity<string, string>
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

            return item;
        }

        public async Task<TEntity> Update(string userId, string id, TEntityInput input)
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

            return item;
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
