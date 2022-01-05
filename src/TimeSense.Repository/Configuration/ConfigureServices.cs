using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TimeSense.Models;

namespace TimeSense.Repository.Configuration
{
    public static class ConfigureServices
    {
        public static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(SensedTimesConfiguration));
            services.Configure<SensedTimesConfiguration>(section);
            services.AddSingleton<ISensedTimesConfiguration>(sp =>
                sp.GetRequiredService<IOptions<SensedTimesConfiguration>>().Value);
            services.AddSingleton<IMongoClient>(sp =>
            {
                var config = sp.GetRequiredService<ISensedTimesConfiguration>();
                return new MongoClient(config.ConnectionString);
            });
            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var config = sp.GetRequiredService<ISensedTimesConfiguration>();
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(config.DatabaseName);
            });
            services.AddSingleton<IMongoCollection<SensedTime>>(sp =>
            {
                var config = sp.GetRequiredService<ISensedTimesConfiguration>();
                var database = sp.GetRequiredService<IMongoDatabase>();
                
                return database.GetCollection<SensedTime>(config.CollectionName);
            });
            services.AddSingleton<IMongoCollection<MetricEntity>>(sp =>
            {
                var config = sp.GetRequiredService<ISensedTimesConfiguration>();
                var database = sp.GetRequiredService<IMongoDatabase>();
                
                return database.GetCollection<MetricEntity>(config.MetricsCollectionName);
            });
            services.AddSingleton<SensedTimesRepository, SensedTimesRepository>();
            services.AddSingleton<MetricsRepository, MetricsRepository>();
        }
    }
}
