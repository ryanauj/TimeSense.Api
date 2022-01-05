namespace TimeSense.Repository.Configuration
{
    public interface IMongoDbConfiguration
    {
        string CollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
        string MetricsCollectionName { get; set; }
    }
}