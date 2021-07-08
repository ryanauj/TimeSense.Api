using System;

namespace TimeSense.Models
{
    public class SensedTime : SensedTimeProperty, IEntity<string, string>
    {
        public string UserId { get; set; }
        public string Id { get; set; }
    }
}
