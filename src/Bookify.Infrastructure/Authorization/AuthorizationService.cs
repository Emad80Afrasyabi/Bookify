using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

internal sealed class AuthorizationService(ApplicationDbContext dbContext)
{
    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        UserRolesResponse roles = await dbContext.Set<User>().Where(user => user.IdentityId == identityId)
                                                             .Select(user => new UserRolesResponse
                                                             {
                                                                 UserId = user.Id,
                                                                 Roles = user.Roles.ToList()
                                                             })
                                                             .FirstAsync();

        return roles;
    }
    
    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        ICollection<Permission> permissions = await dbContext.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .SelectMany(user => user.Roles.Select(r => r.Permissions))
            .FirstAsync();

        return permissions.Select(permission => permission.Name).ToHashSet();
    }
}