using AIronChef.Application.Interfaces;
using AIronChef.Application.Users.Commands.DeleteUser;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Tests.UnitTests.ApplicationTests.UsersTest
{
    [TestFixture]
    public class DeleteUserHandlerTests
    {
        private IUserRepository _userRepository;
        private ILoggingService _loggingService;
        private DeleteUserCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _userRepository = A.Fake<IUserRepository>();
            _loggingService = A.Fake<ILoggingService>();
            _handler = new DeleteUserCommandHandler(_userRepository, _loggingService);
        }


        [Test]
        public async Task Handle_Should_ReturnSuccess_When_UserIsDeleted()
        {
            int userId = 1;
            A.CallTo(() => _userRepository.DeleteAsync(userId))
                .Returns(Task.FromResult(true));

            var command = new DeleteUserCommand { Id = userId };
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.True);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_UserDoesNotExist()
        {
            int userId = 1999;
            A.CallTo(() => _userRepository.DeleteAsync(userId))
                .Returns(false);

            var command = new DeleteUserCommand { Id = userId };
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo($"User with ID {userId} does not exist."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_IdIsInvalid()
        {
            int invalidId = -1;

            var command = new DeleteUserCommand {Id = invalidId};
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid command: User ID is not valid."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_DatabaseErrorOccurs()
        {
            int userId = 1;
            A.CallTo(() => _userRepository.DeleteAsync(userId))
                .Throws(new Exception("Database error"));

            var command = new DeleteUserCommand { Id = userId };
            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("An error occurred while trying to delete the user."));
        }
    }
}
