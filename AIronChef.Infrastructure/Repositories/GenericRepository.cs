using AIronChef.Domain.Common;
using AIronChef.Domain.Interfaces;
using AIronChef.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace AIronChef.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);

        if (entity is null)
            return false;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T>? GetByIdAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);

        if (entity is null)
            return null!;

        return entity;
    }

    public async Task<T>? UpdateAsync(T entity)
    {
        if (await _dbSet.FindAsync(entity.Id) is null)
            return null!;

        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
}
