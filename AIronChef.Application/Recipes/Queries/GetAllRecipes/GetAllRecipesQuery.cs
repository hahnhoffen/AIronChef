using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Application.Recipes.Queries.GetAllRecipes
{
    public record GetAllRecipesQuery() : IRequest<OperationResult<List<Recipe>>>
    {

    }
}
