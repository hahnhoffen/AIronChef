using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Application.Recipes.Commands.UpdateRecipe
{
    public class UpdateRecipeHandler : IRequestHandler<UpdateRecipeCommand, OperationResult<Recipe>>
    {
        private readonly IRecipeRepository _recipeRepository;

        public UpdateRecipeHandler(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<OperationResult<Recipe>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
        {
            if (request == null || !request.IsValid())
            {
                return OperationResult<Recipe>.Failure("Invalid command: Recipe ID is not valid.");
            }

            var recipe = await _recipeRepository.GetRecipeByIdAsync(request.Id);

            if (recipe == null)
            {
                return OperationResult<Recipe>.Failure($"Recipe with ID {request.Id} does not exist.");
            }

            recipe.Name = request.Name;
            recipe.Description = request.Description;
            recipe.Ingredients = request.Ingredients;

            try
            {
                await _recipeRepository.UpdateRecipeAsync(recipe);
                return OperationResult<Recipe>.Successfull(recipe);
            }
            catch (Exception ex)
            {
                return OperationResult<Recipe>.Failure($"An error occurred while updating the recipe: {ex.Message}");
            }
        }
    }
}
