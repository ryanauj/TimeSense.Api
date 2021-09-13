using System;

namespace TimeSense.Models
{
    [Serializable]
    public class SensedTime : IEntity<string, string>
    {
        public string UserId { get; set; }
        
        public string Id { get; set; }
        
        public decimal TargetTime { get; set; }

        public decimal ActualTime { get; set; }
        
        public DateTimeOffset UpdatedAt { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; }
    }
}
