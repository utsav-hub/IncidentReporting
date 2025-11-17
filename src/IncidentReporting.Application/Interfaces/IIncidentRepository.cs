using IncidentReporting.Domain.Entities;

namespace IncidentReporting.Application.Interfaces
{
    /// <summary>
    /// Contract for working with Incident entities.
    /// Infrastructure layer implements this interface (e.g., EF Core repository).
    /// Application layer (handlers) depends on this abstraction.
    /// </summary>
    public interface IIncidentRepository
    {
        Task<Incident?> GetAsync(int id, CancellationToken cancellationToken = default);

        Task<List<Incident>> GetAllAsync(CancellationToken cancellationToken = default);

        Task AddAsync(Incident incident, CancellationToken cancellationToken = default);

        Task UpdateAsync(Incident incident, CancellationToken cancellationToken = default);

        Task DeleteAsync(Incident incident, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
