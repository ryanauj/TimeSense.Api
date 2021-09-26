using System;

namespace TimeSense.Models
{
    [Serializable]
    public class Metric
    {
        public int TargetTime { get; set; }
        public int Total { get; set; }
        public decimal Average { get; set; }
    }
}