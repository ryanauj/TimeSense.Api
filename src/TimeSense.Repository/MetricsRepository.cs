using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TimeSense.Models;
using TimeSense.Repository.Extensions;

namespace TimeSense.Repository
{
    public class MetricsRepository
    {
        private readonly IMongoCollection<SensedTime> _sensedTimesCollection;
        private readonly IMongoCollection<MetricEntity> _metricEntityCollection;
        private readonly SensedTimesRepository _sensedTimesRepository;
        
        public MetricsRepository(
            IMongoCollection<SensedTime> sensedTimesCollection,
            IMongoCollection<MetricEntity> metricsCollection,
            SensedTimesRepository sensedTimesRepository)
        {
            _sensedTimesCollection = sensedTimesCollection ?? throw new ArgumentNullException(nameof(sensedTimesCollection));
            _metricEntityCollection = metricsCollection ?? throw new ArgumentNullException(nameof(metricsCollection));
            _sensedTimesRepository = sensedTimesRepository ?? throw new ArgumentNullException(nameof(sensedTimesRepository));
        }

        public async Task<MetricsEntity> Get(string userId)
        {
            var distinctTargetTimes = await _metricEntityCollection.GetUserDistinctTargetTimes(userId);

            var metricByTargetTimeTasks =
                distinctTargetTimes.Select(targetTime => GetMetricByTargetTime(userId, targetTime)).ToList();
            
            await Task.WhenAll(metricByTargetTimeTasks);

            var metricByTargetTimes =
                metricByTargetTimeTasks
                    .Select(metricByTargetTimeTask => metricByTargetTimeTask.Result)
                    .ToDictionary(
                        metric => metric.First().TargetTime, 
                        metric => metric.First());

            return new MetricsEntity
            {
                UserId = userId,
                Metrics = metricByTargetTimes
            };
        }

        public Task Update(string userId, decimal targetTime)
        {
            var sensedTimesForTargetTime = _sensedTimesRepository.GetLatestSensedTimesForTargetTime(userId, targetTime);

            var metricForTargetTime = sensedTimesForTargetTime.CalculateMetricsForTargetTime();
            
            var metricEntity = new MetricEntity(metricForTargetTime, userId);
            
            return _metricEntityCollection.ReplaceOneAsync(
                metric => metric.Id.TargetTime == targetTime && metric.Id.UserId == userId,
                metricEntity);
        }

        public async Task RefreshAll()
        {
            var userIds = await _sensedTimesCollection.GetDistinctUserIds();
            var tasks = new List<Task>();

            foreach (var userId in userIds)
            {
                var targetTimesForUser = await _sensedTimesCollection.GetUserDistinctTargetTimes(userId);
                tasks.AddRange(targetTimesForUser.Select(targetTimeForUser => Update(userId, targetTimeForUser)));
            }

            await Task.WhenAll(tasks);
        }

        private async Task<IReadOnlyList<Metric>> GetMetricByTargetTime(string userId, decimal targetTime)
        {
            var metricCursor = await _metricEntityCollection.FindAsync(
                st => st.Id.UserId == userId && st.Id.TargetTime == targetTime);
            return await metricCursor.ToListAsync();
        }
    }
}