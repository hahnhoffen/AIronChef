using AIronChef.Application.Common.Helpers;
using AIronChef.Application.DTOs;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Users.Queries.Login;

public class LoginUserQuery : IRequest<OperationResult<(User, string)>>
{
    public LoginUserQuery(LoginUserDto loginUser)
    {
        LoginUser = loginUser;
    }

    public LoginUserDto LoginUser { get; }
}
