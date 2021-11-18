using System;

namespace TimeSense.Models
{
    public class BaseEntity<TId> : IEntity<TId>
    {
        public TId Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}