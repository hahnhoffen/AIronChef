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
                return OperationResult<User>.Failure("The new User's name can not be empty.");
            }
            User existingUser = await _repository.GetByIdAsync(command.Id)!;
            if (existingUser == null)
            {
                return OperationResult<User>.Failure("User not found");
            }
            if (existingUser.Email == command.Email && existingUser.Name == command.Name)
            {
                // Requested value changes are the same as existing values, no need to change anything in the database.
                return OperationResult<User>.Successfull(existingUser);
            }
            if (existingUser.Email != command.Email)
            {
                // Check if new email contain anything
                if (string.IsNullOrWhiteSpace(command.Email))
                {
                    return OperationResult<User>.Failure("The new Email is empty");
                }
                // Check if new email is valid
                if (ValidationHelper.IsValidEmail(command.Email) == false)
                {
                    return OperationResult<User>.Failure("The new Email does not look valid");
                }
                bool uniqueNewEmail = await _repository.IsEmailUniqueAsync(command.Email);
                if (!uniqueNewEmail)
                {
                    return OperationResult<User>.Failure("New email is not unique");
                }
            }
            var userDTO = new User
            {
                Id = command.Id,
                Name = command.Name,
                Email = command.Email
            };
            User updatedUser = await _repository.UpdateAsync(userDTO)!;
            return OperationResult<User>.Successfull(updatedUser);
        }
    }
}

