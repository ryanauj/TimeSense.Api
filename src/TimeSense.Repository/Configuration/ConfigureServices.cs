using TimeSense.Models;
using TimeSense.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace TimeSense.Repository.Configuration
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IRepository<string, string, SensedTimeInput, SensedTime>, TimeSenseRepository>();

            return services;
        }
    }
}
