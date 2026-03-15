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
                options.Configuration = configuration["Redis:Configuration"];
                options.InstanceName = configuration["Redis:InstanceName"];
            });

            services.AddScoped<ICacheService, CacheService>();
            return services;
        }
    }
}
