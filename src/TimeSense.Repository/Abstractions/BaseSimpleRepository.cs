using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using TimeSense.Models;
using TimeSense.Repository.Interfaces;
using TimeSense.Serialization;

namespace TimeSense.Repository.Abstractions
{
    public abstract class BaseSimpleRepository<TInput, T> : ISimpleRepository<string, TInput, T>
        where T : class, ISimpleEntity<string>
    {
        private const string IdKey = "UserId";
        private readonly string _tableName;
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly ISerializer _serializer;

        protected abstract T Build(ISimpleEntity<string> commonData, TInput input);

        protected BaseSimpleRepository(
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

        public async Task<T> Get(string id)
        {
            var request = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    [IdKey] = new AttributeValue {S = id}
                }
            };
            var response = await _dynamoDb.GetItemAsync(request);
            var document = Document.FromAttributeMap(response.Item);
            var serializedItem = document.ToJson();
            
            return _serializer.Deserialize<T>(serializedItem);
        }

        public async Task<T> Create(string id, TInput input)
        {
            var baseItem = new BaseCompositeEntity<string, string>
            {
                Id = id,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            var item = Build(baseItem, input);
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

        public async Task<T> Update(string id, TInput input)
        {
            var baseItem = await Get(id);
            baseItem.UpdatedAt = DateTime.Now;
            var item = Build(baseItem, input);
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

        public Task Delete(string id)
        {
            var request = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    [IdKey] = new AttributeValue {S = id}
                }
            };
            
            return _dynamoDb.DeleteItemAsync(request);
        }
    }
}