using System.Collections.Generic;
using Amazon.DynamoDBv2;
using TimeSense.Models;
using TimeSense.Repository.Abstractions;
using TimeSense.Serialization;

namespace TimeSense.Repository
{
    public class MetricsRepository : BaseSimpleRepository<IDictionary<string, dynamic>, Metrics>
    {
        public MetricsRepository(string tableName, IAmazonDynamoDB dynamoDb, ISerializer serializer) : base(tableName, dynamoDb, serializer)
        {
        }

        protected override Metrics Build(ISimpleEntity<string> commonData, IDictionary<string, dynamic> input)
        {
            return new Metrics
            {
                UserId = commonData.UserId,
                CreatedAt = commonData.CreatedAt,
                UpdatedAt = commonData.UpdatedAt,
                Values = input
            };
        }
    }
}