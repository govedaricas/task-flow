using Application.BackgroundJobs;
using Application.Features.Administration.AuditLog;
using Application.Features.Administration.Auth.Login;
using Application.Interfaces;
using Application.Interfaces.Implementations;
using Application.Settings;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistance.Context;
using Persistance.Helpers;
using System.Text;
using task_flow_api.Identity;
using task_flow_api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---------- CONFIGURATION ----------
// Load appsettings and env variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var configuration = builder.Configuration;

string GetEnvOrConfig(string envVar, string configPath) =>
    Environment.GetEnvironmentVariable(envVar) ?? configuration[configPath]!;

// ---------- SERVICES ----------
builder.Services.AddControllers();

// JWT
builder.Services.Configure<JwtSettings>(options =>
{
    configuration.GetSection("Jwt").Bind(options);

    options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? options.Issuer;
    options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? options.Audience;
    options.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRETKEY") ?? options.SecretKey;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Services.BuildServiceProvider().GetRequiredService<Microsoft.Extensions.Options.IOptions<JwtSettings>>().Value;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

// SMTP
builder.Services.Configure<SmtpSettings>(options =>
{
    configuration.GetSection("Smtp").Bind(options);

    options.Server = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? options.Server;
    options.Port = Environment.GetEnvironmentVariable("SMTP_PORT") ?? options.Port;
    options.SenderName = Environment.GetEnvironmentVariable("SMTP_SENDERNAME") ?? options.SenderName;
    options.SenderEmail = Environment.GetEnvironmentVariable("SMTP_SENDEREMAIL") ?? options.SenderEmail;
    options.Username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? options.Username;
    options.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? options.Password;
});

// DbContext
var host = GetEnvOrConfig("POSTGRES_HOST", "ConnectionStrings:PostgresConnection");
var port = GetEnvOrConfig("POSTGRES_PORT", "ConnectionStrings:PostgresConnection");
var db = GetEnvOrConfig("POSTGRES_DB", "ConnectionStrings:PostgresConnection");
var user = GetEnvOrConfig("POSTGRES_USER", "ConnectionStrings:PostgresConnection");
var pass = GetEnvOrConfig("POSTGRES_PASSWORD", "ConnectionStrings:PostgresConnection");

var connString = $"Host={host};Port={port};Database={db};Username={user};Password={pass}";

builder.Services.AddDbContext<ITaskFlowDbContext, TaskFlowDbContext>(options =>
    options.UseNpgsql(connString));

// Hangfire
builder.Services.AddHangfire(config =>
{
    config.UsePostgreSqlStorage(
        connString,
        new PostgreSqlStorageOptions
        {
            PrepareSchemaIfNecessary = false,
            QueuePollInterval = TimeSpan.FromSeconds(15)
        });
});
builder.Services.AddHangfireServer();

// DI
builder.Services.Scan(scan => scan
    .FromAssembliesOf(typeof(LoginCommandHandler))
    .AddClasses(classes => classes.InNamespaces("Application.Features"))
    .AsSelf()
    .WithScopedLifetime());

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserIdentity, UserIdentity>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<AuditLogJobCommandHandler>();
builder.Services.AddScoped<BackgroundJobHandler>();
builder.Services.AddSingleton<HangfireDashboardJwtAuthorizationFilter>();

builder.Services.AddSingleton<Application.Exceptions.IExceptionHandler, GlobalExceptionHandler>();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// ---------- SWAGGER ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}' to authorize"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// ---------- MIDDLEWARE ----------
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;

        if (exception != null)
        {
            var handler = context.RequestServices.GetRequiredService<Application.Exceptions.IExceptionHandler>();
            await handler.TryHandleAsync(exception, CancellationToken.None);
        }
    });
});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { app.Services.GetRequiredService<HangfireDashboardJwtAuthorizationFilter>() }
});

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Register background jobs
using (var scope = app.Services.CreateScope())
{
    var jobRegistrar = scope.ServiceProvider.GetRequiredService<BackgroundJobHandler>();
    jobRegistrar.RegisterJobs();
}

app.Run();
