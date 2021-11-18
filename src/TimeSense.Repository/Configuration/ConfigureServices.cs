using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TimeSense.Repository.Configuration
{
    public static class ConfigureServices
    {
        public static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SensedTimesConfiguration>(configuration.GetSection(nameof(SensedTimesConfiguration)));
            services.AddSingleton<ISensedTimesConfiguration>(sp =>
                sp.GetRequiredService<IOptions<SensedTimesConfiguration>>().Value);
            services.AddSingleton<SensedTimesRepository, SensedTimesRepository>();
            services.AddSingleton<MetricsRepository, MetricsRepository>();
        }
    }
}
