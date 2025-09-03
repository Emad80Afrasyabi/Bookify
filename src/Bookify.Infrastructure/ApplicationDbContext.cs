using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext(DbContextOptions options, IDateTimeProvider dateTimeProvider) : DbContext(options), IUnitOfWork
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            AddDomainEventsAsOutboxMessages();

            int result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occurred.", ex);
        }
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        List<OutboxMessage> outboxMessages = ChangeTracker.Entries<Entity>()
                                          .Select(entry => entry.Entity)
                                          .SelectMany(entity =>
                                          {
                                              IReadOnlyList<IDomainEvent> domainEvents = entity.GetDomainEvents();
                                          
                                              entity.ClearDomainEvents();
                                          
                                              return domainEvents;
                                          })
                                          .Select(domainEvent => new OutboxMessage(id: Guid.NewGuid(),
                                                                                   dateTimeProvider.UtcNow,
                                                                                   type: domainEvent.GetType().Name,
                                                                                   content: JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings))).ToList();

        AddRange(outboxMessages);
    }
}