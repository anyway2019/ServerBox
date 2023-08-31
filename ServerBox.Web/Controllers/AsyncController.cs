using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServerBox.Services;

namespace ServerBox.Web.Controllers;

public class AsyncController:BaseController
{
    [Route("AsyncTask")]
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> AsyncTask([FromServices] IServiceScopeFactory serviceScopeFactory)
    {
        await Task.Run(() =>
        {
            using var scope = serviceScopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            userService.MockAsyncTask();
        });
        
        return SuccessResult("AsyncTask completed!");
    }
    
    [Route("AsyncTaskWithoutResponse")]
    [AllowAnonymous]
    [HttpGet]
    public IActionResult AsyncTaskWithoutResponse([FromServices] IServiceScopeFactory serviceScopeFactory)
    {
        _ = Task.Run(() =>
        {
            using var scope = serviceScopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<UserService>();
            userService.MockAsyncTask();
        });
        
        return SuccessResult("AsyncTaskWithoutResponse start!");
    }
}