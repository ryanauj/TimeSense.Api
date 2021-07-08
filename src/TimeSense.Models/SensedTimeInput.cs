using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TimeSense.Models
{
    public class SensedTimeInput
    {
        [JsonConverter(typeof(Newtonsoft.Json.Converters.UnixDateTimeConverter))]
        public DateTimeOffset TargetTime { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset StartTime { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset StopTime { get; set; }
    }
}
