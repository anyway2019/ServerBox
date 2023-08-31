using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.MP.AdvancedAPIs;
using ServerBox.Web.Utils;

namespace ServerBox.Web.Controllers;

public class WechatController : BaseController
{
    [Route("WechatLogin")]
    [AllowAnonymous]
    [HttpGet]
    public IActionResult WechatLogin(string code)
    {
        var result = OAuthApi.GetAccessToken(ConfigHelper.WechatAppId, ConfigHelper.WechatAppSecret, code);
        return SuccessResult(result);
    }
}