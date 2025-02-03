using AIronChef.Domain.Models;

namespace AIronChef.Domain.Interfaces
{
    public interface IRecipeRepository : IGenericRepository<Recipe>
    {
        Task<IEnumerable<Recipe>> GetRecipesByUserIdAsync(int userId);
        Task<IEnumerable<Recipe>> SearchRecipesAsync(string keyword);
    }
}