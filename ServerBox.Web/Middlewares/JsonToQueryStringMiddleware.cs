using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ServerBox.Web.Middlewares;

public class JsonToQueryStringMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Method == "POST" && context.Request.ContentType?.StartsWith("application/json") == true)
        {
            //如果接口参数定义为[FromBody] 就不转化成QueryString
            var endpoint = context.GetEndpoint();
            var method = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
            // 方法标记了[FromBody]特性,从请求体读取参数
            var parameters = method?.Parameters.Where(p => p.BindingInfo != null && p.BindingInfo.BindingSource == BindingSource.Body);
            if (parameters != null && parameters.Any())
            {
                await next(context);
                return;
            }
          
            var json = await ReadRequestBodyAsync(context.Request);
            var queryString = ConvertJsonToQueryString(json);

            if (!string.IsNullOrEmpty(queryString))
            {
                context.Request.QueryString = new QueryString(queryString);
            }
        }

        await next(context);
    }

    private async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body);
        return await reader.ReadToEndAsync();
    }

    private string ConvertJsonToQueryString(string json)
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var obj = JsonSerializer.Deserialize<object>(json, options);

        if (obj is not JsonElement jsonElement)
        {
            return null;
        }

        var sb = new StringBuilder();

        foreach (var property in jsonElement.EnumerateObject())
        {
            var encodedName = Uri.EscapeDataString(property.Name);

            switch (property.Value.ValueKind)
            {
                case JsonValueKind.String:
                    var value = property.Value.GetString();
                    var encodedValue = Uri.EscapeDataString(value);
                    SafeAppend(sb,$"{encodedName}={encodedValue}");
                    break;

                case JsonValueKind.Number:
                    if (property.Value.TryGetInt32(out var intValue))
                    {
                        SafeAppend(sb,$"{encodedName}={intValue}");
                    }
                    else if (property.Value.TryGetDouble(out var doubleValue))
                    {
                        SafeAppend(sb,$"{encodedName}={doubleValue}");
                    }
                    else if (property.Value.TryGetDecimal(out var decimalValue))
                    {
                        SafeAppend(sb,$"{encodedName}={decimalValue}");
                    }
                    else if (property.Value.TryGetInt64(out var longValue))
                    {
                        SafeAppend(sb,$"{encodedName}={longValue}");
                    }
                    break;

                case JsonValueKind.True:
                    SafeAppend(sb,$"{encodedName}=true");
                    break;

                case JsonValueKind.False:
                    SafeAppend(sb,$"{encodedName}=false");
                    break;

                case JsonValueKind.Null:
                    SafeAppend(sb,$"{encodedName}=");
                    break;

                case JsonValueKind.Object:
                case JsonValueKind.Array:
                case JsonValueKind.Undefined:
                default:
                    break;
            }
        }

        return sb.ToString();
    }

    private void SafeAppend(StringBuilder sb,string content)
    {
        content = sb.Length == 0 ? $"?{content}": $"&{content}";
        sb.Append(content);
    }
}