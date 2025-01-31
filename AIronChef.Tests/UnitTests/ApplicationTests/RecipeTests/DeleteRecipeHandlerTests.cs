using AIronChef.Application.Recipes.Commands.DeleteRecipe;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using AIronChef.Infrastructure.Logging;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            int recipeId = 1;
            A.CallTo(() => _recipeRepository.DeleteRecipeAsync(recipeId)).Returns(Task.FromResult(new Recipe()));

            var command = new DeleteRecipeCommand { Id = recipeId };
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }


        [Test]
        public async Task Handle_Should_ReturnFailure_When_RecipeDoesNotExist()
        {
            int recipeId = 1999; 
            A.CallTo(() => _recipeRepository.DeleteRecipeAsync(recipeId))
                .Returns(Task.FromResult((Recipe?)null)); 

            var command = new DeleteRecipeCommand { Id = recipeId };
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Recipe not found."));
        }


        [Test]
        public async Task Handle_Should_ReturnFailure_When_IdIsInvalid()
        {
            int invalidId = -1; 
            var command = new DeleteRecipeCommand {Id = invalidId};
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid recipe ID."));
        }
    }
}
