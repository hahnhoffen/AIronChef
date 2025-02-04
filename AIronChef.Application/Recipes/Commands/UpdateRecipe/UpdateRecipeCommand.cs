using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Application.Recipes.Commands.UpdateRecipe
{
    public class UpdateRecipeCommand : IRequest<OperationResult<Recipe>>
    {
        [Required(ErrorMessage = "Recipe ID is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Recipe name is required.")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Ingredients are required.")]
        public List<string> Ingredients { get; set; }

        public UpdateRecipeCommand(Recipe recipe)
        {
            Id = recipe.Id;
            Name = recipe.Name!;
            Description = recipe.Description!;
            Ingredients = recipe.Ingredients!;
        }

        public bool IsValid()
        {
            return ValidationHelper.IsIdValid(Id);
        }
    }
}
