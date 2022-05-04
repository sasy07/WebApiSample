using WebApiSample.Entities;

namespace WebApiSample.Data.Contracts;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByUserAndPass(string userName, string password, CancellationToken cancellationToken);
}