using System.Diagnostics.Metrics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OIDC.Core_Minimal.DAL;
using OIDC.Core.DAL;
using OIDC.Core.Services.Implementation;
using OIDC.Core.Services.Interface;
using OIDC.Core.Util.Metrics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMetrics();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.ToString());
});

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
    var _rsa = RSA.Create();
    var content = File.ReadAllText(builder.Configuration.GetValue<string>("OIDC:PrivateKey:Path")!);
    _rsa.ImportFromPem(content);
    
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetValue<string>("JWT:Issuer"),
        ValidAudience = builder.Configuration.GetValue<string>("JWT:Audience"),
        IssuerSigningKey = new RsaSecurityKey(_rsa),
        ValidAlgorithms = new[] { SecurityAlgorithms.RsaSha256 },
        LogTokenId = true,
        LogValidationExceptions = true
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
builder.Services.AddScoped<IAuthenticationEventService, AuthenticationEventService>();

// Metrics
builder.Services.AddSingleton<APIEvents>();
builder.Services.AddSingleton<IJwksKeyService>(sp =>
    new RsaKeyService(
        privateKeyPath: builder.Configuration.GetValue<string>("OIDC:PrivateKey:Path")!,
        keyId: builder.Configuration.GetValue<string>("OIDC:PrivateKey:KeyId")!
    )
);

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

    builder.Logging.AddOpenTelemetry(options =>
    {
        options
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService("oidc_core_api"))
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otelEndpoint);
            });
    });

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
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddMeter("OIDCCore.API")
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddMeter("System.Net.Http")
            .AddMeter("System.Net.NameResolution")
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otelEndpoint);
            })

        );
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