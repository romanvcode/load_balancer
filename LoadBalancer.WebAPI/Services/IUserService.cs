using LoadBalancer.WebAPI.Models;

namespace LoadBalancer.WebAPI.Services;

public interface IUserService
{
    Task<AuthenticateResponse?> Authenticate(AuthenticateRequest model);
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetById(int id);
    Task<User?> AddAndUpdateUser(User userObj);
}