using System.Security.Claims;
using Common.Exceptions;
using ElmahCore;
using Microsoft.AspNetCore.Authorization;
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

    public UserController(IUserRepository userRepository,
        ILogger<UserController> logger,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _logger = logger;
        _jwtService = jwtService;
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
        _logger.LogInformation("Hello, this is the index!");
        var user = await _userRepository.GetByIdAsync(cancellationToken, id);
        return user != null ? user : NotFound();
    }

    [HttpPost]
    public async Task<ApiResult<User>> Create(UserDto userDto, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Age = userDto.Age,
            FullName = userDto.FullName,
            Gender = userDto.Gender,
            UserName = userDto.UserName
        };
        await _userRepository.AddAsync(user, userDto.Password, cancellationToken);
        return Ok(user);
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
        var user = await _userRepository.GetByUserAndPass(userName, password, cancellationToken);
        if (user == null) throw new BadRequestException("نام کاربری یا رمز عبور اشتباه است .");
        return _jwtService.Generate(user);
    }

    #endregion
}