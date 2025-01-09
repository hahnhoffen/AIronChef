using System.ComponentModel.DataAnnotations;

namespace AIronChef.Application.Recipes.Commands.GenerateRecipe;

public class GenerateRecipeCommand
{
    // List of ingredients must be provided by the user
    [Required(ErrorMessage = "Ingredients are required")]
    public string Ingredients { get; set; }
}