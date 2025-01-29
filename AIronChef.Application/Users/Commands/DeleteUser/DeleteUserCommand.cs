using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommand : IRequest<OperationResult<User>>
    {
        public int Id;
    }
}
