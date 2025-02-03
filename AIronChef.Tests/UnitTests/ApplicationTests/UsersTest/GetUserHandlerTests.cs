using AIronChef.Application.Interfaces;
using AIronChef.Application.Users.Queries.GetUser;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using FakeItEasy;

namespace AIronChef.Tests.UnitTests.ApplicationTests.UsersTest
{
    [TestFixture]
    public class GetUserHandlerTests
    {
        private IUserRepository _userRepository;
        private ILoggingService _loggingService;
        private GetUserHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _userRepository = A.Fake<IUserRepository>();
            _loggingService = A.Fake<ILoggingService>();
            _handler = new GetUserHandler(_userRepository, _loggingService);
        }

        [Test]
        public async Task Handle_Existing_User_Returns_Success()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Name = "Test User", Email = "Test@gmail.com" };

            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(Task.FromResult(user));

            var query = new GetUserQuery(userId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(user, result.Data);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public async Task Handle_NonExistentUser_ReturnsFailure()
        {
            // Arrange
            int nonExistentUserId = 9999; // Change from -1 to valid ID to match behavior

            A.CallTo(() => _userRepository.GetByIdAsync(nonExistentUserId)).Returns(Task.FromResult<User>(null));

            var query = new GetUserQuery(nonExistentUserId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("User not found.", result.ErrorMessage);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task Handle_InvalidId_ReturnsFailure()
        {
            // Arrange
            var query = new GetUserQuery(0);

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
            var userId = 2;

            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Throws(new Exception("Database error"));

            var query = new GetUserQuery(userId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Something went wrong.", result.ErrorMessage);
            Assert.IsNull(result.Data);
        }
    }
}
