using MediatR;
using AIronChef.Application.Common.Helpers;
using AIronChef.Application.Interfaces;
using AIronChef.Domain.Models;
using Microsoft.Extensions.Logging;

namespace AIronChef.Application.Recipes.Commands.GenerateRecipe
{
    public class GenerateRecipeCommandHandler : IRequestHandler<GenerateRecipeCommand, OperationResult<Recipe>>
    {
        private readonly IRecipeGenerationService _recipeGenerationService;
        private readonly ILogger<GenerateRecipeCommandHandler> _logger;

        public GenerateRecipeCommandHandler(IRecipeGenerationService recipeGenerationService, ILogger<GenerateRecipeCommandHandler> logger)
        {
            _recipeGenerationService = recipeGenerationService ?? throw new ArgumentNullException(nameof(recipeGenerationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<OperationResult<Recipe>> Handle(GenerateRecipeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generating recipe for ingredients: {Ingredients}, Meal Type: {MealType}, Max Cooking Time: {MaxCookingTime}", 
                request.Ingredients != null && request.Ingredients.Any() ? string.Join(", ", request.Ingredients) : "None",
                request.MealType,
                request.MaxCookingTimeMinutes.HasValue ? $"{request.MaxCookingTimeMinutes} minutes" : "No limit");

            try
            {
                var generatedRecipe = await _recipeGenerationService.GenerateRecipeAsync(
                    request.Ingredients,
                    request.MealType,
                    request.MaxCookingTimeMinutes
                );

                if (generatedRecipe == null)
                {
                    _logger.LogWarning("AI response was empty or invalid.");
                    return OperationResult<Recipe>.Failure("Recipe generation failed.");
                }

                _logger.LogInformation("Recipe generated successfully: {RecipeName}", generatedRecipe.Name);
                return OperationResult<Recipe>.Successfull(generatedRecipe); // Fixed typo
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while generating recipe with ingredients: {Ingredients}", 
                    request.Ingredients != null && request.Ingredients.Any() ? string.Join(", ", request.Ingredients) : "None");

                return OperationResult<Recipe>.Failure("An error occurred while generating the recipe.");
            }
        }
    }
}