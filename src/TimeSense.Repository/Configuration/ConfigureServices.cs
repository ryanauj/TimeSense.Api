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
            services.Configure<SensedTimesConfiguration>(configuration.GetSection(nameof(SensedTimesConfiguration)));
            services.AddSingleton<ISensedTimesConfiguration>(sp =>
                sp.GetRequiredService<IOptions<SensedTimesConfiguration>>().Value);
            services.AddSingleton<IMongoClient>(sp =>
            {
                var config = sp.GetRequiredService<ISensedTimesConfiguration>();
                return new MongoClient(config.ConnectionString);
            });
            services.AddSingleton<IMongoCollection<SensedTime>>(sp =>
            {
                var config = sp.GetRequiredService<ISensedTimesConfiguration>();
                var client = sp.GetRequiredService<IMongoClient>();
                var database = client.GetDatabase(config.DatabaseName);
                
                return database.GetCollection<SensedTime>(config.CollectionName);
            });
            services.AddSingleton<SensedTimesRepository, SensedTimesRepository>();
            services.AddSingleton<MetricsRepository, MetricsRepository>();
        }
    }
}
