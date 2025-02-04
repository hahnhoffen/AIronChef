using AIronChef.Application.DTOs;
using AIronChef.Application.Users.Commands.AddUser;
using AIronChef.Application.Users.Commands.UpdateUser;
using AIronChef.Application.Users.Commands.DeleteUser;
using AIronChef.Application.Users.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using AIronChef.Application.Users.Queries.Login;
using AIronChef.Domain.Enums;
using System.Security.Claims;
using AIronChef.Application.Users.Queries.GetAllUsers;

namespace AIronChef.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;

        public UserController(IMediator mediator, ILogger<UserController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody, Required] UserDto newUser)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid user data.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding new user {username}", newUser.Name);

            try
            {
                var result = await _mediator.Send(new AddUserCommand(newUser.Name, newUser.Email, newUser.Password));

                if (result.Success)
                {
                    var user = result.Data;
                    _logger.LogInformation("User {username} added successfully with id {id}", user.Name, user.Id);
                    return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, result);
                }
                else
                {
                    _logger.LogWarning("User creation failed: {message}", result.ErrorMessage);
                    return BadRequest(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new user.");
                return StatusCode(500, "An error occurred while adding a new user.");
            }
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID.");
                return BadRequest("Invalid user ID.");
            }

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return Unauthorized("User not logged in.");
            }

            int loggedInId = int.Parse(loggedInUserId);

            if (loggedInUserRole != nameof(UserRole.Admin) && loggedInId != id)
            {
                _logger.LogWarning("User {loggedInId} attempted to access another user's profile {id}.", loggedInId, id);
                return Forbid("You can only access your own profile.");
            }

            _logger.LogInformation("Fetching user with ID: {id}", id);

            try
            {
                var operationResult = await _mediator.Send(new GetUserQuery(id));
                if (!operationResult.Success)
                {
                    _logger.LogWarning("User not found.");
                    return NotFound("User not found.");
                }

                _logger.LogInformation("Successfully retrieved user {id}", id);
                return Ok(operationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the user.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpPut("{id:int}")] // Fixed incorrect `id:guid`
        public async Task<IActionResult> UpdateUser(int id, [FromBody, Required] UserDto updatedUser)
        {
            if (!ModelState.IsValid || id <= 0)
            {
                _logger.LogWarning("Invalid user data or ID.");
                return BadRequest(ModelState);
            }

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return Unauthorized("User not logged in.");
            }

            int loggedInId = int.Parse(loggedInUserId);

            if (loggedInUserRole != nameof(UserRole.Admin) && loggedInId != id)
            {
                _logger.LogWarning("User {loggedInId} attempted to update another user's profile {id}.", loggedInId, id);
                return Forbid("You can only update your own profile.");
            }

            _logger.LogInformation("Updating user {id}", id);

            try
            {
                var operationResult = await _mediator.Send(new UpdateUserCommand(id, updatedUser.Name, updatedUser.Email));

                if (!operationResult.Success)
                {
                    _logger.LogWarning("Update failed: {message}", operationResult.ErrorMessage);
                    return BadRequest(operationResult.ErrorMessage);
                }

                _logger.LogInformation("User {id} updated successfully", id);
                return Ok(operationResult.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user.");
                return StatusCode(500, "An error occurred while updating the user.");
            }
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID.");
                return BadRequest("Invalid user ID.");
            }

            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var loggedInUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (string.IsNullOrEmpty(loggedInUserId))
            {
                return Unauthorized("User not logged in.");
            }

            int loggedInId = int.Parse(loggedInUserId);

            if (loggedInUserRole != nameof(UserRole.Admin) && loggedInId != id)
            {
                _logger.LogWarning("User {loggedInId} attempted to delete another user's profile {id}.", loggedInId, id);
                return Forbid("You can only delete your own profile.");
            }

            _logger.LogInformation("Deleting user with id: {UserId}.", id);

            try
            {
                var operationResult = await _mediator.Send(new DeleteUserCommand(id));

                if (!operationResult.Success)
                {
                    _logger.LogWarning("Failed to delete user {id}: {message}", id, operationResult.ErrorMessage);
                    return BadRequest(operationResult.ErrorMessage);
                }

                _logger.LogInformation("User {id} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user.");
                return StatusCode(500, "An error occurred while deleting the user.");
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUser)
        {
            var result = await _mediator.Send(new LoginUserQuery(loginUser));
            
            if (!result.Success)
            {
                _logger.LogWarning("Something bad happened.");
                return Unauthorized();
            }

            var (user, token) = result.Data;
            return Ok(new
            {
                Message = "Login successful",
                User = new
                {
                    user.Id,
                    user.Name,
                    user.Email
                },
                Token = token
            });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            _logger.LogInformation("Fetching all users.");

            try
            {
                var operationResult = await _mediator.Send(new GetAllUsersQuery());

                if (!operationResult.Success)
                {
                    _logger.LogWarning("No users found.");
                    return NotFound("No users found.");
                }

                _logger.LogInformation("Successfully retrieved all users.");
                return Ok(operationResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
