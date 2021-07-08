namespace TimeSense.Models
{
    public interface IEntity<TUserId, TId>
    {
        TUserId UserId { get; set; }
        TId Id { get; set; }
    }
}
