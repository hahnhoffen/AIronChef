using AIronChef.API.Controllers;
using AIronChef.Domain.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using AIronChef.API; 


namespace AIronChef.Tests.IntegrationTests
{
    [TestFixture]
    public class UsersControllerTests : IDisposable
    {
        private HttpClient _client;
        private WebApplicationFactory<Program> _factory;
        private readonly ILogger<UsersControllerTests> _logger;

        [SetUp]
        public void SetUp()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task CreateUser_ShouldReturnCreated()
        {
            var newUser = new User
            {
                Id = 1234,
                Name = "TestUser",
                Email = "testuser@example.com",
                PasswordHash = "TestPassword123"
            };

            var response = await _client.PostAsJsonAsync("/api/users", newUser);

            Assert.That(response.IsSuccessStatusCode, Is.True, "Response was not successful.");
            var createdUser = await response.Content.ReadFromJsonAsync<User>();
            Assert.That(createdUser, Is.Not.Null, "Created user is null.");
            Assert.That(createdUser.Name, Is.EqualTo(newUser.Name));
            Assert.That(createdUser.Email, Is.EqualTo(newUser.Email));
            _logger.LogInformation("User created: {0}", createdUser.Id);
        }

        [Test]
        public async Task GetUser_ShouldReturnUser()
        {
            var userId = 1234;
            var newUser = new User
            {
                Id = 1234,
                Name = "TestUser",
                Email = "testuser@example.com",
                PasswordHash = "TestPassword123"
            };
            await _client.PostAsJsonAsync("/api/users", newUser);

            var response = await _client.GetAsync($"/api/users/{userId}");

            Assert.That(response.IsSuccessStatusCode, Is.True, "Response was not successful.");
            var user = await response.Content.ReadFromJsonAsync<User>();
            Assert.That(user, Is.Not.Null, "User not found.");
            Assert.That(user.Id, Is.EqualTo(userId));
            _logger.LogInformation("User retrieved: {0}", user.Id);
        }

        [Test]
        public async Task UpdateUser_ShouldReturnUpdatedUser()
        {
            var userId = Guid.NewGuid();
            var newUser = new User
            {
                Id = 1234,
                Name = "TestUser",
                Email = "testuser@example.com",
                PasswordHash = "TestPassword123"
            };
            await _client.PostAsJsonAsync("/api/users", newUser);

            var updatedUser = new User
            {
                Id = 1234,
                Name = "UpdatedUser",
                Email = "updateduser@example.com",
                PasswordHash = "UpdatedPassword123"
            };

            var response = await _client.PutAsJsonAsync($"/api/users/{userId}", updatedUser);

            Assert.That(response.IsSuccessStatusCode, Is.True, "Response was not successful.");
            var user = await response.Content.ReadFromJsonAsync<User>();
            Assert.That(user, Is.Not.Null, "Updated user is null.");
            Assert.That(user.Name, Is.EqualTo(updatedUser.Name));
            Assert.That(user.Email, Is.EqualTo(updatedUser.Email));
            _logger.LogInformation("User updated: {0}", user.Id);
        }

        [Test]
        public async Task DeleteUser_ShouldReturnSuccess()
        {
            var userId = 1234; 
            var newUser = new User
            {
                Id = 1234,
                Name = "TestUser",
                Email = "testuser@example.com",
                PasswordHash = "TestPassword123"
            };
            await _client.PostAsJsonAsync("/api/users", newUser);

            var response = await _client.DeleteAsync($"/api/users/{userId}");

            Assert.That(response.IsSuccessStatusCode, Is.True, "Response was not successful.");
            _logger.LogInformation("User deleted: {0}", userId);
        }

        public void Dispose()
        {
            TearDown();
        }
    }
}
