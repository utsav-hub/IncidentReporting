using MediatR;
using IncidentReporting.Application.Interfaces;
using IncidentReporting.Application.Notifications;
using IncidentReporting.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace IncidentReporting.Application.EventHandlers
{
    public class CreateIncidentHistoryHandler 
        : INotificationHandler<IncidentClosedNotification>   // ✔ FIXED
    {
        private readonly IIncidentHistoryRepository _historyRepository;

        public CreateIncidentHistoryHandler(IIncidentHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        public async Task Handle(IncidentClosedNotification notification, CancellationToken cancellationToken)
        {
            var evt = notification.DomainEvent;

            var history = new IncidentHistory(
                incidentId: evt.IncidentId,
                action: "Closed",
                description: $"Incident closed with resolution: {evt.Resolution}"
            );

            await _historyRepository.AddAsync(history, cancellationToken);
            await _historyRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
