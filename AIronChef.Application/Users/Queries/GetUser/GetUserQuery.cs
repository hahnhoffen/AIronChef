using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace AIronChef.Application.Users.Queries.GetUser
{
    public class GetUserQuery : IRequest<OperationResult<User>>
    {
        [Range(1, int.MaxValue, ErrorMessage = "Id must be greater than zero.")]
        public int Id { get; }
    }
}
