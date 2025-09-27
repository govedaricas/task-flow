using Application.BackgroundJobs;
using Application.Features.AuditLog;
using Application.Features.Auth.Login;
using Application.Interfaces;
using Application.Interfaces.Implementations;
using Application.Settings;
using Hangfire;
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


// Add services to the container.
builder.Services.AddControllers();

// Authentication & JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
    };
});


// DbContext
builder.Services.AddDbContext<ITaskFlowDbContext, TaskFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("SqlServerConnection")));
builder.Services.AddHangfireServer();


// Handlers & DI
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

// Settings configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));


//Adding swagger
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

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<Application.Exceptions.IExceptionHandler, GlobalExceptionHandler>();

var app = builder.Build();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });

    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[]
    {
        app.Services.GetRequiredService<HangfireDashboardJwtAuthorizationFilter>()
    }
    });
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var jobRegistrar = scope.ServiceProvider.GetRequiredService<BackgroundJobHandler>();
    jobRegistrar.RegisterJobs();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
