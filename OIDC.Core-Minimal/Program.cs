using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OIDC.Core_Minimal.DAL;
using OIDC.Core_Minimal.Services.Implementation;
using OIDC.Core_Minimal.Services.Interface;

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
        policy.WithOrigins("http://localhost:3000");
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

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisCache");
});

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();