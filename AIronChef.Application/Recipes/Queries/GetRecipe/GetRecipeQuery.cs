using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AIronChef.Application.Recipes.Queries.GetRecipe
{
    public record GetRecipeQuery (int Id) : IRequest<OperationResult<Recipe>>;
    
}
