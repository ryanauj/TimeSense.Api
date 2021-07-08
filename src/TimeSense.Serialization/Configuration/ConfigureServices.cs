using Microsoft.Extensions.DependencyInjection;

namespace TimeSense.Serialization.Configuration
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddSerialization(this IServiceCollection services) =>
            services.AddSingleton<ISerializer, Serializer>();
    }
}
