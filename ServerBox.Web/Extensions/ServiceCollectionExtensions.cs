using ServerBox.Services;

namespace ServerBox.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterCustomServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<UserService>();
        services.AddHealthChecks().AddMySql(configuration["Data:Conn"]);
        
        return services;
    }
}

