using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Recipes.Commands.GenerateRecipe;

public class GenerateRecipeCommand : IRequest<OperationResult<Recipe>>
{
    public string[] Ingredients { get; set; }
    public int MaxCookingTimeMinutes { get; set; }
    public Enum MealType { get; set; }
    
    public GenerateRecipeCommand(string[] ingredients, int maxCookingTimeMinutes, Enum mealType)
    {
        Ingredients = ingredients;
        MaxCookingTimeMinutes = maxCookingTimeMinutes;
        MealType = mealType;
    }
}