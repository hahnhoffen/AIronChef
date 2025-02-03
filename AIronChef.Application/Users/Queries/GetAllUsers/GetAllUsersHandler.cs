using AIronChef.Application.Common.Helpers;
using AIronChef.Application.Interfaces;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIronChef.Application.Users.Queries.GetAllUsers
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, OperationResult<List<User>>>
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly ILoggingService _loggingService;

        public GetAllUsersHandler(IGenericRepository<User> userRepository, ILoggingService loggingService)
        {
            _userRepository = userRepository;
            _loggingService = loggingService;
        }


        public async Task<OperationResult<List<User>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {

            _loggingService.LogInfo("Fetching all users.");

            try
            {
                var users = await _userRepository.GetAllAsync();

                if (users is null || !users.Any())
                {
                    _loggingService.LogWarning("No users found.");
                    return OperationResult<List<User>>.Failure("No users found.");
                }

                _loggingService.LogInfo("Successfully fetched all users.");
                return OperationResult<List<User>>.Successfull(users.ToList());
            }
            catch (Exception ex)
            {
                _loggingService.LogError("An error occured while fetching the users.", ex);
                return OperationResult<List<User>>.Failure("Something went wrong.");
            }
        }
    }
}
