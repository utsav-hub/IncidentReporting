using MediatR;
using IncidentReporting.Application.Notifications;

public class SendIncidentClosedEmailHandler 
    : INotificationHandler<IncidentClosedNotification>
{
    public Task Handle(IncidentClosedNotification notification, CancellationToken cancellationToken)
    {
        var evt = notification.DomainEvent;

        // use evt.IncidentId, evt.Resolution, etc.
        return Task.CompletedTask;
    }
}
