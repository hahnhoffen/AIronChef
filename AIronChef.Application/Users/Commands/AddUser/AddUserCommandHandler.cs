using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Enums;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Users.Commands.AddUser
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, OperationResult<User>>
    {
        private readonly IUserRepository _repository;

        public AddUserCommandHandler(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<User>> Handle(AddUserCommand command, CancellationToken cancellationToken)
        {
            // Check if title is valid
            if (string.IsNullOrWhiteSpace(command.Name))
            {
                return OperationResult<User>.Failure("The User name can not be empty");
            }
            // Check if password is set
            if (string.IsNullOrWhiteSpace(command.Password))
            {
                return OperationResult<User>.Failure("The Password can not be empty");
            }
            if (await _repository.IsEmailUniqueAsync(command.Email) == false)
            {
                return OperationResult<User>.Failure("The Email already exist");
            }

            var salt = PasswordHasher.GenerateSalt();
            var passwordHash = PasswordHasher.HashPassword(command.Password, salt);
            var userDTO = new User
            {
                Name = command.Name,
                Email = command.Email,
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                Role = UserRole.User
            };
            // The caller should handle exceptions of unforeseen errors.
            User user = await _repository.AddAsync(userDTO);
            return OperationResult<User>.Successfull(userDTO);
        }
    }
}
