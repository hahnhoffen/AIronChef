using AIronChef.Application.Common.Helpers;
using AIronChef.Application.Users.Commands.AddUser;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using FakeItEasy;

namespace AIronChef.Tests.UnitTests.ApplicationTests.UsersTest
{
    [TestFixture]
    public class AddUserCommandHandlerTests
    {
        public IUserRepository CreateFakeRepository()
        {
            // Create fake repository
            var fakeRepository = A.Fake<IUserRepository>();
            A.CallTo(() => fakeRepository.AddAsync(A<User>._))
                .ReturnsLazily(x =>
                {
                    User userIn = x.Arguments.Get<User>(0);
                    // Copy argument user to a new user
                    User createdUser = new User { Name = userIn.Name, Email = userIn.Email };
                    createdUser.PasswordHash = userIn.PasswordHash;
                    createdUser.PasswordSalt = userIn.PasswordSalt;
                    // Set an id
                    createdUser.Id = 1;
                    return Task.FromResult(createdUser);
                });
            return fakeRepository;
        }

        [TestCase("a@b.com")]
        [TestCase("123@b.com")]
        [TestCase("\"quotedmail\"@b.com")]
        [TestCase("a.b@c.com")]
        [TestCase("a@countrydomain.se")]
        [TestCase("a@sub.domain.tld")]
        [TestCase("any@normal.mail")]
        [TestCase("ipmail@[1.2.3.4]")]
        [TestCase("1234567890123456789012345678901234567890123456789012345678901234@b.co")] // Local part 64 letters
        public async Task Valid_AddsUser(string validEmail)
        {
            // Arrange
            var command = new AddUserCommand { Name = "Name", Email = validEmail, Password = "Password" };

            var fakeRepository = CreateFakeRepository();

            A.CallTo(() => fakeRepository.IsEmailUniqueAsync(command.Email)).Returns(Task.FromResult(true));

            var handler = new AddUserCommandHandler(fakeRepository);

            // Act
            var operationResult = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(operationResult.Success);
            Assert.That(operationResult.Data.Name, Is.EqualTo(command.Name));
            Assert.That(operationResult.Data.Email, Is.EqualTo(command.Email));
            Assert.True(PasswordHasher.VerifyPassword(command.Password, operationResult.Data));
        }

        [Test]
        public async Task NotUniqueEmail_ShouldFail()
        {
            // Arrange
            var command = new AddUserCommand { Name = "Name", Email = "a@b.com", Password = "Password" };

            var fakeRepository = CreateFakeRepository();
            A.CallTo(() => fakeRepository.IsEmailUniqueAsync(command.Email)).Returns(Task.FromResult(false));

            var handler = new AddUserCommandHandler(fakeRepository);

            // Act
            var operationResult = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(operationResult.Success);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("a")]
        [TestCase("a.b")]
        [TestCase("@a.com")] // Need a local part
        [TestCase("a@.com")] // Can't have just top level domain
        [TestCase("@b.com")] // Need to have a local part
        [TestCase("a@b.c")] // Domain top level must be at least 2 letters
        [TestCase("a@@b.co")] // Just one @
        [TestCase("a@aa@b.co")] // Just one @
        [TestCase(".startdot@b.co")] // local part should not start with dot
        [TestCase("enddot.@b.co")] // local part should not end with dot
        [TestCase("twoconsecutive..dots@b.co")] // local part should not have two consecutive dots
        [TestCase("a\"a@b.co")] // No " in the middle of local part
        [TestCase("a@\"quoteddomain\".co")] // no quotes in domain
        [TestCase("a@quoted.tld.\".co\"")] // no quotes in top level domain
        [TestCase("12345678901234567890123456789012345678901234567890123456789012345@b.co")] // Local part max 64 letters
        public async Task InvalidEmail_ShouldFail(string invalidEmail)
        {
            // Arrange
            var command = new AddUserCommand { Name = "Name", Email = invalidEmail, Password = "Password" };

            var fakeRepository = CreateFakeRepository();
            A.CallTo(() => fakeRepository.IsEmailUniqueAsync(command.Email)).Returns(Task.FromResult(true));

            var handler = new AddUserCommandHandler(fakeRepository);

            // Act
            var operationResult = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(operationResult.Success);
        }

        public async Task InvalidName_ShouldFail()
        {
            // Arrange
            var command = new AddUserCommand { Name = "", Email = "a@b.com", Password = "Password" };

            var fakeRepository = CreateFakeRepository();
            A.CallTo(() => fakeRepository.IsEmailUniqueAsync(command.Email)).Returns(Task.FromResult(true));

            var handler = new AddUserCommandHandler(fakeRepository);

            // Act
            var operationResult = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(operationResult.Success);
        }
    }
}


