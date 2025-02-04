using AIronChef.Application.DTOs;
using AIronChef.Application.Users.Commands.UpdateUser;
using AIronChef.Application.Interfaces;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using FakeItEasy;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace AIronChef.Tests.UnitTests.ApplicationTests.UsersTest
{
    [TestFixture]
    public class UpdateUserHandlerTests
    {
        private IUserRepository _userRepository;
        private ILoggingService _loggingService;
        private UpdateUserHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _userRepository = A.Fake<IUserRepository>();
            _loggingService = A.Fake<ILoggingService>();
            _handler = new UpdateUserHandler(_userRepository, _loggingService);
        }

        [Test]
        public async Task ValidUserId_UpdatesUser()
        {
            // Arrange
            int userId = 1;
            var command = new UpdateUserCommand(userId, "newname", "new@email.com");
            var existingUser = new User { Id = userId, Name = "oldname", Email = "old@email.com" };
            var updatedUser = new User { Id = userId, Name = command.Name, Email = command.Email };
            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(existingUser);
            A.CallTo(() => _userRepository.UpdateAsync(A<User>.That.Matches(user => user.Id == userId && user.Email == command.Email && user.Name == command.Name))).Returns(updatedUser);
            A.CallTo(() => _userRepository.IsEmailUniqueAsync(command.Email)).Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.That(result.Data.Email, Is.EqualTo(command.Email));
            Assert.That(result.Data.Name, Is.EqualTo(command.Name));
        }

        [Test]
        public async Task InvalidUserId_Returns_Failure()
        {
            // Arrange
            int userId = 1;
            var command = new UpdateUserCommand(userId, "newname", "new@email.com");
            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(Task.FromResult<User>(null));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.That(result.ErrorMessage, Is.EqualTo("User not found"));
        }

        [Test]
        public async Task InvalidEmail_Returns_Failure()
        {
            // Arrange
            int userId = 1;
            var command = new UpdateUserCommand(userId, "newname", "newinvalidemail");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.That(result.ErrorMessage, Is.EqualTo("Invalid email."));
        }

        [Test]
        public async Task InvalidName_Returns_Failure()
        {
            // Arrange
            int userId = 1;
            var command = new UpdateUserCommand(userId, "", "new@email.com");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
        }

        [Test]
        public async Task NotUniqueEmail_Returns_Failure()
        {
            // Arrange
            int userId = 1;
            var command = new UpdateUserCommand(userId, "newname", "existing@email.com");
            var existingUser = new User { Id = userId, Name = "oldname", Email = "old@email.com" };
            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(existingUser);
            A.CallTo(() => _userRepository.IsEmailUniqueAsync(command.Email)).Returns(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.That(result.ErrorMessage, Is.EqualTo("New email is not unique"));
        }
    }
}
    