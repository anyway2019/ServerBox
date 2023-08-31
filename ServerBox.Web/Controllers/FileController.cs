using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerBox.Web.Utils;

namespace ServerBox.Web.Controllers;

public class FileController : BaseController
{
    [Route("UploadImage")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult UploadImage(IFormFile file)
    {
        var result = OssHelper.PutStream(file.FileName, file.OpenReadStream(), "", "", file.ContentType);
        return SuccessResult(result);
    }

    [Route("UploadText")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult UploadText()
    {
        var result = OssHelper.PutString("helloworld.txt", "hello world! 123");
        return SuccessResult(result);
    }
}