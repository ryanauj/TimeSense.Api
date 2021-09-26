using Microsoft.Extensions.DependencyInjection;

namespace TimeSense.Metrics
{
    public static class ServiceConfiguration
    {
        public static void AddMetrics(this IServiceCollection services)
        {
            services.AddSingleton<MetricsProcessor, MetricsProcessor>();
        }
    }
}