using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, OperationResult<User>>
    {
        private readonly IUserRepository _repository;

        public DeleteUserCommandHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<User>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            User user = await _repository.DeleteUserAsync(command.Id);
            if (user == null)
            {
                return OperationResult<User>.Failure("Invalid id");
            }
            return OperationResult<User>.Successfull(user);
        }
    }
}
