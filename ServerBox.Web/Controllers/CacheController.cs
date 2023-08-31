using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ServerBox.Web.Controllers;

public class CacheController:BaseController
{
    private readonly IMemoryCache _memoryCache;

    public CacheController(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    
    [Route("SetCache")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult SetCache()
    {
        const string key = "time";
        var currentDateTime = DateTime.Now;
        
        if (!_memoryCache.TryGetValue(key, out DateTime cacheValue))
        {
            cacheValue = currentDateTime;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(30));

            _memoryCache.Set(key, cacheValue, cacheEntryOptions);
        }

        return SuccessResult();
    }
 

    [Route("GetCache")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult GetCache()
    {
        const string key = "time";
        return _memoryCache.TryGetValue(key, out DateTime cacheValue) ? SuccessResult(cacheValue) : FailResult();
    }
}