using System;
using System.Collections.Generic;

namespace TimeSense.Models
{
    [Serializable]
    public class Metrics : ICompositeEntity<string, string>
    {
        public string UserId { get; set; }
        public string Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public IDictionary<int, decimal> Averages { get; set; }
    }
}