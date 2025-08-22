using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Repositories;

internal sealed class UserRepository(ApplicationDbContext dbContext) : Repository<User>(dbContext), IUserRepository
{
    public override void Add(User entity)
    {
        foreach (Role role in entity.Roles)
            DbContext.Attach(role);
        
        base.Add(entity);
    }
};