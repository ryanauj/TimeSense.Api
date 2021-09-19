using Microsoft.Extensions.DependencyInjection;

namespace TimeSense.Mapping.Configuration
{
    public static class ServiceConfiguration
    {
        public static void AddMapping(this IServiceCollection services)
        {
            services.AddSingleton<MetricsInputMapper, MetricsInputMapper>();
        }
    }
}