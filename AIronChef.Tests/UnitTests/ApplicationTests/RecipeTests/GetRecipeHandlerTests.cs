using AIronChef.Application.Interfaces;
using AIronChef.Application.Recipes.Queries.GetRecipe;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Tests.UnitTests.ApplicationTests.RecipeTests
{
    [TestFixture]
    public class GetRecipeHandlerTests
    {
        private IGenericRepository<Recipe> _recipeRepository; 
        private ILoggingService _loggingService;
        private GetRecipeHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _recipeRepository = A.Fake<IGenericRepository<Recipe>>();
            _loggingService = A.Fake<ILoggingService>();
            _handler = new GetRecipeHandler(_recipeRepository, _loggingService);
        }

        [Test]
        public async Task Handle_ExistingRecipe_ReturnsSuccess()
        {
            // Arrange
            var recipeId = 1;
            var recipe = new Recipe { Id = recipeId, Name = "Test Recipe", Ingredients = new[] { "Tomato", "Cheese" } };
            A.CallTo(() => _recipeRepository.GetByIdAsync(recipeId)).Returns(Task.FromResult(recipe));

            var query = new GetRecipeQuery(recipeId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(recipe, result.Data);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public async Task Handle_NonExistentRecipe_ReturnsFailure()
        {
            // Arrange
            var nonExistentRecipeId = 9999;
            A.CallTo(() => _recipeRepository.GetByIdAsync(nonExistentRecipeId)).Returns(Task.FromResult<Recipe>(null));

            var query = new GetRecipeQuery(nonExistentRecipeId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Recipe not found.", result.ErrorMessage);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task Handle_InvalidId_ReturnsFailure()
        {
            // Arrange
            var query = new GetRecipeQuery(0);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Id must be greater than zero.", result.ErrorMessage);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task Handle_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var recipeId = 2;
            A.CallTo(() => _recipeRepository.GetByIdAsync(recipeId)).Throws(new Exception("Database error"));

            var query = new GetRecipeQuery(recipeId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("An error occurred while retrieving the recipe: Database error", result.ErrorMessage);
            Assert.IsNull(result.Data);
        }
    }
}
