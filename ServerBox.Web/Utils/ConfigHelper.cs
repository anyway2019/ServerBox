using System.Runtime.CompilerServices;

namespace ServerBox.Web.Utils;

public static class ConfigHelper
{
    public static string ConnectionString { get; set; }
    public static IConfiguration Configuration { get; set; }
    public static string Environment { get; set; }
    public static string OSSHost { get; set; }
    public static string BucketName { get; set; }
    public static string OSSKeyId { get; set; }
    public static string OSSKeySecret{ get; set; }

    public static void RegisterEnvironment(string environment)
    {
        Environment = environment;
    }

    public static void RegisterConfiguration(IConfiguration configuration)
    {
        Configuration = configuration;
        ConnectionString = Configuration["Data:Conn"];
        OSSHost = Configuration["OSS:OSSHost"];
        BucketName = Configuration["OSS:BucketName"];
        OSSKeyId = Configuration["OSS:OSSKeyId"];
        OSSKeySecret = Configuration["OSS:OSSKeySecret"];
    }
}