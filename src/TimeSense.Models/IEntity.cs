using System;

namespace TimeSense.Models
{
    public interface IEntity<TUserId, TId>
    {
        TUserId UserId { get; set; }
        TId Id { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}
