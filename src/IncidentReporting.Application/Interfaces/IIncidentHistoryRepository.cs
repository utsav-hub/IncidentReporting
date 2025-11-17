using IncidentReporting.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace IncidentReporting.Application.Interfaces
{
    public interface IIncidentHistoryRepository
    {
        Task AddAsync(IncidentHistory history, CancellationToken cancellationToken);
        Task<List<IncidentHistory>> GetByIncidentIdAsync(int incidentId, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
