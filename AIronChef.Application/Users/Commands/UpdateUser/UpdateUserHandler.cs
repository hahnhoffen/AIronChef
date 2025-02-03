using AIronChef.Application.Common.Helpers;
using MediatR;
using AIronChef.Domain.Models;
using AIronChef.Domain.Interfaces;

namespace AIronChef.Application.Users.Commands.UpdateUser
{
    internal class UpdateUserHandler : IRequestHandler<UpdateUserCommand, OperationResult<User>>
    {
        private readonly IUserRepository _repository;

        public UpdateUserHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<User>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return OperationResult<User>.Failure("The new User's name cannot be empty.");
            }

            // Retrieve existing user from the repository
            User existingUser = await _repository.GetByIdAsync(command.Id);
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

            User updatedUser = await _repository.UpdateAsync(existingUser);
    
            return OperationResult<User>.Successfull(updatedUser);
        }

    }
}

