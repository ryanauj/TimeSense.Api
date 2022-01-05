using System.Collections.Generic;

namespace TimeSense.Models
{
    public class SensedTimesAndMetrics
    {
        public IEnumerable<SensedTime> SensedTimes { get; set; }
        public Metric Metrics { get; set; }
    }
}