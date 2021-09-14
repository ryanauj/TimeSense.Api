using System;

namespace TimeSense.Models
{
    public interface ICompositeEntity<TUserId, TId> : ISimpleEntity<TUserId>
    {
        TId Id { get; set; }
    }
}
