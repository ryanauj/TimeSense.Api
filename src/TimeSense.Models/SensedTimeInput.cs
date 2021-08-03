using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TimeSense.Models
{
    public class SensedTimeInput
    {
        public long TargetTimeInMilliseconds { get; set; }

        public long ActualTimeInMilliseconds { get; set; }
    }
}
