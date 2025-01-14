using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Polly;
using Polly.Retry;

namespace AIronChef.Infrastructure.Services
{
    public class OpenAiService
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
        public async Task<string> SendPromptAsync(string[] ingredients, string mealType, int maxCookingTime)
        {
            // Validate user input.
            if (ingredients == null || ingredients.Length == 0)
            {
                _logger.LogWarning("Invalid input: No ingredients provided.");
                return "Please provide a list of ingredients for the recipe.";
            }
            if (string.IsNullOrWhiteSpace(mealType))
            {
                _logger.LogWarning("Invalid input: Meal type is missing.");
                return "Please specify the type of meal (e.g., breakfast, lunch, or dinner).";
            }
            if (maxCookingTime <= 0)
            {
                _logger.LogWarning("Invalid input: Max cooking time is not valid.");
                return "Please provide a valid maximum cooking time in minutes.";
            }

            // Define the system and user prompt
            var systemPrompt = "You are a professional chef. Based on the given ingredients, meal type, and cooking time, create a recipe that fits the criteria. Respond with a detailed recipe and only a recipe, nothing more, nothing less.";
            var userPrompt = $"Here are the details:\n" +
                             $"- Ingredients: {string.Join(", ", ingredients)}\n" +
                             $"- Meal Type: {mealType}\n" +
                             $"- Maximum Cooking Time: {maxCookingTime} minutes";


            string url = "https://api.openai.com/v1/chat/completions";

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
                        }
                    };

                    var requestContent = new StringContent(
                        JsonSerializer.Serialize(requestPayload),
                        Encoding.UTF8,
                        "application/json");

                    return await _httpClient.PostAsync(url, requestContent);
                });

                // Handle response
                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonDocument.Parse(responseString);

                    string parsedRecipe = responseJson
                        .RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("message")
                        .GetProperty("content")
                        .GetString()!;

                    _logger.LogInformation("Recipe generated successfully.");
                    return parsedRecipe;
                }
                else
                {
                    _logger.LogError("API call failed with status code {StatusCode}.", response.StatusCode);
                    return $"Error: API call failed with status code {response.StatusCode}.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred during the API call.");
                return "Error: An unexpected error occurred while generating the recipe.";
            }
        }
    }
}
