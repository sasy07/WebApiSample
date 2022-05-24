using WebApiSample.Entities;

namespace WebApiSample.Services.Services;

public interface IJwtService
{
    string Generate(User user);
}