using ServerBox.Core;
using ServerBox.Core.Dto;
using Microsoft.AspNetCore.Mvc;
using ServerBox.Core.Domain;
using ServerBox.Services;

namespace ServerBox.Web.Controllers;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
public abstract class BaseController : Controller
{
    protected DateTime now = DateTime.Now;
    private User _user;

    public User CurrentUser
    {
        get
        {
            if (_user != null) return _user;

            var userService = (UserService)HttpContext.RequestServices.GetService(typeof(UserService));
            _user = userService.GetById(Convert.ToInt64(User.Identity?.Name));
            return _user;
        }
    }

    protected IActionResult SuccessResult(object data = null, string message = "")
    {
        return JsonResult(200, data, message);
    }

    protected IActionResult FailResult(object data, string message)
    {
        return JsonResult(-1, data, message);
    }

    protected IActionResult FailResult(string message = "")
    {
        return JsonResult(-1, null, message);
    }

    protected IActionResult JsonResult(object data, string message)
    {
        return JsonResult(200, data, message);
    }

    protected IActionResult JsonResult(int code, object data, string message = "")
    {
        return Json(new JsonResultModel<object>
        {
            code = code,
            data = data,
            message = message
        });
    }
}

