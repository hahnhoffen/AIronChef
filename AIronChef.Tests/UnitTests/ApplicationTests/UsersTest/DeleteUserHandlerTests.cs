using AIronChef.Application.Interfaces;
using AIronChef.Application.Users.Commands.DeleteUser;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using FakeItEasy;

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
            // Arrange
            int userId = 1;
            var user = new User { Id = userId, Name = "Test User", Email = "test@test.com" };

            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(Task.FromResult(user));
            A.CallTo(() => _userRepository.DeleteAsync(userId)).Returns(Task.FromResult(true));

            var command = new DeleteUserCommand(userId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.Data, Is.Null);
            Assert.That(result.ErrorMessage, Is.Null);
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_UserDoesNotExist()
        {
            // Arrange
            int userId = 1999;
            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(Task.FromResult<User>(null));

            var command = new DeleteUserCommand(userId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo($"User with ID {userId} does not exist."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_IdIsInvalid()
        {
            // Arrange
            int invalidId = -1;
            var command = new DeleteUserCommand(invalidId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid command: User ID is not valid."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_DeletionFails()
        {
            // Arrange
            int userId = 2;
            var user = new User { Id = userId, Name = "Failed Deletion User", Email = "failed@test.com" };

            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(Task.FromResult(user));
            A.CallTo(() => _userRepository.DeleteAsync(userId)).Returns(Task.FromResult(false));

            var command = new DeleteUserCommand(userId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("Failed to delete the user."));
        }

        [Test]
        public async Task Handle_Should_ReturnFailure_When_ExceptionOccurs()
        {
            // Arrange
            int userId = 3;
            var user = new User { Id = userId, Name = "Exception User", Email = "exception@test.com" };

            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(Task.FromResult(user));
            A.CallTo(() => _userRepository.DeleteAsync(userId)).Throws(new Exception("Database failure"));

            var command = new DeleteUserCommand(userId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Is.EqualTo("An error occurred while trying to delete the user."));
        }
    }
}
