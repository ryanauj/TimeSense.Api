using System;

namespace TimeSense.Models
{
    [Serializable]
    public class SensedTime : BaseCompositeEntity<string, string>
    {
        public decimal TargetTime { get; set; }

        public decimal ActualTime { get; set; }
    }
}
