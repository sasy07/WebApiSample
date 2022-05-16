using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSample.Data.Contracts;
using WebApiSample.Entities;
using WebApiSample.MyApi.Models;
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

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    #endregion

    #region CRUD

    [HttpGet]
    public async Task<ActionResult<List<User>>> Get(CancellationToken cancellationToken)
        => Ok(await _userRepository.TableNoTracking.ToListAsync(cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<ApiResult<User>> Get(int id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(cancellationToken, id);
        return user != null ? user : NotFound();
    }

    [HttpPost]
    public async Task<ApiResult<User>> Create(UserDto userDto, CancellationToken cancellationToken)
    {
        var exists = await _userRepository.TableNoTracking.AnyAsync(p => p.UserName == userDto.UserName);
        if (exists) return BadRequest("نام کاربری تکراری است"); 
        
        var user = new User
        {
            Age = userDto.Age,
            FullName = userDto.FullName,
            Gender = userDto.Gender,
            UserName = userDto.UserName
        };
        await _userRepository.AddAsync(user,userDto.Password, cancellationToken);
        return Ok(user);
    }

    [HttpPut]
    public async Task<ApiResult> Update(int id , User user, CancellationToken cancellationToken)
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

    #endregion
    
}