using AIronChef.Application.Interfaces;
using AIronChef.Application.Recipes.Commands.GenerateRecipe;
using AIronChef.Domain.Models;
using AIronChef.Domain.Enums;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace AIronChef.Tests.UnitTests.ApplicationTests.RecipeTests
{
    [TestFixture]
    public class GenerateRecipeCommandHandlerTests
    {
        private IRecipeGenerationService _recipeGenerationService;
        private ILogger<GenerateRecipeCommandHandler> _logger;
        private GenerateRecipeCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _recipeGenerationService = A.Fake<IRecipeGenerationService>();
            _logger = A.Fake<ILogger<GenerateRecipeCommandHandler>>();
            _handler = new GenerateRecipeCommandHandler(_recipeGenerationService, _logger);
        }

        [Test]
        public async Task Handle_Should_ReturnSuccess_When_RecipeIsGenerated()
        {
            // Arrange
            var ingredients = new List<string> { "Chicken", "Garlic", "Salt" };
            var mealType = MealType.Dinner;
            var maxCookingTime = 30;

            var generatedRecipe = new Recipe
            {
                Name = "Garlic Chicken",
                Description = "A delicious garlic-infused chicken dish.",
                Ingredients = ingredients,
                Instructions = new List<string> { "Step 1: Prep", "Step 2: Cook" },
                CreatedAt = DateTime.UtcNow
            };

            A.CallTo(() => _recipeGenerationService.GenerateRecipeAsync(ingredients, mealType, maxCookingTime))
                .Returns(Task.FromResult(generatedRecipe));

            var command = new GenerateRecipeCommand(ingredients, maxCookingTime, mealType);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Name, Is.EqualTo("Garlic Chicken"));

            // Verify Logging
            A.CallTo(() => _logger.LogInformation(
                A<string>.That.Contains("Recipe generated successfully"),
                A<object[]>._)).MustHaveHappened();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_RecipeGenerationFails()
        {
            // Arrange
            var ingredients = new List<string> { "Tomato", "Onion" };
            var mealType = MealType.Lunch;
            int? maxCookingTime = 20;

            A.CallTo(() => _recipeGenerationService.GenerateRecipeAsync(ingredients, mealType, maxCookingTime))
                .Returns(Task.FromResult<Recipe>(null!)); // Simulating AI failure

            var command = new GenerateRecipeCommand(ingredients, maxCookingTime, mealType);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Recipe generation failed."));

            // Verify Logging
            A.CallTo(() => _logger.LogWarning(
                A<string>.That.Contains("AI response was empty or invalid.")))
                .MustHaveHappened();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_ExceptionOccurs()
        {
            // Arrange
            var ingredients = new List<string> { "Egg", "Butter" };
            var mealType = MealType.Breakfast;
            int? maxCookingTime = 10;

            A.CallTo(() => _recipeGenerationService.GenerateRecipeAsync(ingredients, mealType, maxCookingTime))
                .Throws(new Exception("API error")); // Simulating an exception

            var command = new GenerateRecipeCommand(ingredients, maxCookingTime, mealType);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("An error occurred while generating the recipe."));

            // Verify Logging
            A.CallTo(() => _logger.LogError(
                A<Exception>.That.Matches(e => e.Message == "API error"),
                A<string>.That.Contains("Error while generating recipe with ingredients"),
                A<object[]>._)).MustHaveHappened();
        }
    }
}
