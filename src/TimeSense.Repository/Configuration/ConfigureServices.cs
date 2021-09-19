using Microsoft.Extensions.DependencyInjection;

namespace TimeSense.Repository.Configuration
{
    public static class ConfigureServices
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<SensedTimesRepository, SensedTimesRepository>();
            services.AddSingleton<MetricsRepository, MetricsRepository>();
        }
    }
}
