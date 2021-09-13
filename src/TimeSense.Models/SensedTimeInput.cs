using System;

namespace TimeSense.Models
{
    [Serializable]
    public class SensedTimeInput
    {
        public decimal? TargetTime { get; set; }

        public decimal? ActualTime { get; set; }
    }
}
