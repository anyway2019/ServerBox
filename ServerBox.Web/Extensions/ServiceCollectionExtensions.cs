using System.Reflection;
using ServerBox.Web.Middlewares;

namespace ServerBox.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterCustomServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        RegisterConfigurationServices(services,configuration);
        RegisterServicesFromAssembly(services, "ServerBox.Services");
        RegisterMiddleware(services);
        return services;
    }

    private static void RegisterConfigurationServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddHealthChecks().AddMySql(configuration["Data:Conn"] ?? string.Empty);
    }

    //register services by assembly
    private static void RegisterServicesFromAssembly(this IServiceCollection services,string serviceNameSpace)
    {
        var assembly = Assembly.Load(serviceNameSpace);
        var types = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"));
        foreach (var type in types)
        {
            services.AddScoped(type);
        }
    }
    
    //register system middlewares
    private static void RegisterMiddleware(this IServiceCollection services)
    {
        services.AddSingleton<JsonToQueryStringMiddleware>();
        services.AddSingleton<RewriteMiddleware>();
        services.AddSingleton<GlobalExceptionMiddleware>();
    }
}

