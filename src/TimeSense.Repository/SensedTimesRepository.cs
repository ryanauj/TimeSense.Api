using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeSense.Models;
using TimeSense.Repository.Abstractions;
using MongoDB.Driver;
using TimeSense.Repository.Configuration;

namespace TimeSense.Repository
{
    public class SensedTimesRepository : BaseMongoUserCentricRepository<SensedTimeInput, SensedTime>
    {
        public SensedTimesRepository(ISensedTimesConfiguration sensedTimesConfiguration) :
            base(sensedTimesConfiguration)
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

        public Task<IEnumerable<SensedTime>> List(string userId) => List(st => st.UserId == userId);

        public IEnumerable<SensedTime> GetLatestSensedTimes(string userId, int numToRetrieve)
        {
            var allSensedTimesFluent = EntityCollection.Find(st => st.UserId == userId);

            var validSensedTimes = allSensedTimesFluent.SortBy(st => st.CreatedAt).ToEnumerable();

            return validSensedTimes.Take(numToRetrieve);
        }
    }
}
