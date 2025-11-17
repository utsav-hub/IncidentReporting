using IncidentReporting.Application.Interfaces;
using IncidentReporting.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IncidentReporting.Infrastructure.Data;

namespace IncidentReporting.Infrastructure.Repositories
{
    public class IncidentHistoryRepository : IIncidentHistoryRepository
    {
        private readonly AppDbContext _context;

        public IncidentHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(IncidentHistory history, CancellationToken cancellationToken)
        {
            await _context.IncidentHistories.AddAsync(history, cancellationToken);
        }

        public async Task<List<IncidentHistory>> GetByIncidentIdAsync(int incidentId, CancellationToken cancellationToken)
        {
            return await _context.IncidentHistories
                                 .AsNoTracking()
                                 .Where(x => x.IncidentId == incidentId)
                                 .ToListAsync(cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
