using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;

internal sealed class AuthorizationService(ApplicationDbContext dbContext, ICacheService cacheService)
{
    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        var cacheKey = $"auth:roles-{identityId}";
        var cachedRoles = await cacheService.GetAsync<UserRolesResponse>(cacheKey);

        if (cachedRoles is not null)
            return cachedRoles;
        
        UserRolesResponse roles = await dbContext.Set<User>().Where(user => user.IdentityId == identityId)
                                                             .Select(user => new UserRolesResponse
                                                             {
                                                                 UserId = user.Id,
                                                                 Roles = user.Roles.ToList()
                                                             })
                                                             .FirstAsync();
        
        await cacheService.SetAsync(cacheKey, roles);

        return roles;
    }
    
    public async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var cacheKey = $"auth:permissions-{identityId}";
        var cachedPermissions = await cacheService.GetAsync<HashSet<string>>(cacheKey);

        if (cachedPermissions is not null)
            return cachedPermissions;
        
        ICollection<Permission> permissions = await dbContext.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .SelectMany(user => user.Roles.Select(r => r.Permissions))
            .FirstAsync();
        
        await cacheService.SetAsync(cacheKey, permissions);

        return permissions.Select(permission => permission.Name).ToHashSet();
    }
}