namespace TimeSense.Models
{
    public class BaseCompositeEntity<TUserId, TId> : BaseEntity<TId>, ICompositeEntity<TUserId, TId>
    {
        public TUserId UserId { get; set; }
    }
}