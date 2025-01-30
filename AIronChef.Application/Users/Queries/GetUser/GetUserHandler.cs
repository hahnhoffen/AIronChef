using AIronChef.Application.Common.Helpers;
using AIronChef.Application.Interfaces;
using AIronChef.Domain.Interfaces;
using AIronChef.Domain.Models;
using MediatR;

namespace AIronChef.Application.Users.Queries.GetUser;

public class GetUserHandler : IRequestHandler<GetUserQuery, OperationResult<User>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILoggingService _loggingService;

    public GetUserHandler(IUserRepository userRepository, ILoggingService loggingService)
    {
        _userRepository = userRepository;
        _loggingService = loggingService;
    }

    public async Task<OperationResult<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        _loggingService.LogInfo($"Fetching user with id: {request.Id}");

        if (request.Id < 0)
        {
            _loggingService.LogWarning("Id must be greater than zero.");
            return OperationResult<User>.Failure("Id must be greater than zero.");
        }

        try
        {
            var user = await _userRepository.GetUserByIdAsync(request.Id);

            if (user is null)
            {
                _loggingService.LogWarning($"User with id {request.Id} not found.");
                return OperationResult<User>.Failure("User not found.");
            }

            _loggingService.LogInfo($"Successfully fetched user with id: {request.Id}");
            return OperationResult<User>.Successfull(user);
        }
        catch (Exception ex)
        {
            _loggingService.LogError("An error occured while fetching the user.", ex);
            return OperationResult<User>.Failure("Something went wrong.");
        }
    }
}
