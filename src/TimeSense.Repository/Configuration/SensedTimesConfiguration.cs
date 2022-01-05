namespace TimeSense.Repository.Configuration
{
    public class SensedTimesConfiguration : ISensedTimesConfiguration
    {
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string MetricsCollectionName { get; set; }
    }

    public interface ISensedTimesConfiguration : IMongoDbConfiguration
    {
    }
}