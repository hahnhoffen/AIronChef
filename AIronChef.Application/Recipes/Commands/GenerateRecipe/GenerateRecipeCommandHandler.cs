using MediatR;
using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using AIronChef.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace AIronChef.Application.Recipes.Commands.GenerateRecipe
{
    public class GenerateRecipeCommandHandler : IRequestHandler<GenerateRecipeCommand, OperationResult<Recipe>>
    {
        private readonly OpenAiService _openAiService;
        private readonly ILogger<GenerateRecipeCommandHandler> _logger;

        public GenerateRecipeCommandHandler(OpenAiService openAiService, ILogger<GenerateRecipeCommandHandler> logger)
        {
            _openAiService = openAiService;
            _logger = logger;
        }

        public async Task<OperationResult<Recipe>> Handle(GenerateRecipeCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generating recipe for ingredients: {Ingredients}", string.Join(", ", request.Ingredients));

            try
            {
                // Call OpenAI service to generate the recipe
                string generatedRecipeText = await _openAiService.SendPromptAsync(
                    request.Ingredients, 
                    "Any", // No user-defined name, AI decides
                    request.MaxCookingTime
                );

                if (string.IsNullOrWhiteSpace(generatedRecipeText))
                {
                    _logger.LogWarning("AI response was empty.");
                    return OperationResult<Recipe>.Failure("Recipe generation failed.");
                }

                // Create Recipe object with AI-generated name and description
                var generatedRecipe = new Recipe
                {
                    Name = "Generated Recipe", // AI can generate this dynamically later
                    Description = generatedRecipeText,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Recipe generated successfully.");
                return OperationResult<Recipe>.Success(generatedRecipe);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while generating recipe.");
                return OperationResult<Recipe>.Failure("An error occurred while generating the recipe.");
            }
        }
    }
}
