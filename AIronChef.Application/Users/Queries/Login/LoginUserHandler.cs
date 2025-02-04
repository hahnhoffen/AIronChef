using AIronChef.Application.Common.Helpers;
using AIronChef.Application.DTOs;
using AIronChef.Application.Users.Queries.Login.Helpers;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Users.Queries.Login;

public class LoginUserHandler : IRequestHandler<LoginUserQuery, OperationResult<(User, string)>>
{
    private readonly IUserRepository _userRepository;
    private readonly TokenHelper _tokenHelper;

    public LoginUserHandler(IUserRepository userRepository, TokenHelper tokenHelper)
    {
        _userRepository = userRepository;
        _tokenHelper = tokenHelper;
    }

    public async Task<OperationResult<(User, string)>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmail(request.LoginUser.Email);
        bool verify = PasswordHasher.VerifyPassword(request.LoginUser.Password, user);

        if (user is null)
        {
            return OperationResult<(User, string)>.Failure("User does not exists.");
        }

        if (!verify)
        {
            return OperationResult<(User, string)>.Failure("Wrong password.");
        }

        string token = _tokenHelper.GenerateJwtToken(user);

        return OperationResult<(User, string)>.Successfull((user, token));
    }
}
