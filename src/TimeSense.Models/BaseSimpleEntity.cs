using System;

namespace TimeSense.Models
{
    public class BaseSimpleEntity<TUserId> : ISimpleEntity<TUserId>
    {
        public TUserId UserId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}