using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiSample.Data.Contracts;
using WebApiSample.Data.Repositories;
using WebApiSample.Entities;

namespace WebApiSample.MyApi.Controllers;
[Route("api/[controller]")]
[ApiController]
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
    public async Task<List<User>> Get(CancellationToken cancellationToken)
    {
        var users = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);
        return users;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> Get(int id,CancellationToken cancellationToken)
        => await _userRepository.GetByIdAsync(cancellationToken, id);

    [HttpPost]
    public async Task Create(User user,CancellationToken cancellationToken)
    {
        var a = 1111;
        await _userRepository.AddAsync(user, cancellationToken);
    }

    [HttpPut]
    public async Task<IActionResult> Update(int id, User user,CancellationToken cancellationToken)
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
    public async Task<IActionResult> Delete(int id,CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(cancellationToken, id);
        await _userRepository.DeleteAsync(user, cancellationToken);
        return Ok();
    }
    #endregion
}