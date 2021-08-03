using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TimeSense.Models
{
    public class SensedTime : SensedTimeInput, IEntity<string, string>
    {
        public string UserId { get; set; }
        
        public string Id { get; set; }
        
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset UpdatedAt { get; set; }
        
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
