using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dysmsapi.Model.V20170525;

namespace ServerBox.Web.Utils;

public class AliMessageHelper
{
    private const string product = "Dysmsapi"; 
    private const string domain = "dysmsapi.aliyuncs.com";
    
    private static IClientProfile Profile
    {
        get
        {
            var result = DefaultProfile.GetProfile("cn-hangzhou", ConfigHelper.SmsAccessKeyId, ConfigHelper.SmsAccessKeySecret);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", product, domain);
            return result;
        }
    }

    /// <summary>
    /// send captcha to phone
    /// </summary>
    public static string SendCaptcha(string recNum, string code, string country)
    {
        switch (country)
        {
            case "86":
                return Send(recNum, ConfigHelper.SmsCaptchaTemplateCode, new
                {
                    code
                });
            default:
            {
                var recNumbers = string.Join(",", recNum.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(r => country + r));
                return Send(recNumbers, ConfigHelper.SmsCaptchaTemplateCodeEn, new
                {
                    code
                });
            }
        }
    }

    private static string Send(string phone, string templateCode, object data)
    {
        var acsClient = new DefaultAcsClient(Profile);
        var request = new SendSmsRequest();
        try
        {
            request.PhoneNumbers = phone;
            request.SignName = "PowerFun";
            request.TemplateCode = templateCode;

            request.TemplateParam = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            var sendSmsResponse = acsClient.GetAcsResponse(request);
            return sendSmsResponse.Message;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }
}

