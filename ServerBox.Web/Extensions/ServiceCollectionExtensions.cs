using ServerBox.Services;
using ServerBox.Web.Middlewares;

namespace ServerBox.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterCustomServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<UserService>();
        services.AddHealthChecks().AddMySql(configuration["Data:Conn"]);
        RegisterMiddleware(services);
        return services;
    }
    
    private static void RegisterMiddleware(this IServiceCollection services)
    {
        services.AddSingleton<JsonToQueryStringMiddleware>();
        services.AddSingleton<RewriteMiddleware>();
    }
}

