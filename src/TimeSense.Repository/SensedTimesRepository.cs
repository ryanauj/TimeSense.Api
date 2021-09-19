using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using TimeSense.Models;
using TimeSense.Repository.Abstractions;
using TimeSense.Serialization;
using Microsoft.AspNetCore.Hosting;

namespace TimeSense.Repository
{
    public class SensedTimesRepository : BaseCompositeRepository<SensedTimeInput, SensedTime>
    {
        public SensedTimesRepository(IHostingEnvironment env, IAmazonDynamoDB dynamoDb, ISerializer serializer) :
            base($"sensed-time-table-{env.EnvironmentName}", dynamoDb, serializer)
        {
        }

        protected override SensedTime Build(
            ICompositeEntity<string, string> baseEntity,
            SensedTimeInput input
        ) =>
            new SensedTime
            {
                UserId = baseEntity.UserId,
                Id = baseEntity.Id,
                TargetTime = input.TargetTime ?? 0,
                ActualTime = input.ActualTime ?? 0,
                CreatedAt = baseEntity.CreatedAt,
                UpdatedAt = baseEntity.UpdatedAt
            };

        public virtual async Task<IEnumerable<SensedTime>> GetLatestSensedTimes(string userId, int numToRetrieve)
        {
            var allSensedTimes = await List(userId);

            var validSensedTimes = allSensedTimes.Where(x => x.UserId != x.Id);

            return validSensedTimes.Take(numToRetrieve);
        }
    }
}
