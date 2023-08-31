using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerBox.Web.Utils;

namespace ServerBox.Web.Controllers;

public class SmsController:BaseController
{
    [Route("SendPhoneMessage")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult SendPhoneMessage()
    {
        var result = AliMessageHelper.SendCaptcha("15x6x82xxx","6689","86");
        return FailResult(result);
    }
}