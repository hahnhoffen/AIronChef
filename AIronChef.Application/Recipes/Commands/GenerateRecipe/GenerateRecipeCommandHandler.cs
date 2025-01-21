using System.ComponentModel.DataAnnotations;
using MediatR;

namespace AIronChef.Application.Recipes.Commands.GenerateRecipe;

public class GenerateRecipeCommandHandler : IRequestHandler<GenerateRecipeCommand, OperationResult<Recipe>>
{
    // List of ingredients must be provided by the user
    [Required(ErrorMessage = "Ingredients are required")]
    public string Ingredients { get; set; }
}