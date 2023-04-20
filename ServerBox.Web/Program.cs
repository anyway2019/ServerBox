using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using ServerBox.Web.Extensions;
using ServerBox.Web.Utils;
using SqlSugar;

var logger = NLog.LogManager.Setup().LoadConfigurationFromFile().GetCurrentClassLogger();
logger.Debug("server is starting.");
try
{
    var builder = WebApplication.CreateBuilder(args);
    // 配置 NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    // read config
    ConfigHelper.RegisterConfiguration(builder.Configuration);
    // Add Response Compression
    builder.Services.AddResponseCompression();
    // Add memory cache services
    builder.Services.AddMemoryCache();
    // Add services to the container.
    builder.Services.RegisterCustomServices(builder.Configuration);
    // Add sqlsugar
    var connectionString = builder.Configuration["Data:Conn"];
    builder.Services.AddSqlSugarClient<SqlSugarClient>((config) =>
    {
        config.ConnectionString = connectionString;
        config.DbType = DbType.MySql;
        config.IsAutoCloseConnection = true;
        config.InitKeyType = InitKeyType.Attribute;
    });
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen((c) =>
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
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
    
    // Add JwtBearer
    var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtKey"]);
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                
            };
        });

    // By default, all incoming requests will be authorized according to the default policy.
    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = options.DefaultPolicy;
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseHealthChecks("/health");
         
    app.UseCookiePolicy();

    app.UseStaticFiles();
    app.UseCors(x => x
        .AllowAnyOrigin().SetPreflightMaxAge(TimeSpan.FromDays(1))
        .AllowAnyMethod()
        .AllowAnyHeader());
    
    app.UseRouting();

    app.UseAuthentication();
    
    app.UseAuthorization();
    
    app.MapControllers();
    
    app.Run();
}
catch (Exception exception)
{
    logger.Debug(exception, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}