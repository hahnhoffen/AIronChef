using AIronChef.Domain.Models;

namespace AIronChef.Domain.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> IsEmailUniqueAsync(string email);
    Task<User> GetUserByEmail(string email);
}
