using AIronChef.Application.Common.Helpers;
using MediatR;
using AIronChef.Domain.Models;
using AIronChef.Domain.Interfaces;
using AIronChef.Application.Interfaces;

namespace AIronChef.Application.Users.Commands.UpdateUser
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, OperationResult<User>>
    {
        private readonly IUserRepository _repository;
        private readonly ILoggingService _loggingService;

        public UpdateUserHandler(IUserRepository repository, ILoggingService loggingService)
        {
            _repository = repository;
            _loggingService = loggingService;
        }

        public async Task<OperationResult<User>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            _loggingService.LogInfo($"Updating user with id: {command.Id}");
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return OperationResult<User>.Failure("The new User's name cannot be empty.");
            }
            if (!ValidationHelper.IsValidEmail(command.Email))
            {
                return OperationResult<User>.Failure("Invalid email.");
            }

            // Retrieve existing user from the repository
            var existingUser = await _repository.GetByIdAsync(command.Id)!;
            if (existingUser == null)
            {
                return OperationResult<User>.Failure("User not found");
            }

            // If no changes are made, return success with the existing user
            if (existingUser.Email == command.Email && existingUser.Name == command.Name)
            {
                return OperationResult<User>.Successfull(existingUser);
            }

            // Check if the new email is unique before updating
            if (existingUser.Email != command.Email)
            {
                bool isEmailUnique = await _repository.IsEmailUniqueAsync(command.Email);
                if (!isEmailUnique)
                {
                    return OperationResult<User>.Failure("New email is not unique");
                }
            }

            // Update only the changed fields
            existingUser.Name = command.Name;
            existingUser.Email = command.Email;

            try
            {
                User updatedUser = await _repository.UpdateAsync(existingUser)!;

                return OperationResult<User>.Successfull(updatedUser);
            }
            catch (Exception ex)
            {
                return OperationResult<User>.Failure($"An error occurred while updating the user: {ex.Message}");
            }
        }

    }
}

