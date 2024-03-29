﻿using System;
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
                metrics[key] = value.CalculateMetric();
            }

            return metrics;
        }
        
        private static Metric CalculateMetric(this IReadOnlyList<SensedTime> sensedTimes, int mostRecentToTake = 5)
        {
            var sensedTimesSortedByActualTime = sensedTimes.OrderBy(st => st.ActualTime).ToList();
            var sensedTimesSortedByDate = sensedTimes.OrderByDescending(st => st.CreatedAt);

            return new Metric
            {
                Average = sensedTimesSortedByActualTime.Average(t => t.ActualTime),
                Median = sensedTimesSortedByActualTime.CalculateMedian(),
                TargetTime = sensedTimesSortedByActualTime.First().TargetTime,
                Min = sensedTimesSortedByActualTime.First().ActualTime,
                Max = sensedTimesSortedByActualTime.Last().ActualTime,
                Total = sensedTimesSortedByActualTime.Count,
                MostRecent = sensedTimesSortedByDate.Take(mostRecentToTake)
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