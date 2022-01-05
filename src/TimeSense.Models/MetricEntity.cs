using System;

namespace TimeSense.Models
{
    [Serializable]
    public class MetricEntity : Metric
    {
        public MetricEntity(Metric metric, string userId)
        {
            Id = new MetricEntityId
            {
                TargetTime = metric.TargetTime,
                UserId = userId
            };
            TargetTime = metric.TargetTime;
            Total = metric.Total;
            Average = metric.Average;
            Median = metric.Median;
            Min = metric.Min;
            Max = metric.Max;
            MostRecent = metric.MostRecent;
        }
        
        public MetricEntityId Id { get; set; }
    }
}