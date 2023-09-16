using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Senparc.Weixin.AspNet;
using Senparc.Weixin.RegisterServices;
using ServerBox.Web.Extensions;
using ServerBox.Web.Middlewares;
using ServerBox.Web.Utils;
using SqlSugar;


var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var logger = LogManager.Setup().LoadConfigurationFromFile($"nlog.{env}.config").GetCurrentClassLogger();
logger.Debug("server is starting.");
try
{
    var builder = WebApplication.CreateBuilder(args);
    var configuration = builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"appsettings.json", optional: false)
        .AddJsonFile($"appsettings.{env}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();
    
    //NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    // read config
    ConfigHelper.RegisterEnvironment(env);
    ConfigHelper.RegisterConfiguration(configuration);
    // Add Response Compression
    builder.Services.AddResponseCompression();
    // Add memory cache services
    builder.Services.AddMemoryCache();
    // Add services to the container.
    builder.Services.RegisterCustomServices(configuration);
    // Add SenparcWeixinServices
    builder.Services.AddSenparcWeixinServices(builder.Configuration);
    // Add sqlsugar
    var connectionString = builder.Configuration["Data:Conn"];

    //generate entity from connection string.
    //EntityGenerator.Build(connectionString,"ServerBox.Core.Domain");
    builder.Services.AddScoped<SqlSugarClient>(sp =>
    {
        var connectionConfig = new ConnectionConfig
        {
            ConnectionString = connectionString,
            DbType = DbType.MySql,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        };
        return new SqlSugarClient(connectionConfig, (d) =>
        {
            //TODO:other Advanced config
        });
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
    var key = Encoding.ASCII.GetBytes(configuration["JwtKey"]);
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

    app.UseSenparcWeixin(app.Environment, null, null, register => { }, (register, setting) => { });

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseHealthChecks("/health");
         
    app.UseCookiePolicy();

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
                           Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
    });

    app.UseStaticFiles();
    
    app.UseCors(x => x
        .AllowAnyOrigin().SetPreflightMaxAge(TimeSpan.FromDays(1))
        .AllowAnyMethod()
        .AllowAnyHeader());
    
    app.UseMiddleware<RewriteMiddleware>();//rewrite url before routing
    
    app.UseRouting();

    app.UseAuthentication();
    
    app.UseAuthorization();
    
    app.MapControllers();
    
    //convert post request body which is json object to query string
    app.UseWhen(context => context.Request.Method.Equals("POST"),
        _ => { app.UseMiddleware<JsonToQueryStringMiddleware>(); });
    
    app.UseMiddleware<GlobalExceptionMiddleware>();
    
    app.Run();
}
catch (Exception exception)
{
    logger.Debug(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}