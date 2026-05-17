using Application.Interfaces;
using Application.Interfaces.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Persistance.Services
{
    public static class InfastructureServiceRegistration
    {
        public static IServiceCollection AddInfastructureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Environment.GetEnvironmentVariable("REDIS_CONFIGURATION")?? configuration["Redis:Configuration"];

                options.InstanceName = Environment.GetEnvironmentVariable("REDIS_INSTANCE_NAME") ?? configuration["Redis:InstanceName"];
            });

            services.AddScoped<ICacheService, CacheService>();
            return services;
        }
    }
}
