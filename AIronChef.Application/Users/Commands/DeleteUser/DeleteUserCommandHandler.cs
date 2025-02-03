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
            if (command.Id <= 0)
            {
                _loggingService.LogWarning("Invalid command: User ID is not valid.");
                return OperationResult<User>.Failure("Invalid command: User ID is not valid.");
            }

            var user = await _repository.GetByIdAsync(command.Id);
            if (user == null)
            {
                _loggingService.LogWarning($"User with ID {command.Id} does not exist.");
                return OperationResult<User>.Failure($"User with ID {command.Id} does not exist.");
            }

            try
            {
                bool success = await _repository.DeleteAsync(command.Id);
                if (!success)
                {
                    _loggingService.LogWarning("Failed to delete the user.");
                    return OperationResult<User>.Failure("Failed to delete the user.");
                }

                _loggingService.LogInfo("User deleted successfully.");
                return OperationResult<User>.Successfull(null!);
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"An error occurred while trying to delete the user: {ex.Message}", ex);
                return OperationResult<User>.Failure("An error occurred while trying to delete the user.");
            }
        }
    }
}
