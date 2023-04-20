namespace ServerBox.Web.Utils;

public static class ConfigHelper
{
    public static string ConnectionString { get; set; }
    public static IConfiguration Configuration { get; set; }

    public static void RegisterConfiguration(IConfiguration configuration)
    {
        Configuration = configuration;
        ConnectionString = Configuration["Data:Conn"];
    }
}