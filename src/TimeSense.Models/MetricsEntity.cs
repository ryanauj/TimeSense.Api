using System;
using System.Collections.Generic;

namespace TimeSense.Models
{
    [Serializable]
    public class MetricsEntity
    {
        public string UserId { get; set; }
        public IDictionary<decimal, Metric> Metrics { get; set; }
    }
}