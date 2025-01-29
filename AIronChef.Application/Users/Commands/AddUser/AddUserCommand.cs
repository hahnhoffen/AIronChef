using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Users.Commands.AddUser
{
    public class AddUserCommand : IRequest<OperationResult<User>>
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
