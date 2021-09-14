using System;

namespace TimeSense.Models
{
    public interface ISimpleEntity<TUserId>
    {
        TUserId UserId { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}