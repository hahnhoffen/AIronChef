using AIronChef.Application.Interfaces;
using AIronChef.Application.Recipes.Commands.GenerateRecipe;
using AIronChef.Domain.Enums;
using AIronChef.Domain.Models;
using Castle.Core.Logging;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Tests.UnitTests.ApplicationTests.RecipeTests
{
    [TestFixture]
    public class GenerateRecipeHandlerTests
    {
        private GenerateRecipeCommandHandler _recipeHandler;
        private IRecipeGenerationService _recipeGenerationService;
        private ILogger<GenerateRecipeCommandHandler> _logger;

        [SetUp]
        public void SetUp()
        {
            _recipeGenerationService = A.Fake<IRecipeGenerationService>();
            _logger = A.Fake<ILogger<GenerateRecipeCommandHandler>>();
            _recipeHandler = new GenerateRecipeCommandHandler(_recipeGenerationService, _logger);
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsGeneratedRecipe()
        {
            var ingredients = new[] { "Tomato", "Cheese" };
            var mealType = MealType.Dinner;
            var cookingTime = 30;
            var generatedRecipe = new Recipe { Name = "Tomato Cheese Pasta" };

            A.CallTo(() => _recipeGenerationService.GenerateRecipeAsync(ingredients, mealType, cookingTime))
                .Returns(Task.FromResult(generatedRecipe));

            var command = new GenerateRecipeCommand(ingredients, cookingTime, mealType);

            var result = await _recipeHandler.Handle(command, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(generatedRecipe, result.Data);
        }

        [Test]
        public async Task Handle_InvalidRequest_EmptyIngredients_ReturnsFailure()
        {
            var command = new GenerateRecipeCommand(Array.Empty<string>(), 20, MealType.Breakfast);

            var result = await _recipeHandler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Recipe generation failed.", result.ErrorMessage);
        }

        [Test]
        public async Task Handle_ServiceReturnsNull_ReturnsFailure()
        {
            var ingredients = new[] { "Chicken", "Rice" };
            var mealType = MealType.Lunch;
            var cookingTime = 45;

            A.CallTo(() => _recipeGenerationService.GenerateRecipeAsync(ingredients, mealType, cookingTime))
                .Returns(Task.FromResult<Recipe>(null));

            var command = new GenerateRecipeCommand(ingredients, cookingTime, mealType);

            var result = await _recipeHandler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Recipe generation failed.", result.ErrorMessage);
        }

        [Test]
        public async Task Handle_ServiceThrowsException_ReturnsFailure()
        {
            var ingredients = new[] { "Egg", "Bacon" };
            var mealType = MealType.Breakfast;
            var cookingTime = 15;

            A.CallTo(() => _recipeGenerationService.GenerateRecipeAsync(ingredients, mealType, cookingTime))
                .Throws<Exception>();

            var command = new GenerateRecipeCommand(ingredients, cookingTime, mealType);

            var result = await _recipeHandler.Handle(command, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("An error occurred while generating the recipe.", result.ErrorMessage);
        }
    }
}
