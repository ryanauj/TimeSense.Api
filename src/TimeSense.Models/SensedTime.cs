using System;

namespace TimeSense.Models
{
    public class SensedTime : SensedTimeInput, IEntity<string, string>
    {
        public string UserId { get; set; }
        
        public string Id { get; set; }
        
        public DateTimeOffset UpdatedAt { get; set; }
        
        public DateTimeOffset CreatedAt { get; set; }
    }
}
