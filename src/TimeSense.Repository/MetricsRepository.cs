using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using TimeSense.Models;
using TimeSense.Repository.Extensions;

namespace TimeSense.Repository
{
    public class MetricsRepository
    {
        private readonly IMongoCollection<SensedTime> _sensedTimesCollection;
        private readonly IMongoCollection<MetricEntity> _metricEntityCollection;
        private readonly SensedTimesRepository _sensedTimesRepository;
        private readonly ILogger<MetricsRepository> _logger;
        
        public MetricsRepository(
            IMongoCollection<SensedTime> sensedTimesCollection,
            IMongoCollection<MetricEntity> metricsCollection,
            SensedTimesRepository sensedTimesRepository,
            ILogger<MetricsRepository> logger)
        {
            _sensedTimesCollection = sensedTimesCollection ?? throw new ArgumentNullException(nameof(sensedTimesCollection));
            _metricEntityCollection = metricsCollection ?? throw new ArgumentNullException(nameof(metricsCollection));
            _sensedTimesRepository = sensedTimesRepository ?? throw new ArgumentNullException(nameof(sensedTimesRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        public async Task Update(string userId, decimal targetTime)
        {
            var sensedTimesForTargetTime = _sensedTimesRepository.GetLatestSensedTimesForTargetTime(userId, targetTime);

            var metricForTargetTime = sensedTimesForTargetTime.CalculateMetricsForTargetTime();
            
            var metricEntity = new MetricEntity(metricForTargetTime, userId);

            var current = await _metricEntityCollection.FindAsync(metric =>
                metric.Id.TargetTime == targetTime && metric.Id.UserId == userId);
            if (!(await current.AnyAsync()))
            {
                await _metricEntityCollection.InsertOneAsync(metricEntity);
            }
            
            await _metricEntityCollection.ReplaceOneAsync(
                metric => metric.Id.TargetTime == targetTime && metric.Id.UserId == userId,
                metricEntity);
        }

        public async Task RefreshAll()
        {
            var userIds = (await _sensedTimesCollection.GetDistinctUserIds()).ToList();
            // _logger.LogInformation($"userIds: {JsonConvert.SerializeObject(userIds)}");
            
            var tasks = new List<Task>();

            foreach (var userId in userIds)
            {
                var targetTimesForUser = (await _sensedTimesCollection.GetUserDistinctTargetTimes(userId)).ToList();
                // _logger.LogInformation($"userId: {userId}, targetTimes: {JsonConvert.SerializeObject(targetTimesForUser)}");
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