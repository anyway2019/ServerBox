using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using ServerBox.Web.Extensions;

namespace ServerBox.Web.Middlewares;

public class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context,ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var ip = context.GetClientUserIp();
        var sb = new StringBuilder();
        sb.AppendLine();
        sb.AppendLine(ip);
        sb.AppendLine(DateTime.Now.ToString(CultureInfo.InvariantCulture));
        sb.AppendLine(context.RequestFullPath());
        sb.AppendLine(context.RequestBody());
        sb.AppendLine($"{ex.Source}:{ex.Message}");
        sb.AppendLine(ex.StackTrace);
        if (ex.InnerException != null)
        {
            sb.AppendLine($"InnerException:{ex.InnerException.Message}:{ex.InnerException.StackTrace}");
        }

        _logger.LogError(sb.ToString());
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/json;charset=utf-8";
        var body = new
        {
            code = context.Response.StatusCode,
            msg = "internal error",
            error = ex.Message,
        };
        await context.Response.WriteAsync(JsonConvert.SerializeObject(body), System.Text.Encoding.UTF8);
    }
}