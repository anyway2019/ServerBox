# ServerBox
A pure net7 scaffold with sqlsugar(mysql)+jwt

# Project
```
    ├───ServerBox.Core
    ├───ServerBox.Data
    ├───ServerBox.Services
    ├───ServerBox.Web
    │   ├───Properties
    │   ├───Content
    │   ├───Controllers
    │   ├───Extensions
    │   ├───Filters
    │   ├───Middlewares
    │   └───Utils
    ├───appsettings.json
    ├───appsettings.Development.json
    ├───nlog.config
    ├───nlog.Development.config
    └───Program.cs
```
# Dependencies
|  Name   | Version  |
|  ----  | ----  |
| SqlSygarCore  | 5.1.4.104 |
| Aliyun.OSS.SDK.NetCore  | 2.13.0 |
| AspNetCore.HealthChecks.MySql  | 7.0.0-rc2.5 |
| Microsoft.AspNetCore.Authentication.JwtBearer  | 7.0.5 |
| Microsoft.AspNetCore.Authentication.Negotiate  | 7.0.5 |
| Microsoft.AspNetCore.OpenApi  | 7.0.5 |
| Microsoft.Extensions.DependencyInjection  | 7.0.0 |
| Newtonsoft.Json  | 5.1.3 |
| NLog  | 2.13.0 |
| NLog.Web.AspNetCore  | 5.2.3 |
| NPOI  | 2.6.0 |
| Senparc.Weixin  | 6.15.8.7 |
| Swashbuckle.AspNetCore  | 6.4.0 |
| NLog.Web.AspNetCore  | 5.2.3 |
# Middleware

#### note: all middlewares have to register with singleton scope in program.cs

```csharp
private static void RegisterMiddleware(this IServiceCollection services)
{
    services.AddSingleton<JsonToQueryStringMiddleware>();
    services.AddSingleton<RewriteMiddleware>();
}
```
- <strong>RewriteMiddleware</strong>: e.g.  //route/action =》 /route/action 

```csharp
    app.UseMiddleware<RewriteMiddleware>();//must before UseRouting()
    
    app.UseRouting();
```
- <strong>JsonToQueryStringMiddleware</strong>:  convert post json body to querystring on server


