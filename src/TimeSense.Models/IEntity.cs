using System;

namespace TimeSense.Models
{
    public interface IEntity<TId>
    {
        TId Id { get; set; }
        DateTimeOffset CreatedAt { get; set; }
        DateTimeOffset UpdatedAt { get; set; }
    }
}