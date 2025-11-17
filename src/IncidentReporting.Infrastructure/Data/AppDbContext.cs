using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using IncidentReporting.Domain.Common;
using IncidentReporting.Domain.Entities;

namespace IncidentReporting.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Incident> Incidents => Set<Incident>();
        public DbSet<IncidentHistory> IncidentHistories => Set<IncidentHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations automatically (if added)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Example: configure IncidentHistory (optional, EF will auto-map)
            modelBuilder.Entity<IncidentHistory>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<IncidentHistory>()
                .Property(x => x.ChangedAt)
                .IsRequired();
        }

        /// <summary>
        /// Override SaveChangesAsync to dispatch domain events AFTER the transaction is committed.
        /// Ensures domain events run only if data is successfully saved.
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Save database changes first
            var result = await base.SaveChangesAsync(cancellationToken);

            // Then dispatch domain events
            await DispatchDomainEventsAsync(cancellationToken);

            return result;
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            // Get all entities that have domain events
            var domainEntities = ChangeTracker
                .Entries<EntityBase>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var allEvents = domainEntities
                .SelectMany(e => e.PopDomainEvents())
                .ToList();

            // Publish via MediatR
            foreach (var domainEvent in allEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
