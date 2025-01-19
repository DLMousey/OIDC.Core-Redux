using System.Diagnostics.Metrics;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OIDC.Core_Minimal.DAL;
using OIDC.Core_Minimal.Services.Implementation;
using OIDC.Core_Minimal.Services.Interface;
using OIDC.Core_Minimal.Util.Metrics;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddEnvironmentVariables(prefix: "OIDCC_");

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "localDev", policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyMethod();
        policy.AllowAnyHeader();
    });
});

builder.Services.AddDbContext<OIDCCoreMinimalDbContext>(options =>
{
    if (!builder.Configuration.GetValue<bool>("Database:FromEnvironment"))
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));    
    }
    else
    {
        string? host = builder.Configuration.GetValue<string>("Database:Host");
        string? port = builder.Configuration.GetValue<string>("Database:Port");
        string? user = builder.Configuration.GetValue<string>("Database:User");
        string? pass = builder.Configuration.GetValue<string>("Database:Password");
        string? name = builder.Configuration.GetValue<string>("Database:Name");

        options.UseNpgsql($"Host={host};Port={port};Username={user};Password={pass};Database={name}");
    }
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetValue<string>("JWT:Issuer"),
        ValidAudience = builder.Configuration.GetValue<string>("JWT:Audience"),
        IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWT:SigningKey")!))
    };
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
builder.Services.AddScoped<IScopeService, ScopeService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMailService, MailService>();

// Metrics
builder.Services.AddSingleton<AuthenticationEvents>();
builder.Services.AddSingleton<OAuthEvents>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisCache");
});

// Open Telemetry Instrumentation
if (builder.Configuration.GetValue("Otel:Enabled", false))
{
    string? otelEndpoint = builder.Configuration.GetValue<string>("Otel:OtlpEndpoint");
    if (otelEndpoint == null)
    {
        throw new ApplicationException("OTEL enabled but otlp endpoint not provided");
    }

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource =>
            resource.AddService(serviceName: "oidc_core_api"))
        .WithTracing(tracingBuilder => tracingBuilder
            .AddAspNetCoreInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otelEndpoint);
            }))
        .WithMetrics(metricsBuilder => metricsBuilder
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otelEndpoint);
            })
            .AddMeter("OIDCCore.Authentication.*")
            .AddMeter("OIDCCore.OAuth.*")
        );

    builder.Logging.AddOpenTelemetry(loggingBuilder => loggingBuilder.AddOtlpExporter(
        "logging",
        options =>
        {
            options.Endpoint = new Uri(otelEndpoint);
        }));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (builder.Configuration.GetValue<bool>("Database:RunMigrations"))
{
    IServiceScope serviceScope = app.Services.CreateScope();
    OIDCCoreMinimalDbContext context = serviceScope.ServiceProvider.GetRequiredService<OIDCCoreMinimalDbContext>();
    await context.Database.MigrateAsync();

    serviceScope.Dispose();
}

app.UseCors("localDev");

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();