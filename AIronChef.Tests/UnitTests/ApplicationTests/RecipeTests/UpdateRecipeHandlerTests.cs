using AIronChef.Application.Interfaces;
using AIronChef.Application.Recipes.Commands.UpdateRecipe;
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
    public class UpdateRecipeHandlerTests
    {
        private IGenericRepository<Recipe> _recipeRepository;
        private UpdateRecipeHandler _handler;
        private ILoggingService _loggingService;

        [SetUp]
        public void SetUp()
        {
            _recipeRepository = A.Fake<IGenericRepository<Recipe>>();
            _loggingService = A.Fake<ILoggingService>();
            _handler = new UpdateRecipeHandler(_recipeRepository, _loggingService);

        }

        [Test]
        public async Task Handle_ValidRecipe_UpdatesSuccessfully()
        {
            // Arrange
            var recipeId = 1;
            var existingRecipe = new Recipe { Id = recipeId, Name = "Old Recipe", Description = "Old Description", Ingredients = new List<string> { "Salt" } };
            var updatedRecipe = new Recipe { Id = recipeId, Name = "New Recipe", Description = "New Description", Ingredients = new List<string> { "Pepper" } };
            var command = new UpdateRecipeCommand(updatedRecipe);

            A.CallTo(() => _recipeRepository.GetByIdAsync(recipeId)).Returns(existingRecipe);
            A.CallTo(() => _recipeRepository.UpdateAsync(A<Recipe>.That.Matches(r => r.Id == recipeId))).Returns(updatedRecipe);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(updatedRecipe.Name, result.Data.Name);
            Assert.AreEqual(updatedRecipe.Description, result.Data.Description);
        }

        [Test]
        public async Task Handle_NonExistentRecipe_ReturnsFailure()
        {
            // Arrange
            var recipeId = 9999;
            var updatedRecipe = new Recipe { Id = recipeId, Name = "New Recipe", Description = "New Description", Ingredients = new List<string> { "Pepper" } };
            var command = new UpdateRecipeCommand(updatedRecipe);

            A.CallTo(() => _recipeRepository.GetByIdAsync(recipeId)).Returns(Task.FromResult<Recipe>(null));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual($"Recipe with ID {recipeId} does not exist.", result.ErrorMessage);
        }

        [Test]
        public async Task Handle_InvalidRecipeId_ReturnsFailure()
        {
            // Arrange
            var updatedRecipe = new Recipe { Id = 0, Name = "New Recipe", Description = "New Description", Ingredients = new List<string> { "Pepper" } };
            var command = new UpdateRecipeCommand(updatedRecipe);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Invalid command: Recipe ID is not valid.", result.ErrorMessage);
        }

        [Test]
        public async Task Handle_RepositoryThrowsException_ReturnsFailure()
        {
            // Arrange
            var recipeId = 2;
            var existingRecipe = new Recipe { Id = recipeId, Name = "Old Recipe", Description = "Old Description", Ingredients = new List<string> { "Salt" } };
            var updatedRecipe = new Recipe { Id = recipeId, Name = "New Recipe", Description = "New Description", Ingredients = new List<string> { "Pepper" } };
            var command = new UpdateRecipeCommand(updatedRecipe);

            A.CallTo(() => _recipeRepository.GetByIdAsync(recipeId)).Returns(existingRecipe);
            A.CallTo(() => _recipeRepository.UpdateAsync(A<Recipe>.That.Matches(r => r.Id == recipeId))).Throws(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.ErrorMessage.Contains("An error occurred while updating the recipe"));
        }
    }
}
