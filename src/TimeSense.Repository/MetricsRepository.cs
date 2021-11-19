using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TimeSense.Metrics;
using TimeSense.Models;

namespace TimeSense.Repository
{
    public class MetricsRepository
    {
        private readonly IMongoCollection<SensedTime> _sensedTimesCollection;
        
        public MetricsRepository(IMongoCollection<SensedTime> sensedTimesCollection)
        {
            _sensedTimesCollection = sensedTimesCollection ?? throw new ArgumentNullException(nameof(sensedTimesCollection));
        }

        public async Task<MetricsEntity> Get(string userId)
        {
            var distinctTargetTimesForUserCursor = await _sensedTimesCollection.DistinctAsync(
                st => st.TargetTime, st => st.UserId == userId);
            var distinctTargetTimes = distinctTargetTimesForUserCursor.ToEnumerable();

            var sensedTimesByTargetTimeTasks =
                distinctTargetTimes.Select(targetTime => GetSensedTimesByTargetTime(userId, targetTime)).ToList();
            
            await Task.WhenAll(sensedTimesByTargetTimeTasks);

            var sensedTimesByTargetTimes =
                sensedTimesByTargetTimeTasks
                    .Select(sensedTimesByTargetTimeTask => sensedTimesByTargetTimeTask.Result)
                    .ToDictionary(sts => sts.First().TargetTime, sts => sts);

            var metrics = sensedTimesByTargetTimes.CalculateMetrics();
            
            return new MetricsEntity
            {
                UserId = userId,
                Metrics = metrics
            };
        }

        private async Task<IReadOnlyList<SensedTime>> GetSensedTimesByTargetTime(string userId, decimal targetTime)
        {
            var targetTimesCursor = await _sensedTimesCollection.FindAsync(
                st => st.UserId == userId && st.TargetTime == targetTime);
            return await targetTimesCursor.ToListAsync();
        }
    }
}