using AIronChef.Application.Interfaces;
using AIronChef.Application.Users.Queries.GetUser;
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
        public async Task Handle_Existing_User_Return_Sucess()
        {
            var userId = 1;
            var user = new User { Id = userId, Name = "Test User", Email = "Test@gmail.com" };
            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Returns(user);

            var query = new GetUserQuery(userId);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(user, result.Data);
        }

        [Test]
        public async Task Handle_NonExistentUser_ReturnsFailure()
        {
            int invalidId = -1;
            A.CallTo(() => _userRepository.GetByIdAsync(invalidId)).Returns(null);

            var query = new GetUserQuery(invalidId);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("User not found.", result.ErrorMessage);
        }

        [Test]
        public async Task Handle_InvalidId_ReturnsFailure()
        {
            var query = new GetUserQuery(0);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Id must be greater than zero.", result.ErrorMessage);
        }

        [Test]
        public async Task Handle_RepositoryThrowsException_ReturnsFailure()
        {
            var userId = 2;
            A.CallTo(() => _userRepository.GetByIdAsync(userId)).Throws<Exception>();

            var query = new GetUserQuery(userId);

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.IsFalse(result.Success);
            Assert.AreEqual("Something went wrong.", result.ErrorMessage);
        }
    }
}
