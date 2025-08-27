using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TuneStore.Application.Abstractions;
using TuneStore.Infrastructure;
using TuneStore.Infrastructure.Data;
using TuneStore.Infrastructure.Security;
using TuneStore.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TuneStore API", Version = "v1" });
});

// DbContext
builder.Services.AddDbContext<TuneStoreDbContext>(opt =>
    opt.UseInMemoryDatabase("TuneStoreInMemoryDb"));

// DataProtection & MemoryCache
builder.Services.AddDataProtection();
builder.Services.AddMemoryCache();

// JWT Authentication
var key = Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? "super-secret-key-min-32-chars-change-me");
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"] ?? "TuneStore",
            ValidAudience = config["Jwt:Audience"] ?? "TuneStore",
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

// Dependency Injection
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<IOrderService, OrderService>();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseHttpsRedirection();
app.UseMiddleware<TuneStore.Api.Middlewares.RequestLoggingMiddleware>();
app.UseMiddleware<TuneStore.Api.Middlewares.MaintenanceMiddleware>();
app.UseMiddleware<TuneStore.Api.Middlewares.ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Database seeding
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<TuneStoreDbContext>();
    var protector = scope.ServiceProvider
                        .GetRequiredService<IDataProtectionProvider>()
                        .CreateProtector("TuneStore.PasswordProtector.v1");

}

// Run the app
app.Run();
