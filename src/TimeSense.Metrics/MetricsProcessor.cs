using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeSense.Models;
using TimeSense.Repository;

namespace TimeSense.Metrics
{
    public class MetricsProcessor
    {
        private readonly MetricsRepository _repository;

        public MetricsProcessor(MetricsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task AddMetric(string userId, SensedTime sensedTime) =>
            AddMetrics(
                userId,
                decimal.ToInt32(sensedTime.TargetTime),
                new List<SensedTime> {sensedTime});

        public Task AddMetrics(string userId, int targetTime, IList<SensedTime> sensedTimes) =>
            AddMetrics(
                userId,
                new Dictionary<int, IList<SensedTime>>
                {
                    [targetTime] = sensedTimes
                });
        

        public async Task AddMetrics(string userId, IDictionary<int, IList<SensedTime>> sensedTimesByTargetTime)
        {
            var metrics = await _repository.Get(userId) ?? new MetricsEntity();

            foreach (var kvp in sensedTimesByTargetTime)
            {
                var targetTime = kvp.Key;
                var sensedTimes = kvp.Value;
            
                var metric = metrics.Metrics.ContainsKey(targetTime)
                    ? metrics.Metrics[targetTime]
                    : new Metric {TargetTime = targetTime};
                
                var previousTotal = metric.Total;
                var previousAverage = metric.Average;
                var previousAverageSum = previousAverage * previousTotal;
                var actualTimesSum = sensedTimes.Sum(st => st.ActualTime);
                
                metric.Total += sensedTimes.Count;
                metric.Average = (previousAverageSum + actualTimesSum) / metric.Total;

                metrics.Metrics[targetTime] = metric;
            }

            await _repository.Update(userId, metrics.Metrics);
        }
    }
}