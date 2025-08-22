using System.Data;
using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Dapper;

namespace Bookify.Application.Users.GetLoggedInUser;

internal sealed class GetLoggedInUserQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IUserContext userContext) : IQueryHandler<GetLoggedInUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetLoggedInUserQuery request, CancellationToken cancellationToken)
    {
        using IDbConnection connection = sqlConnectionFactory.CreateConnection();

        const string sql = $"""
                            SELECT
                                id AS Id,
                                first_name AS FirstName,
                                last_name AS LastName,
                                email AS Email
                            FROM users
                            WHERE identity_id = @IdentityId
                            """;

        var user = await connection.QuerySingleAsync<UserResponse>(sql, param: new { userContext.IdentityId });

        return user;
    }
}