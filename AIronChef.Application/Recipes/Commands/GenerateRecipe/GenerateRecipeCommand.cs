using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Recipes.Commands.GenerateRecipe;

public class GenerateRecipeCommand : IRequest<OperationResult<Recipe>>
{
    public string[] Ingredients { get; set; }
    public int MaxCookingTime { get; set; }

    public GenerateRecipeCommand(string[] ingredients, int maxCookingTime)
    {
        Ingredients = ingredients;
        MaxCookingTime = maxCookingTime;
    }
}
