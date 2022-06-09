using WebApiSample.Entities;

namespace WebApiSample.Services.Services;

public interface IJwtService
{
    Task<string> GenerateAsync(User user);
}