using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using TimeSense.Models;
using TimeSense.Repository.Abstractions;
using TimeSense.Serialization;
using Microsoft.AspNetCore.Hosting;
using TimeSense.Repository.Interfaces;

namespace TimeSense.Repository
{
    public class SensedTimesRepository : BaseRepository<SensedTimeInput, SensedTime>, ISensedTimesRepository
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

        public async Task<IEnumerable<SensedTime>> GetLatestSensedTimes(string userId, int numToRetrieve)
        {
            var allSensedTimes = await List(userId);

            return allSensedTimes.OrderByDescending(t => t.CreatedAt).Take(numToRetrieve);
        }
    }
}
