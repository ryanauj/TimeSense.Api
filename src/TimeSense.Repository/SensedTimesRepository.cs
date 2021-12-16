using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeSense.Models;
using TimeSense.Repository.Abstractions;
using MongoDB.Driver;

namespace TimeSense.Repository
{
    public class SensedTimesRepository : BaseMongoUserCentricRepository<SensedTimeInput, SensedTime>
    {
        public SensedTimesRepository(IMongoCollection<SensedTime> sensedTimesCollection) :
            base(sensedTimesCollection)
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

        public IDictionary<decimal, IList<SensedTime>> GetLatestSensedTimesByTargetTime(string userId, int? numToRetrieve=null)
        {
            var allSensedTimesFluent = EntityCollection.Find(st => st.UserId == userId);

            var validSensedTimes = allSensedTimesFluent.SortBy(st => st.CreatedAt).ToEnumerable();

            var sensedTimesByTargetTime = new Dictionary<decimal, IList<SensedTime>>();
            foreach (var sensedTime in validSensedTimes)
            {
                if (!sensedTimesByTargetTime.ContainsKey(sensedTime.TargetTime))
                {
                    sensedTimesByTargetTime[sensedTime.TargetTime] = new [] { sensedTime };
                }
                else if (numToRetrieve != null && sensedTimesByTargetTime[sensedTime.TargetTime].Count < numToRetrieve)
                {
                    sensedTimesByTargetTime[sensedTime.TargetTime].Add(sensedTime);
                }
            }

            return sensedTimesByTargetTime;
        }

        public IEnumerable<SensedTime> GetLatestSensedTimesForTargetTime(string userId, decimal targetTime, int numToRetrieve)
        {
            var allSensedTimesFluent = EntityCollection.Find(st => st.UserId == userId && st.TargetTime == targetTime);

            var validSensedTimes = allSensedTimesFluent.ToEnumerable();

            return validSensedTimes.Take(numToRetrieve);
        }
    }
}
