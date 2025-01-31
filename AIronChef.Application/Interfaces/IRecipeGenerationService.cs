using AIronChef.Domain.Enums;
using AIronChef.Domain.Models;

namespace AIronChef.Application.Interfaces;

public interface IRecipeGenerationService
{
    // <summary>
    /// Generates a recipe based on given ingredients and optional constraints.
    /// </summary>
    /// <param name="ingredients">List of ingredients available.</param>
    /// <param name="maxCookingTime">Optional max cooking time in minutes.</param>
    /// <returns>A generated recipe as a string.</returns>
    Task<Recipe>? GenerateRecipeAsync(IEnumerable<string> ingredients, MealType mealType, int? maxCookingTime);
}