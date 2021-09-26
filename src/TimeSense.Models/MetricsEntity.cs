using System;
using System.Collections.Generic;

namespace TimeSense.Models
{
    [Serializable]
    public class MetricsEntity : ICompositeEntity<string, string>
    {
        public string UserId { get; set; }
        public string Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public IDictionary<int, Metric> Metrics { get; set; }
    }
}