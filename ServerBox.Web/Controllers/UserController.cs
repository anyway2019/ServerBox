﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using ServerBox.Core.Domain;
using ServerBox.Services;

namespace ServerBox.Web.Controllers;

[Authorize]
public class UserController : BaseController
{
    private readonly UserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserController> _logger;

    public UserController(UserService userService, IConfiguration configuration, ILogger<UserController> logger)
    {
        _userService = userService;
        _configuration = configuration;
        _logger = logger;
    }

    [Route("List")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult List()
    {
        var list = _userService.GetPagedList(0, 20);
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
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
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
    public IActionResult Add(string username, string password)
    {
        var user = _userService.GetUserByName(username);
        if (user != null) return FailResult("user already exist");

        try
        {
            _userService.Add(new User
            {
                Username = username,
                Password = password,
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return FailResult(e.Message);
        }

        _logger.LogTrace("add success!");
        return SuccessResult();
    }

    [Route("Echo")]
    [AllowAnonymous]
    [HttpPost]
    public IActionResult Echo()
    {
        _logger.LogTrace("hello test");
        return SuccessResult("hello test");
    }
}