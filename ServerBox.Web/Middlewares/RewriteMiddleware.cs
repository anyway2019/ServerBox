namespace ServerBox.Web.Middlewares;

public class RewriteMiddleware : IMiddleware
{
    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path.Value != null && context.Request.Path.Value.Contains("//"))
        {
            context.Request.Path = context.Request.Path.Value.Replace("//", "/");
        }

        return next(context);
    }
}