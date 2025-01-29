using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;


namespace AIronChef.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<OperationResult<User>>
    {
        // User ID to update.
        public int Id;

        // New name.
        public required string Name;

        // New email.
        public required string Email;
    }
}
