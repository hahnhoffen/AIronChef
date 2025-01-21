using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Users.Queries.GetUser
{
    public record GetUserQuery(int Id) : IRequest<OperationResult<User>>;
}
