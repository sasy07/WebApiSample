using Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using WebApiSample.Common.Utility;
using WebApiSample.Data.Contracts;
using WebApiSample.Entities;

namespace WebApiSample.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
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


    public async Task AddAsync(User user, string password, CancellationToken cancellationToken)
    {
        var exists = await TableNoTracking.AnyAsync(p => p.UserName == user.UserName);
        if (exists)
            throw new BadRequestException("نام کاربری تکراری است");
        user.PasswordHash = SecurityHelper.GetSha256Hash(password);
        await base.AddAsync(user, cancellationToken);
    }

    public Task UpdateSecurityStampAsync(User user, CancellationToken cancellationToken)
    {
        // user.SecurityStamp = Guid.NewGuid();
        return UpdateAsync(user, cancellationToken);
    }

    public Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken)
    {
        user.LastLoginDate = DateTimeOffset.Now;
        return UpdateAsync(user, cancellationToken);
    }
}