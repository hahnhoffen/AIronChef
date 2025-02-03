using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Recipes.Commands.DeleteRecipe
{
    public class DeleteRecipeCommand : IRequest<OperationResult<Recipe>>
    {
        public int Id { get; set; }

        public DeleteRecipeCommand(int id)
        {
            Id = id;
        }

        public bool IsValid()
        {
            return ValidationHelper.IsIdValid(Id);
        }
    }
}
