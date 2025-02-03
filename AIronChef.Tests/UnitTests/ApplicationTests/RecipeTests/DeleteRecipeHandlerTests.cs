using AIronChef.Application.Interfaces;
using AIronChef.Application.Recipes.Commands.DeleteRecipe;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using FakeItEasy;

namespace AIronChef.Tests.UnitTests.ApplicationTests.RecipeTests
{
    [TestFixture]
    public class DeleteRecipeHandlerTests
    {
        private IRecipeRepository _recipeRepository;
        private ILoggingService _loggingService;
        private DeleteRecipeHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _recipeRepository = A.Fake<IRecipeRepository>();
            _loggingService = A.Fake<ILoggingService>();
            _handler = new DeleteRecipeHandler(_recipeRepository, _loggingService);
        }

        [Test]
        public async Task Handle_Should_ReturnSuccess_When_RecipeIsDeleted()
        {
            // Arrange
            int recipeId = 1;
            var recipe = new Recipe { Id = recipeId, Name = "Test Recipe" };

            A.CallTo(() => _recipeRepository.GetByIdAsync(recipeId))
                .Returns(Task.FromResult(recipe));

            A.CallTo(() => _recipeRepository.DeleteAsync(recipeId))
                .Returns(Task.FromResult(true));

            var command = new DeleteRecipeCommand(recipeId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data.Id, Is.EqualTo(recipeId));
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_RecipeDoesNotExist()
        {
            // Arrange
            int recipeId = 1999;
            
            A.CallTo(() => _recipeRepository.GetByIdAsync(recipeId))
                .Returns(Task.FromResult<Recipe>(null));

            var command = new DeleteRecipeCommand(recipeId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo($"Recipe with ID {recipeId} does not exist."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_IdIsInvalid()
        {
            // Arrange
            int invalidId = -1;
            var command = new DeleteRecipeCommand(invalidId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid command: Recipe ID is not valid."));

            // Ensure GetByIdAsync and DeleteAsync are never called
            A.CallTo(() => _recipeRepository.GetByIdAsync(invalidId)).MustNotHaveHappened();
            A.CallTo(() => _recipeRepository.DeleteAsync(invalidId)).MustNotHaveHappened();
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_DeletionFails()
        {
            // Arrange
            int recipeId = 2;
            var recipe = new Recipe { Id = recipeId, Name = "Failed Deletion Recipe" };

            A.CallTo(() => _recipeRepository.GetByIdAsync(recipeId))
                .Returns(Task.FromResult(recipe));

            A.CallTo(() => _recipeRepository.DeleteAsync(recipeId))
                .Returns(Task.FromResult(false));

            var command = new DeleteRecipeCommand(recipeId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Failed to delete the recipe."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_ExceptionOccurs()
        {
            // Arrange
            int recipeId = 3;
            var recipe = new Recipe { Id = recipeId, Name = "Exception Recipe" };

            A.CallTo(() => _recipeRepository.GetByIdAsync(recipeId))
                .Returns(Task.FromResult(recipe));

            A.CallTo(() => _recipeRepository.DeleteAsync(recipeId))
                .Throws(new Exception("Database failure"));

            var command = new DeleteRecipeCommand(recipeId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Does.StartWith("An error occurred while deleting the recipe:"));
        }
    }
}
