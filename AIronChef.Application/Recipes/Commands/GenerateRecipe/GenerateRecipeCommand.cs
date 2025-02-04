using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Enums;
using MediatR;
using AIronChef.Application.DTOs;
using AIronChef.Domain.Models;

namespace AIronChef.Application.Recipes.Commands.GenerateRecipe;

public class GenerateRecipeCommand : IRequest<OperationResult<Recipe>>
{
    public ICollection<string> Ingredients { get; set; }
    public int? MaxCookingTimeMinutes { get; set; }
    public MealType MealType { get; set; }
    public int UserId { get; set; }
    
    public GenerateRecipeCommand(ICollection<string> ingredients, int? maxCookingTimeMinutes, MealType mealType, int userId)
    {
        Ingredients = ingredients;
        MaxCookingTimeMinutes = maxCookingTimeMinutes;
        MealType = mealType;
        UserId = userId;
    }
}