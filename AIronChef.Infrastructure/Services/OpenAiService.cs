using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Polly;
using Polly.Retry;
using AIronChef.Domain.Models;
using AIronChef.Domain.Enums;
using AIronChef.Application.Interfaces;

namespace AIronChef.Infrastructure.Services
{
    public class OpenAiService : IRecipeGenerationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenAiService> _logger;
        private readonly string _apiKey;

        public OpenAiService(HttpClient httpClient, ILogger<OpenAiService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")!;

            if (string.IsNullOrEmpty(_apiKey))
                throw new InvalidOperationException("No API key found");
        }

        /// <summary>
        /// Generates a recipe based on the provided details.
        /// </summary>
        /// <param name="ingredients">An array of ingredients.</param>
        /// <param name="mealType">The type of meal (e.g., breakfast, lunch, dinner).</param>
        /// <param name="maxCookingTimeMinutes">The maximum cooking time in minutes.</param>
        /// <returns>The generated recipe as a string.</returns>
        public async Task<Recipe>? GenerateRecipeAsync(IEnumerable<string> ingredients, MealType mealType, int? maxCookingTime)
        {
            // Validate user input.
            if (ingredients == null || !ingredients.Any())
            {
                _logger.LogWarning("Invalid input: No ingredients provided.");
                return null!;
            }

            if (maxCookingTime <= 0)
            {
                _logger.LogWarning("Invalid input: Max cooking time is not valid.");
                return null!;
            }

            // Define the system and user prompt
            var systemPrompt = "You are a professional chef. Create a recipe based on the provided ingredients, meal type, and cooking time. Respond with a structured JSON object: { \"name\": \"Recipe Name\", \"description\": \"Description of the dish\", \"instructions\": \"List of instructions\", \"ingredients\": \"List of ingredients\" }";
            var userPrompt = $"Here are the details:\n" +
                             $"- Ingredients: {string.Join(", ", ingredients)}\n" +
                             $"- Meal Type: {mealType}\n" +
                             $"- Maximum Cooking Time: {maxCookingTime} minutes";


            string endpoint = "chat/completions";

            // Define Polly Retry Policy
            AsyncRetryPolicy<HttpResponseMessage> retryPolicy = Policy
                .Handle<HttpRequestException>() // Handle transient network issues
                .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode) // Retry on non-success status codes
                .WaitAndRetryAsync(
                    retryCount: 3, // Retry up to 3 times
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        // Log retry attempts
                        if (outcome.Exception != null)
                        {
                            _logger.LogWarning(outcome.Exception, "Retry {RetryAttempt} due to exception.", retryAttempt);
                        }
                        else
                        {
                            _logger.LogWarning("Retry {RetryAttempt} due to response status {StatusCode}.", retryAttempt, outcome.Result.StatusCode);
                        }
                    });

            try
            {
                // Execute the HTTP request with the retry policy
                HttpResponseMessage response = await retryPolicy.ExecuteAsync(async () =>
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                    var requestPayload = new
                    {
                        model = "gpt-4o-mini",
                        messages = new[]
                        {
                            new { role = "system", content = systemPrompt },
                            new { role = "user", content = userPrompt }
                        },
                        temperature = 0,
                        max_tokens = 300,
                        response_format = new { type = "json_object" }
                    };

                    var requestContent = new StringContent(
                        JsonSerializer.Serialize(requestPayload),
                        Encoding.UTF8,
                        "application/json");

                    return await _httpClient.PostAsync(endpoint, requestContent);
                });

                // Handle response
                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonDocument.Parse(responseString);

                    var rawContent = responseJson.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString()!;

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };

                    var recipeData = JsonSerializer.Deserialize<RecipeResponse>(rawContent, options);

                    if (recipeData is null || string.IsNullOrWhiteSpace(recipeData.Name))
                    {
                        _logger.LogError("Failed to deserialize recipe response.");
                        return null!;
                    }

                    var recipe = new Recipe
                    {
                        Name = recipeData.Name,
                        Description = recipeData.Description,
                        Ingredients = recipeData.Ingredients,
                        Instructions = recipeData.Instructions,
                        CreatedAt = DateTime.UtcNow,
                    };

                    _logger.LogInformation("Recipe generated successfully.");

                    return recipe;
                }
                else
                {
                    _logger.LogError("API call failed with status code {StatusCode}.", response.StatusCode);
                    return null!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred during the API call.");
                return null!;
            }
        }
        private class RecipeResponse
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public ICollection<string> Ingredients { get; set; } = new List<string>();
            public ICollection<string> Instructions { get; set; } = new List<string>();
        }
    }
}
