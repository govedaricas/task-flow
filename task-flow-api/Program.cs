using Application.BackgroundJobs;
using Application.Settings;
using DotNetEnv;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using task_flow_api.Extensions;
using task_flow_api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---------- CONFIGURATION ----------

Env.Load();

// Load appsettings and env variables
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// ---------- SERVICES ----------
builder.Services.AddControllers();

// Configure Jwt & SMTP using extensions
builder.Services
    .ConfigureJwtSettings(builder.Configuration)
    .ConfigureSmtpSettings(builder.Configuration)
    .AddDatabase(builder.Configuration)          
    .AddApplicationServices();                   

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Services.BuildServiceProvider()
        .GetRequiredService<Microsoft.Extensions.Options.IOptions<JwtSettings>>().Value;

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

// Enable legacy timestamp behavior for Npgsql
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("SignalRCors", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true); 
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
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseMiddleware<CurrentUserMiddleware>();
app.UseAuthorization();
app.MapControllers();

// ---------- Register background jobs ----------
using (var scope = app.Services.CreateScope())
{
    var jobRegistrar = scope.ServiceProvider.GetRequiredService<BackgroundJobHandler>();
    jobRegistrar.RegisterJobs();
}

app.Run();
