using MediatR;
using IncidentReporting.Domain.DomainEvents;

namespace IncidentReporting.Application.EventHandlers
{
    /// <summary>
    /// Mock email notification handler.
    /// In a real system, you'd inject IEmailService and send a real email.
    /// </summary>
    public class SendIncidentClosedEmailHandler : INotificationHandler<IncidentClosedEvent>
    {
        public Task Handle(IncidentClosedEvent notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[EMAIL MOCK] Incident {notification.IncidentId} was closed. Resolution: {notification.Resolution}");

            return Task.CompletedTask;
        }
    }
}
