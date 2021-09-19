using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Hosting;
using TimeSense.Models;
using TimeSense.Repository.Abstractions;
using TimeSense.Serialization;

namespace TimeSense.Repository
{
    public class MetricsRepository : BaseCompositeRepository<MetricsRepositoryInput, Metrics>
    {
        public MetricsRepository(IHostingEnvironment env, IAmazonDynamoDB dynamoDb, ISerializer serializer) :
            base($"sensed-time-table-{env.EnvironmentName}", dynamoDb, serializer)
        {
        }

        public Task<Metrics> Get(string userId) => Get(userId, userId);
        
        public Task<Metrics> Update(string userId, MetricsRepositoryInput input) => Update(userId, userId, input);

        public Task Delete(string userId) => Delete(userId, userId);
        
        protected override Metrics Build(ICompositeEntity<string, string> commonData, MetricsRepositoryInput input)
        {
            return new Metrics
            {
                UserId = commonData.UserId,
                Id = commonData.Id,
                CreatedAt = commonData.CreatedAt,
                UpdatedAt = commonData.UpdatedAt,
                Averages = input.Averages
            };
        }
    }
}