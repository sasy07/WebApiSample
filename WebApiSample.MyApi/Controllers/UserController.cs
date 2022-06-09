using System.Security.Claims;
using Common.Exceptions;
using ElmahCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSample.Common.Utility;
using WebApiSample.Data.Contracts;
using WebApiSample.Entities;
using WebApiSample.MyApi.Models;
using WebApiSample.Services.Services;
using WebApiSample.WebFramework.Api;
using WebApiSample.WebFramework.Filters;

namespace WebApiSample.MyApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiResultFilter]
public class UserController : ControllerBase
{
    #region constructor

    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserController> _logger;
    private readonly IJwtService _jwtService;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly SignInManager<User> _signInManager;

    public UserController(IUserRepository userRepository,
        ILogger<UserController> logger,
        IJwtService jwtService,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        SignInManager<User> signInManager)
    {
        _userRepository = userRepository;
        _logger = logger;
        _jwtService = jwtService;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _logger.LogError(1, "NLog injected into HomeController");
    }

    #endregion

    #region CRUD

    [HttpGet]
    public async Task<ActionResult<List<User>>> Get(CancellationToken cancellationToken)
    {
        var userName = HttpContext.User.Identity.GetUserName();
        var userId = HttpContext.User.Identity.GetUserId<int>();
        var role = HttpContext.User.Identity.FindFirstValue(ClaimTypes.Role);
        return Ok(await _userRepository.TableNoTracking.ToListAsync(cancellationToken));
    }


    [HttpGet("{id:int}")]
    public async Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
    {
        var user2 = await _userManager.FindByIdAsync(id.ToString());
        _logger.LogInformation("Hello, this is the index!");
        var user = await _userRepository.GetByIdAsync(cancellationToken, id);
        return user != null ? user : NotFound();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ApiResult<User>> Create(UserDto userDto, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Age = userDto.Age,
            FullName = userDto.FullName,
            Gender = userDto.Gender,
            UserName = userDto.UserName,
            Email = userDto.Email
        };
        var result_user = await _userManager.CreateAsync(user, userDto.Password);
       
        return Ok(result_user);
    }

    [HttpPut]
    public async Task<ApiResult> Update(int id, User user, CancellationToken cancellationToken)
    {
        var updateUser = await _userRepository.GetByIdAsync(cancellationToken, id);

        updateUser.UserName = user.UserName;
        updateUser.PasswordHash = user.PasswordHash;
        updateUser.FullName = user.FullName;
        updateUser.Age = user.Age;
        updateUser.Gender = user.Gender;
        updateUser.IsActive = user.IsActive;
        updateUser.LastLoginDate = user.LastLoginDate;

        await _userRepository.UpdateAsync(updateUser, cancellationToken);

        return Ok();
    }

    [HttpDelete]
    public async Task<ApiResult> Delete(int id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(cancellationToken, id);
        await _userRepository.DeleteAsync(user, cancellationToken);
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("[action]")]
    public async Task<string> Token(string userName, string password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user == null) 
            throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است .");
        if(!await _userManager.CheckPasswordAsync(user, password))
            throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است .");
        return await _jwtService.GenerateAsync(user);
    }

    #endregion
}