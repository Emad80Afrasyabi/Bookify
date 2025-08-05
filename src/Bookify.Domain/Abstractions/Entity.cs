namespace Bookify.Domain.Abstractions;

public abstract class Entity(Guid id)
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; init; } = id;

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}