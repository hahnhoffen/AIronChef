using AIronChef.Application.Common.Helpers;
using AIronChef.Application.Interfaces;
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
        private readonly IGenericRepository<Recipe> _recipeRepository;
        private readonly ILoggingService _loggingService;

        public UpdateRecipeHandler(IGenericRepository<Recipe> recipeRepository, ILoggingService loggingService)
        {
            _recipeRepository = recipeRepository;
            _loggingService = loggingService;
        }

        public async Task<OperationResult<Recipe>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
        {
            if (request == null || !request.IsValid())
            {
                return OperationResult<Recipe>.Failure("Invalid command: Recipe ID is not valid.");
            }

            var recipe = await _recipeRepository.GetByIdAsync(request.Id)!;

            if (recipe == null)
            {
                _loggingService.LogWarning($"Recipe with ID {request.Id} does not exist.");
                return OperationResult<Recipe>.Failure($"Recipe with ID {request.Id} does not exist.");
            }

            recipe.Name = request.Name;
            recipe.Description = request.Description;
            recipe.Ingredients = request.Ingredients;

            try
            {
                await _recipeRepository.UpdateAsync(recipe)!;
                _loggingService.LogInfo($"Recipe with ID {request.Id} has been updated successfully.");
                return OperationResult<Recipe>.Successfull(recipe);
            }
            catch (Exception ex)
            {
                _loggingService.LogWarning($"An error occurred while updating the recipe: {ex.Message}");
                return OperationResult<Recipe>.Failure($"An error occurred while updating the recipe: {ex.Message}");
            }
        }
    }
}
