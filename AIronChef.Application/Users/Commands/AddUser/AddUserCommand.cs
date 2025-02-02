using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Users.Commands.AddUser
{
    public class AddUserCommand : IRequest<OperationResult<User>>
    {
        public string Name { get; }
        public string Email { get; }
        public string Password { get; }

        public AddUserCommand(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }
    }
}
