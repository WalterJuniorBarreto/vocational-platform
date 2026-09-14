namespace Vocational.Assessment.Domain.Primitives;

public interface IDomainEvent
{
    Guid EventId => Guid.NewGuid();
    DateTime OccurredOn => DateTime.UtcNow;
}