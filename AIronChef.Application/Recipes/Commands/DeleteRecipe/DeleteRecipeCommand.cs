using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Application.Recipes.Commands.DeleteRecipe
{
    public class DeleteRecipeCommand : IRequest<OperationResult<Recipe>>
    {
        public int Id { get; set; }

        public bool IsValid()
        {
            return ValidationHelper.IsIdValid(Id);
        }
    }
}
