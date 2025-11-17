using MediatR;
using IncidentReporting.Domain.DomainEvents;

namespace IncidentReporting.Application.Notifications
{
    /// <summary>
    /// Application-layer wrapper for domain event.
    /// </summary>
    public class IncidentClosedNotification : INotification
    {
        public IncidentClosedEvent DomainEvent { get; }

        public IncidentClosedNotification(IncidentClosedEvent domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
