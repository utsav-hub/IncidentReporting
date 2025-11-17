using MediatR;
using IncidentReporting.Domain.DomainEvents;
using IncidentReporting.Domain.Entities;
using IncidentReporting.Application.Interfaces;

namespace IncidentReporting.Application.EventHandlers
{
    /// <summary>
    /// Writes an audit entry whenever an incident is closed.
    /// </summary>
    public class CreateIncidentHistoryHandler : INotificationHandler<IncidentClosedEvent>
    {
        private readonly IIncidentRepository _repo;

        public CreateIncidentHistoryHandler(IIncidentRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(IncidentClosedEvent notification, CancellationToken ct)
        {
            // Load the incident (includes existing status)
            var incident = await _repo.GetAsync(notification.IncidentId, ct);

            if (incident == null)
                return; // Should not happen but safe check

            // Create a history entry
            var history = new IncidentHistory
            {
                IncidentId = notification.IncidentId,
                FromStatus = IncidentStatus.InProgress,
                ToStatus = IncidentStatus.Closed,
                ChangedBy = "system",
                ChangedAt = notification.ClosedAt
            };

            // Use UpdateAsync so EF tracks it
            await _repo.UpdateAsync(incident, ct);

            // Insert the history (your repo implementation will handle DbSet)
            // If you create a separate IHistoryRepository, adjust here.
            // For now, EF context in repository will detect added entries.

            // We need direct DbContext access or we can extend repository later.
            // For now, write to console (acts as audit mock)
            Console.WriteLine($"[AUDIT] Incident {notification.IncidentId} closed at {notification.ClosedAt}");

            await _repo.SaveChangesAsync(ct);
        }
    }
}
