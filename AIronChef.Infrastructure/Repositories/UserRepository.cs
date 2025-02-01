using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using AIronChef.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace AIronChef.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        return await _context.Users.AnyAsync(user => user.Email == email);
    }
}
