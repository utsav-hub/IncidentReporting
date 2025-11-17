using IncidentReporting.Domain.Common;

namespace IncidentReporting.Domain.DomainEvents
{
    /// <summary>
    /// Raised when an Incident is successfully closed.
    /// 
    /// This event does NOT perform any side effects (like sending emails).
    /// The Application/Infrastructure layers will handle side effects by subscribing
    /// to this event via MediatR notification handlers.
    /// </summary>
    public sealed class IncidentClosedEvent : IDomainEvent
    {
        public int IncidentId { get; }
        public string Resolution { get; }
        public DateTime ClosedAt { get; }

        public IncidentClosedEvent(int incidentId, string resolution)
        {
            IncidentId = incidentId;
            Resolution = resolution;
            ClosedAt = DateTime.UtcNow;
        }
    }
}
