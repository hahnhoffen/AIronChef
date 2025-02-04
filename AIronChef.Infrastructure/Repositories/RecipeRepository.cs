using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using AIronChef.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace AIronChef.Infrastructure.Repositories
{
    public class RecipeRepository : GenericRepository<Recipe>, IRecipeRepository
    {
        private readonly AppDbContext _context;

        public RecipeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ICollection<Recipe>> GetRecipesByUserIdAsync(int userId, ICollection<Recipe> recipes)
        {
            recipes = await _context.Recipes.Where(r => r.UserId == userId).ToListAsync();
            return recipes;
        }

        public async Task<IEnumerable<Recipe>> SearchRecipesAsync(string keyword)
        {
            return await _context.Recipes
                .Where(r => r.Name.Contains(keyword) || r.Description.Contains(keyword))
                .ToListAsync();
        }
    }
}
