using AIronChef.Application.Common.Helpers;
using AIronChef.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Application.Users.Queries.GetAllUsers
{
    public record GetAllUsersQuery() : IRequest<OperationResult<List<User>>>
    {
    }
}
