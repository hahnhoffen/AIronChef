using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;


namespace AIronChef.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommand : IRequest<OperationResult<User>>
    {
        public int Id { get; }
        public string Name { get; }
        public string Email { get; }

        public UpdateUserCommand(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }
}
