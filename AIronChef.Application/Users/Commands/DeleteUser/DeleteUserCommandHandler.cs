using AIronChef.Application.Common.Helpers;
using AIronChef.Application.Interfaces;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, OperationResult<User>>
    {
        private readonly IUserRepository _repository;
        private readonly ILoggingService _loggingService;

        public DeleteUserCommandHandler(IUserRepository repository, ILoggingService loggingService)
        {
            _repository = repository;
            _loggingService = loggingService;
        }

        public async Task<OperationResult<User>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            bool success = await _repository.DeleteAsync(command.Id);
            if (!success)
            {
                _loggingService.LogWarning("Invalid id");
                return OperationResult<User>.Failure("Invalid id");
            }
            _loggingService.LogInfo("User deleted");
            return OperationResult<User>.Successfull(null!);
        }
    }
}
