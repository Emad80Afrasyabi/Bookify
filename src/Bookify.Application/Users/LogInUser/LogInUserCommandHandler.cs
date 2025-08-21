using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;

namespace Bookify.Application.Users.LogInUser;

internal sealed class LogInUserCommandHandler(IJwtService jwtService) : ICommandHandler<LogInUserCommand, AccessTokenResponse>
{
    public async Task<Result<AccessTokenResponse>> Handle(LogInUserCommand request, CancellationToken cancellationToken)
    {
        Result<string> result = await jwtService.GetAccessTokenAsync(request.Email, request.Password, cancellationToken);

        return result.IsFailure ? Result.Failure<AccessTokenResponse>(UserErrors.InvalidCredentials) : new AccessTokenResponse(result.Value);
    }
}