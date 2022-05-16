using Microsoft.EntityFrameworkCore;
using WebApiSample.Common.Utility;
using WebApiSample.Data.Contracts;
using WebApiSample.Entities;

namespace WebApiSample.Data.Repositories;

public class UserRepository :Repository<User>,  IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User> GetByUserAndPass(string userName, string password, CancellationToken cancellationToken)
    {
        var passwordHash = SecurityHelper.GetSha256Hash(password);
        return await Table.Where(p => p.UserName == userName && p.PasswordHash == passwordHash)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public Task AddAsync(User user, string password, CancellationToken cancellationToken)
    {
        user.PasswordHash = SecurityHelper.GetSha256Hash(password);
        return base.AddAsync(user, cancellationToken);
    } 
}