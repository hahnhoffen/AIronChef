using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using AIronChef.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace AIronChef.Infrastructure.Repositories;

public class RecipeRepository : GenericRepository<Recipe>, IRecipeRepository
{
    private readonly AppDbContext _context;

    public RecipeRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ICollection<Recipe>>? GetUserRecipes(int userId, ICollection<Recipe> recipes)
    {
        recipes = await _context.Recipes.Where(r => r.UserId == userId).ToListAsync();

        return recipes;
    }
}
