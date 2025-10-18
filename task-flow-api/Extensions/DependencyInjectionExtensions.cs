using Application.BackgroundJobs;
using Application.Features.Administration.AuditLog;
using Application.Features.Administration.Auth.Login;
using Application.Interfaces;
using Application.Interfaces.Implementations;
using Persistance.Helpers;
using task_flow_api.Identity;
using task_flow_api.Middleware;

namespace task_flow_api.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(LoginCommandHandler))
                .AddClasses(classes => classes.InNamespaces("Application.Features"))
                .AsSelf()
                .WithScopedLifetime());

            services.AddHttpContextAccessor();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserIdentity, UserIdentity>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddScoped<AuditLogJobCommandHandler>();
            services.AddScoped<BackgroundJobHandler>();
            services.AddSingleton<HangfireDashboardJwtAuthorizationFilter>();
            services.AddSingleton<Application.Exceptions.IExceptionHandler, GlobalExceptionHandler>();

            return services;
        }
    }
}
