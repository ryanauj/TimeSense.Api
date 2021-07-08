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
    public abstract class BaseRepository<TItemInput, TItem> : IRepository<string, string, TItemInput, TItem>
        where TItem : class, IEntity<string, string>
    {
        private readonly string _tableName;
        private readonly IAmazonDynamoDB _dynamoDb;
        private readonly ISerializer _serializer;

        protected abstract TItem Build(string userId, string id, TItemInput input);

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

        public async Task<TItem> Get(string userId, string id)
        {
            var request = new GetItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    [nameof(userId)] = new AttributeValue {S = userId.ToString()},
                    [nameof(id)] = new AttributeValue {S = id.ToString()}
                }
            };
            var response = await _dynamoDb.GetItemAsync(request);
            var document = Document.FromAttributeMap(response.Item);
            var serializedItem = document.ToJson();
            
            return _serializer.Deserialize<TItem>(serializedItem);
        }

        public async Task<string> Create(string userId, TItemInput input)
        {
            var id = Guid.NewGuid().ToString();
            var item = Build(userId, id, input);
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

        public Task Update(TItem input)
        {
            var serializedItem = _serializer.Serialize(input);
            var document = Document.FromJson(serializedItem);
            
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = document.ToAttributeMap()
            };
            
            return _dynamoDb.PutItemAsync(request);
        }

        public Task Delete(string userId, string id)
        {
            var request = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = new Dictionary<string, AttributeValue>
                {
                    [nameof(userId)] = new AttributeValue {S = userId.ToString()},
                    [nameof(id)] = new AttributeValue {S = id.ToString()}
                }
            };
            
            return _dynamoDb.DeleteItemAsync(request);
        }

        public async Task<IEnumerable<TItem>> List(string userId)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = $"{nameof(userId )} = :userId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    [nameof(userId)] = new AttributeValue {S = userId.ToString()},
                }
            };
            var response = await _dynamoDb.QueryAsync(request);
            var items = response.Items.Select(i =>
                _serializer.Deserialize<TItem>(Document.FromAttributeMap(i).ToJson()));
            
            return items;
        }
    }
}
