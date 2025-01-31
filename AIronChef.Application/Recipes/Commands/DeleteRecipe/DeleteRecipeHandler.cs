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
        private readonly ILoggingService _loggingService;

        public DeleteRecipeHandler(IRecipeRepository recipeRepository, ILoggingService _loggingService)
        {
            _recipeRepository = recipeRepository;
            _loggingService = _loggingService;
        }

        public async Task<OperationResult<Recipe>> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
        {

            if (request == null || !request.IsValid())
            {
                return OperationResult<Recipe>.Failure("Invalid command: Recipe ID is not valid.");
                _loggingService.LogWarning("Invalid command: Recipe ID is not valid.");
            }

            var recipe = await _recipeRepository.GetRecipeByIdAsync(request.Id);
            if (recipe == null)
            {
                return OperationResult<Recipe>.Failure($"Recipe with ID {request.Id} does not exist.");
                _loggingService.LogWarning($"Recipe with ID {request.Id} does not exist.");
            }

            try
            {
                await _recipeRepository.DeleteRecipeAsync(request.Id);

                return OperationResult<Recipe>.Successfull(recipe);
                _loggingService.LogInformation($"Recipe with ID {request.Id} deleted successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult<Recipe>.Failure($"An error occurred while deleting the recipe: {ex.Message}");
                _loggingService.LogError(ex, $"An error occurred while deleting the recipe.");
            }
        }
    }
}
