using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Hosting;
using TimeSense.Models;
using TimeSense.Repository.Abstractions;
using TimeSense.Serialization;

namespace TimeSense.Repository
{
    public class MetricsRepository : BaseCompositeRepository<IDictionary<int, Metric>, MetricsEntity>
    {
        public MetricsRepository(IHostingEnvironment env, IAmazonDynamoDB dynamoDb, ISerializer serializer) :
            base($"sensed-time-table-{env.EnvironmentName}", dynamoDb, serializer)
        {
        }

        public Task<MetricsEntity> Get(string userId) => Get(userId, userId);
        
        public Task<MetricsEntity> Update(string userId, IDictionary<int, Metric> input) => Update(userId, userId, input);

        public Task Delete(string userId) => Delete(userId, userId);
        
        protected override MetricsEntity Build(ICompositeEntity<string, string> commonData, IDictionary<int, Metric> input)
        {
            return new MetricsEntity
            {
                UserId = commonData.UserId,
                Id = commonData.Id,
                CreatedAt = commonData.CreatedAt,
                UpdatedAt = commonData.UpdatedAt,
                Metrics = input
            };
        }
    }
}