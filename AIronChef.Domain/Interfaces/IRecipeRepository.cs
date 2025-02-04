using AIronChef.Domain.Models;

namespace AIronChef.Domain.Interfaces;

public interface IRecipeRepository : IGenericRepository<Recipe>
{
    Task<ICollection<Recipe>>? GetUserRecipes(int userId, ICollection<Recipe> recipes);
}
