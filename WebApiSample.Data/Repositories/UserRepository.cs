using Microsoft.EntityFrameworkCore;
using WebApiSample.Common.Utility;
using WebApiSample.Entities;

namespace WebApiSample.Data.Repositories;

public class UserRepository : Repository<User>
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
}