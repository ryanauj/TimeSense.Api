using System;
using System.Collections.Generic;

namespace TimeSense.Models
{
    [Serializable]
    public class Metrics : ISimpleEntity<string>
    {
        public string UserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public IDictionary<string, dynamic> Values { get; set; }
    }
}