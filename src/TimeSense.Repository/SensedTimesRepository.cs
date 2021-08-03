using Amazon.DynamoDBv2;
using TimeSense.Models;
using TimeSense.Repository.Abstractions;
using TimeSense.Serialization;
using Microsoft.AspNetCore.Hosting;

namespace TimeSense.Repository
{
    public class SensedTimesRepository : BaseRepository<SensedTimeInput, SensedTime>
    {
        public SensedTimesRepository(IHostingEnvironment env, IAmazonDynamoDB dynamoDb, ISerializer serializer) :
            base($"sensed-time-table-{env.EnvironmentName}", dynamoDb, serializer)
        {
        }

        protected override SensedTime Build(
            IEntity<string, string> baseEntity,
            SensedTimeInput input
        ) =>
            new SensedTime
            {
                UserId = baseEntity.UserId,
                Id = baseEntity.Id,
                TargetTimeInMilliseconds = input.TargetTimeInMilliseconds,
                ActualTimeInMilliseconds = input.ActualTimeInMilliseconds,
                CreatedAt = baseEntity.CreatedAt,
                UpdatedAt = baseEntity.UpdatedAt
            };
    }
}
