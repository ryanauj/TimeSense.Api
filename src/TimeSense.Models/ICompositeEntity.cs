using System;

namespace TimeSense.Models
{
    public interface ICompositeEntity<TUserId, TId> : IEntity<TId>
    {
        TUserId UserId { get; set; }
    }
}
