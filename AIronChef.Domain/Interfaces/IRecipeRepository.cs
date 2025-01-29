using AIronChef.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Domain.Interfaces
{
    public interface IRecipeRepository
    {
        Task<Recipe> GetRecipeByIdAsync(int id);
        Task<IEnumerable<Recipe>> GetAllRecipesAsync();
        Task<Recipe> AddRecipeAsync(Recipe recipe);
        Task<Recipe> UpdateRecipeAsync(Recipe recipe);
        Task<Recipe> DeleteRecipeAsync(int id);
    }
}
