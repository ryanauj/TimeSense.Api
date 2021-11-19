using System;
using System.Collections.Generic;

namespace TimeSense.Models
{
    [Serializable]
    public class Metric
    {
        public decimal TargetTime { get; set; }
        public int Total { get; set; }
        public decimal Average { get; set; }
        public decimal Median { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public IEnumerable<SensedTime> MostRecent { get; set; }
    }
}