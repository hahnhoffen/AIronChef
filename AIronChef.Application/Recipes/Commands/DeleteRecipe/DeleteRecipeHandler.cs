using AIronChef.Application.Common.Helpers;
using AIronChef.Application.Interfaces;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Recipes.Commands.DeleteRecipe
{
    public class DeleteRecipeHandler : IRequestHandler<DeleteRecipeCommand, OperationResult<Recipe>>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly ILoggingService _loggingService;

        public DeleteRecipeHandler(IRecipeRepository recipeRepository, ILoggingService loggingService)
        {
            _recipeRepository = recipeRepository;
            _loggingService = loggingService;
        }

        public async Task<OperationResult<Recipe>> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
        {
            if (request == null || !request.IsValid())
            {
                _loggingService.LogWarning("Invalid command: Recipe ID is not valid.");
                return OperationResult<Recipe>.Failure("Invalid command: Recipe ID is not valid.");
            }

            var recipe = await _recipeRepository.GetByIdAsync(request.Id);
            if (recipe == null)
            {
                _loggingService.LogWarning($"Recipe with ID {request.Id} does not exist.");
                return OperationResult<Recipe>.Failure($"Recipe with ID {request.Id} does not exist.");
            }

            try
            {
                var deleteSuccess = await _recipeRepository.DeleteAsync(request.Id);
                if (!deleteSuccess)
                {
                     _loggingService.LogError($"An error occurred while deleting the recipe.", new Exception("Fuck around and find out"));
                    return OperationResult<Recipe>.Failure("Failed to delete the recipe.");
                }

                _loggingService.LogInfo($"Recipe with ID {request.Id} deleted successfully.");
                return OperationResult<Recipe>.Successfull(recipe);
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"An error occurred while deleting the recipe.", ex);
                return OperationResult<Recipe>.Failure($"An error occurred while deleting the recipe: {ex.Message}");
            }
        }
    }
}
