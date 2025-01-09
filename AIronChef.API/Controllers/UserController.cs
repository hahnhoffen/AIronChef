using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AIronChef.API.Controllers
{
    public class UserController : Controller
    {

        private readonly IMediator _mediator;
        private readonly ILogger<UserController> _logger;


        [HttpPost]
        [Route("api/users")]
        public async Task<IActionResult> Register([FromBody, Required] User newUser)
        {
            _logger.LogInformation("Adding new User {username}", newUser.UserName);
            try
            {
                var operationResult = await _mediator.Send(new AddUserCommand(newUser));
                _logger.LogInformation("User {username} added successfully", operationResult.Data.UserName);
                return Ok(operationResult.Data);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding new User");
                return StatusCode(500, "An error occurred while adding new User.");
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
                var operationResult = await _mediator.Send(new GetUserByIdQuery(id));
                _logger.LogInformation("Successfully retrieved the user!");
                return Ok(operationResult.Data);
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
            _logger.LogInformation("Updating User {username}", user.UserName);
            try
            {
                var operationResult = await _mediator.Send(new UpdateUserCommand(user));
                _logger.LogInformation("User {username} updated successfully", operationResult.Data.UserName);
                return Ok(operationResult.Data);
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
