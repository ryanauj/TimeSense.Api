using System;

namespace TimeSense.Models
{
    public class BaseEntity<TUserId, TId> : IEntity<TUserId, TId>
    {
        public TUserId UserId { get; set; }
        public TId Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}