using AIronChef.Application.Users.Commands.AddUser;
using AIronChef.Application.Users.Commands.UpdateUser;
using AIronChef.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AIronChef.API.Controllers
{
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
        [Route("api/users")]
        public async Task<IActionResult> Register([FromBody, Required] User newUser)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid user data.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding new user {username}", newUser.Name);
            try
            {
                var result = await _mediator.Send(new AddUserCommand
                {
                    Name = newUser.Name,
                    Email = newUser.Email,
                    Password = newUser.PasswordHash
                });

                if (result.Success)
                {
                    var user = result.Data;
                    _logger.LogInformation("User {username} added successfully with id {id}", user.Name, user.Id);
                    return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, result);
                }
                else
                {
                    _logger.LogWarning("New user OperationResult failure: {message}", result.ErrorMessage);
                    return BadRequest(result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new user.");
                return StatusCode(500, "An error occurred while adding a new user.");
            }
        }


        [HttpGet("api/users/{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid input data");
                return BadRequest("Invalid input data.");
            }

            try
            {
                var operationResult = await _mediator.Send(new GetUserQuery(id));
                if (operationResult == null)
                {
                    _logger.LogWarning("User not found");
                    return NotFound("User not found.");
                }

                _logger.LogInformation("Successfully retrieved the user!");
                return Ok(operationResult);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize]
        [HttpPut("api/users/{id:guid}")]
        public async Task<IActionResult> UpdateUser([FromBody, Required] User user)
        {
            _logger.LogInformation("Updating User {username}", user.Name);
            try
            {
                var operationResult = await _mediator.Send(new UpdateUserCommand
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                });
                if (operationResult == null)
                {
                    _logger.LogWarning("User not found");
                    return NotFound("User not found.");
                }
                if (operationResult.Success)
                {
                    _logger.LogInformation("User {username} updated successfully", operationResult.Data.Name);
                    return Ok(operationResult.Data);
                }
                else
                {
                    _logger.LogWarning("Update user failure: {message}", operationResult.ErrorMessage);
                    return BadRequest(operationResult.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating User");
                return StatusCode(500, "An error occurred while updating User.");
            }
        }

        [Authorize]
        [HttpDelete("api/users/{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid input data");
                return BadRequest("Invalid input data.");
            }
            try
            {
                var operationResult = await _mediator.Send(new DeleteUserCommand(id));
                _logger.LogInformation("User {id} deleted successfully", id);
                return Ok(operationResult.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting User");
                return StatusCode(500, "An error occurred while deleting User.");
            }
        }


        /*[HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] User userWantingToLogIn)
        {
            var userDto = new UserDto
            {
                UserName = userWantingToLogIn.UserName,
                Password = userWantingToLogIn.Password
                // Map other properties as needed
            };
            return Ok(await _mediator.Send(new LoginUserQuery(userDto)));
        }*/
    }
}
