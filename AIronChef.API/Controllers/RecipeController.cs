using AIronChef.Application.DTOs;
using AIronChef.Application.Recipes.Commands.DeleteRecipe;
using AIronChef.Application.Recipes.Commands.GenerateRecipe;
using AIronChef.Application.Recipes.Commands.UpdateRecipe;
using AIronChef.Application.Recipes.Queries.GetRecipe;
using AIronChef.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
                //var result = await _mediator.Send(new GenerateRecipeCommandHandler
                //{
                //    Ingredients = newRecipe.Ingredients
                //});
                var result = await _mediator.Send(new GenerateRecipeCommand(newRecipe.Ingredients, newRecipe.MaxCookingTimeMinutes, newRecipe.MealType));

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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(int id, [FromBody, Required] Recipe updatedRecipe)
        {
            if (!ModelState.IsValid || id <= 0)
            {
                _logger.LogWarning("Invalid recipe data or ID.");
                return BadRequest(ModelState);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid recipe ID.");
                return BadRequest("Invalid recipe ID.");
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
    }
}

