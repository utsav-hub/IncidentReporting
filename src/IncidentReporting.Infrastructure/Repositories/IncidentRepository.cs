using IncidentReporting.Application.Interfaces;
using IncidentReporting.Domain.Entities;
using IncidentReporting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentReporting.Infrastructure.Repositories
{
    /// <summary>
    /// EF Core implementation of IIncidentRepository.
    /// The Application layer depends only on the interface, not this class.
    /// </summary>
    public class IncidentRepository : IIncidentRepository
    {
        private readonly AppDbContext _context;

        public IncidentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Incident?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Incidents
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<List<Incident>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Incidents
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Incident incident, CancellationToken cancellationToken = default)
        {
            await _context.Incidents.AddAsync(incident, cancellationToken);
        }

        public Task UpdateAsync(Incident incident, CancellationToken cancellationToken = default)
        {
            _context.Incidents.Update(incident);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Incident incident, CancellationToken cancellationToken = default)
        {
            _context.Incidents.Remove(incident);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
