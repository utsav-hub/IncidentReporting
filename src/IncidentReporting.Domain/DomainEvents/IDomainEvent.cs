namespace IncidentReporting.Domain.DomainEvents
{
    /// <summary>
    /// Marker interface for domain events.
    /// 
    /// Domain events are plain CLR objects that describe something important
    /// that happened in the domain (for example: IncidentClosedEvent).
    /// 
    /// Note: this interface intentionally has no dependencies on MediatR or any
    /// transport library to keep the Domain layer pure. The Infrastructure layer
    /// will be responsible for dispatching implementations of this interface
    /// to application/infrastructure handlers (for example via MediatR).
    /// </summary>
    public interface IDomainEvent
    {
        // Marker only - no members required.
    }
}
