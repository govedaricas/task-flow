using Application.Settings;

namespace task_flow_api.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string GetEnvOrConfig(this IConfiguration config, string envVar, string configPath)
        {
            return Environment.GetEnvironmentVariable(envVar) ?? config[configPath] ?? throw new InvalidOperationException($"Missing configuration for {configPath}");
        }

        public static IServiceCollection ConfigureJwtSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtSettings>(options =>
            {
                config.GetSection("Jwt").Bind(options);

                options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? options.Issuer;
                options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? options.Audience;
                options.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRETKEY") ?? options.SecretKey;
            });

            return services;
        }

        public static IServiceCollection ConfigureSmtpSettings(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<SmtpSettings>(options =>
            {
                config.GetSection("Smtp").Bind(options);

                options.SenderEmail = Environment.GetEnvironmentVariable("SMTP_SENDEREMAIL") ?? options.SenderEmail;
                options.Username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? options.Username;
                options.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? options.Password;
            });

            return services;
        }
    }
}
