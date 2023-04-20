using Microsoft.Extensions.DependencyInjection.Extensions;
using SqlSugar;

namespace ServerBox.Web.Extensions;

public static class SqlSugarExtensions
{

    public static IServiceCollection AddSqlSugarClient<TSugarContext>(this IServiceCollection serviceCollection,
        Action<ConnectionConfig> configAction, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        where TSugarContext : SqlSugarClient
    {
        return AddSqlSugarClient<TSugarContext>(serviceCollection, (p, b) => configAction.Invoke(b), lifetime);
    }

    /// <summary>
    /// SqlSugar上下文注入
    /// </summary>
    /// <typeparam name="TSugarContext">要注册的上下文的类型</typeparam>
    /// <param name="serviceCollection"></param>
    /// <param name="configAction"></param>
    /// <param name="lifetime">用于在容器中注册TSugarClient服务的生命周期</param>
    /// <returns></returns>
    private static IServiceCollection AddSqlSugarClient<TSugarContext>(this IServiceCollection serviceCollection,
        Action<IServiceProvider, ConnectionConfig> configAction, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TSugarContext : SqlSugarClient
    {
        //serviceCollection.AddMemoryCache().AddLogging();
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(ConnectionConfig),
            p => ConnectionConfigFactory(p, configAction), lifetime));
        serviceCollection.Add(new ServiceDescriptor(typeof(ConnectionConfig),
            p => ConnectionConfigFactory(p, configAction), lifetime));
        serviceCollection.TryAdd(new ServiceDescriptor(typeof(TSugarContext), typeof(TSugarContext), lifetime));
        return serviceCollection;
    }

    private static ConnectionConfig ConnectionConfigFactory(IServiceProvider applicationServiceProvider,
        Action<IServiceProvider, ConnectionConfig> configAction)
    {
        var config = new ConnectionConfig();
        configAction.Invoke(applicationServiceProvider, config);
        return config;
    }
}
