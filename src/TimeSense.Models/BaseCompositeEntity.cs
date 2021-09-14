namespace TimeSense.Models
{
    public class BaseCompositeEntity<TUserId, TId> : BaseSimpleEntity<TUserId>, ICompositeEntity<TUserId, TId>
    {
        public TId Id { get; set; }
    }
}