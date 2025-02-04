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

namespace AIronChef.Application.Recipes.Queries.GetAllRecipes
{
    public class GetAllRecipesHandler : IRequestHandler<GetAllRecipesQuery, OperationResult<List<Recipe>>>
    {
        private readonly IGenericRepository<Recipe> _recipeRepository;
        private readonly ILoggingService _loggingService;

        public GetAllRecipesHandler(IGenericRepository<Recipe> recipeRepository, ILoggingService loggingService)
        {
            _recipeRepository = recipeRepository;
            _loggingService = loggingService;
        }

        public async Task<OperationResult<List<Recipe>>> Handle(GetAllRecipesQuery request, CancellationToken cancellationToken)
        {
            _loggingService.LogInfo("Fetching all recipes.");

            try
            {
                var recipes = await _recipeRepository.GetAllAsync();

                if (recipes is null || !recipes.Any())
                {
                    _loggingService.LogWarning("No recipes found.");
                    return OperationResult<List<Recipe>>.Failure("No recipes found.");
                }

                _loggingService.LogInfo("Successfully fetched all recipes.");
                return OperationResult<List<Recipe>>.Successfull(recipes.ToList());
            }
            catch (Exception ex)
            {
                _loggingService.LogError("An error occurred while fetching recipes.", ex);
                return OperationResult<List<Recipe>>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
