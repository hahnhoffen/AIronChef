using AIronChef.Domain.Models;
using System.Collections.ObjectModel;

namespace AIronChef.Domain.Interfaces
{
    public interface IRecipeRepository : IGenericRepository<Recipe>
    {
        Task<ICollection<Recipe>> GetRecipesByUserIdAsync(int userId, ICollection<Recipe> recipes);
        Task<IEnumerable<Recipe>> SearchRecipesAsync(string keyword);
    }
}
