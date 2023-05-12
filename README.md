# ServerBox
A pure net7 scaffold with sqlsugar(mysql)+jwt

# Project
```
    ├───ServerBox.Web
    │   ├───Properties
    │   ├───Content
    │   ├───Controllers
    │   ├───Filters
    │   ├───Middlewares
    │   └───Utils
    ├───appsettings.json
    ├───appsettings.json
    ├───nlog.config
    ├───nlog.Development.config
    └───Program.cs
```
# Dependencies
//TODO:add dependencies
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


