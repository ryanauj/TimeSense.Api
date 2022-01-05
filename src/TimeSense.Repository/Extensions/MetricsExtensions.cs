using System;
using System.Collections.Generic;
using System.Linq;
using TimeSense.Models;

namespace TimeSense.Repository.Extensions
{
    public static class MetricsExtensions
    {
        public static IDictionary<decimal, Metric> CalculateMetrics(
            this IDictionary<decimal, IReadOnlyList<SensedTime>> sensedTimesByTargetTimes)
        {
            var metrics = new Dictionary<decimal, Metric>();

            foreach (var (key, value) in sensedTimesByTargetTimes)
            {
                metrics[key] = value.CalculateMetricsForTargetTime();
            }

            return metrics;
        }
        
        public static Metric CalculateMetricsForTargetTime(this IEnumerable<SensedTime> sensedTimesByTargetTime)
        {
            var sensedTimesSortedByActualTime = sensedTimesByTargetTime.OrderBy(st => st.ActualTime).ToList();

            return new Metric
            {
                Average = sensedTimesSortedByActualTime.Average(t => t.ActualTime),
                Median = sensedTimesSortedByActualTime.CalculateMedian(),
                TargetTime = sensedTimesSortedByActualTime.First().TargetTime,
                Min = sensedTimesSortedByActualTime.First().ActualTime,
                Max = sensedTimesSortedByActualTime.Last().ActualTime,
                Total = sensedTimesSortedByActualTime.Count
            };
        }

        private static decimal CalculateMedian(this IReadOnlyList<SensedTime> sensedTimesSortedByActualTime)
        {
            if (sensedTimesSortedByActualTime.Count == 1) return sensedTimesSortedByActualTime.First().ActualTime;
            
            var medianIndex = (sensedTimesSortedByActualTime.Count / 2) - 1;
            if (sensedTimesSortedByActualTime.Count % 2 == 1) return sensedTimesSortedByActualTime[medianIndex].ActualTime;

            var fasterTime = sensedTimesSortedByActualTime[medianIndex];
            var slowerTime = sensedTimesSortedByActualTime[medianIndex + 1];
            
            Console.WriteLine("fasterTime: {0}", fasterTime.ActualTime);
            Console.WriteLine("slowerTime: {0}", slowerTime.ActualTime);

            return (fasterTime.ActualTime + slowerTime.ActualTime) / 2;
        }
    }
}