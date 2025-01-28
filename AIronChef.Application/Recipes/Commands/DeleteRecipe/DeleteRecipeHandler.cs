using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Application.Recipes.Commands.DeleteRecipe
{
    public class DeleteRecipeHandler : IRequestHandler<DeleteRecipeCommand, OperationResult<Recipe>>
    {
        private readonly IRecipeRepository _recipeRepository;

        public DeleteRecipeHandler(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<OperationResult<Recipe>> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
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

            try
            {
                await _recipeRepository.DeleteRecipeAsync(request.Id);

                return OperationResult<Recipe>.Successfull(recipe);
            }
            catch (Exception ex)
            {
                return OperationResult<Recipe>.Failure($"An error occurred while deleting the recipe: {ex.Message}");
            }
        }
    }
}
