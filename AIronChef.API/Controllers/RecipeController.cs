using AIronChef.Application.DTOs;
using AIronChef.Application.Recipes.Commands.DeleteRecipe;
using AIronChef.Application.Recipes.Commands.GenerateRecipe;
using AIronChef.Application.Recipes.Commands.UpdateRecipe;
using AIronChef.Application.Recipes.Queries.GetRecipe;
using AIronChef.Domain.Enums;
using AIronChef.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AIronChef.Application.Recipes.Queries.GetAllRecipes;

namespace AIronChef.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RecipeController> _logger;

        public RecipeController(IMediator mediator, ILogger<RecipeController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [Authorize(Roles = nameof(UserRole.User))]
        [HttpPost]
        public async Task<IActionResult> AddRecipe([FromBody, Required] RecipeDto newRecipe)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid recipe data.");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding new recipe: {recipeType}", newRecipe.MealType.ToString());

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();
                
                var result = await _mediator.Send(new GenerateRecipeCommand(newRecipe.Ingredients, newRecipe.MaxCookingTimeMinutes, newRecipe.MealType, int.Parse(userId)));

                if (result.Data is Recipe generatedRecipe)
                {
                    return CreatedAtAction(nameof(GetRecipeById), new { id = generatedRecipe.Id }, generatedRecipe);
                }
                else
                {
                    _logger.LogError("Unexpected result type.");
                    return StatusCode(500, "An error occurred while adding the recipe.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the recipe.");
                return StatusCode(500, "An error occurred while adding the recipe.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipeById(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid recipe ID.");
                return BadRequest("Invalid recipe ID.");
            }

            _logger.LogInformation("Fetching recipe with ID: {id}", id);

            try
            {
                var result = await _mediator.Send(new GetRecipeQuery(id));
                if (result == null)
                {
                    _logger.LogWarning("Recipe with ID {id} not found.", id);
                    return NotFound($"Recipe with ID {id} not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the recipe.");
                return StatusCode(500, "An error occurred while fetching the recipe.");
            }
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, [FromBody, Required] Recipe updatedRecipe)
        {
            if (!ModelState.IsValid || id <= 0)
            {
                _logger.LogWarning("Invalid recipe data or ID.");
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
                _logger.LogWarning("User {loggedInId} attempted to update another user's recipe {id}.", loggedInId, id);
                return Forbid("You can only update your own recipes.");
            }

            _logger.LogInformation("Updating recipe with ID: {id}", id);

            try
            {
                updatedRecipe.Id = id;
                var result = await _mediator.Send(new UpdateRecipeCommand(updatedRecipe));
                if (result == null)
                {
                    _logger.LogWarning("Recipe with ID {id} not found.", id);
                    return NotFound($"Recipe with ID {id} not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the recipe.");
                return StatusCode(500, "An error occurred while updating the recipe.");
            }
        }

        [Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.User))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid recipe ID.");
                return BadRequest("Invalid recipe ID.");
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
                _logger.LogWarning("User {loggedInId} attempted to delete another user's recipe {id}.", loggedInId, id);
                return Forbid("You can only delete your own recipes.");
            }

            _logger.LogInformation("Deleting recipe with ID: {id}", id);

            try
            {
                var result = await _mediator.Send(new DeleteRecipeCommand(id));
                if (!result.Success)
                {
                    _logger.LogWarning("Recipe with ID {id} not found.", id);
                    return NotFound($"Recipe with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the recipe.");
                return StatusCode(500, "An error occurred while deleting the recipe.");
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllRecipes()
        {
            _logger.LogInformation("Fetching all recipes.");
            try
            {
                var result = await _mediator.Send(new GetAllRecipesQuery());
                if (!result.Success || result.Data is null)
                {
                    _logger.LogWarning("No recipes found.");
                    return NotFound("No recipes found.");
                }
                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all recipes.");
                return StatusCode(500, "An error occurred while retrieving recipes.");
            }
        }
    }
}