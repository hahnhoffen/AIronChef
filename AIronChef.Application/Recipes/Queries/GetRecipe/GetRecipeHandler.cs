using AIronChef.Application.Common.Helpers;
using AIronChef.Application.Users.Queries.GetUser;
using AIronChef.Application.Interfaces;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Application.Recipes.Queries.GetRecipe
{
    public class GetRecipeHandler : IRequestHandler<GetRecipeQuery, OperationResult<Recipe>>
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly ILoggingService _loggingService;
        public GetRecipeHandler(IRecipeRepository recipeRepository, ILoggingService loggingService)
        {
            _recipeRepository = recipeRepository;
            _loggingService = loggingService;
        }

        public async Task<OperationResult<Recipe>> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
        {
            _loggingService.LogInfo($"Fetching recipe with id: {request.Id}");

            if (request.Id <= 0)
            {
                _loggingService.LogWarning("Id must be greater than zero.");
                return OperationResult<Recipe>.Failure("Id must be greater than zero.");
            }

            try
            {
                var recipe = await _recipeRepository.GetRecipeByIdAsync(request.Id);

                if (recipe is null)
                {
                    _loggingService.LogWarning($"Recipe with id {request.Id} not found.");
                    return OperationResult<Recipe>.Failure("Recipe not found.");
                }

                _loggingService.LogInfo($"Successfully fetched recipe with id: {request.Id}");
                return OperationResult<Recipe>.Successfull(recipe);
            }
            catch (Exception ex)
            {
                _loggingService.LogError("An error occured while fetching the recipe.", ex);
                return OperationResult<Recipe>.Failure($"An error occurred while retrieving the recipe: {ex.Message}");
            }
        }
    }
}
