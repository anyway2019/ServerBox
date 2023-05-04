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
    
    public static string SmsAccessKeyId{ get; set; }
    public static string SmsAccessKeySecret{ get; set; }
    public static string SmsSignName{ get; set; }
    public static string SmsCaptchaTemplateCode{ get; set; }
    public static string SmsCaptchaTemplateCodeEn{ get; set; }
    public static string WechatAppId{ get; set; }
    public static string WechatAppSecret{ get; set; }
    
    public static string WechatOfficialAccount{ get; set; }

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
        SmsAccessKeyId = Configuration["Sms:AccessKeyId"];
        SmsAccessKeySecret = Configuration["Sms:AccessKeySecret"];
        SmsSignName = Configuration["Sms:SignName"];
        SmsCaptchaTemplateCode = Configuration["Sms:CaptchaTemplateCode"];
        SmsCaptchaTemplateCodeEn = Configuration["Sms:CaptchaTemplateCodeEn"];
        WechatAppId = Configuration["Wechat:WechatAppId"];
        WechatAppSecret = Configuration["Wechat:WechatAppSecret"];
        WechatOfficialAccount = Configuration["Wechat:WechatOfficialAccount"];
    }
}