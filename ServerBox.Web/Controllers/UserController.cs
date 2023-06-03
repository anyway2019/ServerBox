using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Senparc.Weixin.MP.AdvancedAPIs;
using ServerBox.Core.Domain;
using ServerBox.Services;
using ServerBox.Web.Utils;

namespace ServerBox.Web.Controllers;

[Authorize]
public class UserController : BaseController
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserController> _logger;
    private readonly IMemoryCache _memoryCache;

    public UserController(UserService userService, IConfiguration configuration, ILogger<UserController> logger,IMemoryCache memoryCache)
    {
        _userService = userService;
        _configuration = configuration;
        _logger = logger;
        _memoryCache = memoryCache;
    }

    [Route("List")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult List()
    {
        var list = _userService.GetPagedList(0,20);
        return SuccessResult(list);
    }

    [Route("Login")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login(string userName, string password)
    {
        var user = _userService.GetUserByName(userName);
        if (user == null || user.Password != password.ToLower())
            return FailResult("Invalid User");

        var tokenHandler = new JwtSecurityTokenHandler();
        var machineKey = _configuration["JwtKey"] ?? string.Empty;
        var key = Encoding.ASCII.GetBytes(machineKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, "")
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenStr = tokenHandler.WriteToken(token);

        return base.JsonResult(200, new { token = tokenStr, tokenHead = "Bearer" }, "操作成功");
    }

    [Route("Logout")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult Logout()
    {
        return SuccessResult();
    }
    
    [Route("Add")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult Add(string username,string password)
    {
        var user = _userService.GetUserByName(username);
        if(user != null) return FailResult("user already exist");
        user = new User
        {
            Username = username,
            Password = password,
        };
        try
        {
            _userService.Add(user);
        }
        catch (Exception e)
        {
          _logger.LogError(e.Message);
          return FailResult(e.Message);
        }
      
        _logger.LogTrace("add success!");
        return SuccessResult();
    }

    [Route("Test")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult Test()
    {
        _logger.LogTrace("hello test");
        return SuccessResult();
    }
    
    [Route("UploadImage")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult UploadImage(IFormFile file)
    {
        var result = OssHelper.PutStream(file.FileName,file.OpenReadStream(), "","",file.ContentType);
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
    
    [Route("SetCache")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult SetCache()
    {
        var CurrentDateTime = DateTime.Now;
        var key = "time";

        if (!_memoryCache.TryGetValue(key, out DateTime cacheValue))
        {
            cacheValue = CurrentDateTime;

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
        var key = "time";
        if (_memoryCache.TryGetValue(key, out DateTime cacheValue))
        {
            return SuccessResult(cacheValue);
        }
        return FailResult();
    }
    
    [Route("SendPhoneMessage")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult SendPhoneMessage()
    {
        var result = AliMessageHelper.SendCaptcha("15x6x82xxx","6689","86");
        return FailResult(result);
    }

    [Route("WechatLogin")]
    [AllowAnonymous]
    [HttpGet]
    public IActionResult WechatLogin(string code)
    {
        var result = OAuthApi.GetAccessToken(ConfigHelper.WechatAppId, ConfigHelper.WechatAppSecret, code);
        return SuccessResult(result);
    }
}