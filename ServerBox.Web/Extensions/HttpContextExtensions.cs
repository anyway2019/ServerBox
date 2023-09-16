using System.Text;

namespace ServerBox.Web.Extensions;

public static class HttpContextExtensions
{
    public static string GetClientUserIp(this HttpContext context)
    {
        if (context == null) return "";
        var result = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (string.IsNullOrEmpty(result))
        {
            result = context.Connection.RemoteIpAddress?.ToString();
        }

        if (string.IsNullOrEmpty(result)) return "?:?:?:?";
        if (result.Contains("::1")) result = "127.0.0.1";
        result = result.Replace("::ffff:", "");
        result = result.Split(':')?.FirstOrDefault() ?? "127.0.0.1";
        result = result.IsIp() ? result : "127.0.0.1";
        return result;
    }

    public static string UserAgent(this HttpContext context)
    {
        return context.Request.Headers["User-Agent"];
    }

    public static string RequestFullPath(this HttpContext context)
    {
        var query = context != null ? context.Request.QueryString.Value : "";
        var url = context != null ? context.Request.Path.Value : "";
        return $"{url}/{query}";
    }

    public static string RequestBody(this HttpContext context)
    {
        context.Request.EnableBuffering();
        string body;
        var buffer = new MemoryStream();
        context.Request.Body.Seek(0, SeekOrigin.Begin);
        context.Request.Body.CopyToAsync(buffer);
        buffer.Position = 0;
        try
        {
            using StreamReader streamReader = new(buffer, Encoding.UTF8);
            body = streamReader.ReadToEndAsync().Result;
        }
        finally
        {
            buffer.Dispose();
        }

        return body;
    }
}