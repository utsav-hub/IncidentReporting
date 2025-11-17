using System.ComponentModel.DataAnnotations.Schema;
using IncidentReporting.Domain.DomainEvents;

namespace IncidentReporting.Domain.Common
{
    /// <summary>
    /// Base entity supporting Domain Events (DDD style).
    /// </summary>
    public abstract class EntityBase
    {
        [NotMapped]
        private List<IDomainEvent>? _domainEvents;

        [NotMapped]
        public IReadOnlyCollection<IDomainEvent> DomainEvents =>
            (_domainEvents ??= new List<IDomainEvent>()).AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            (_domainEvents ??= new List<IDomainEvent>()).Add(domainEvent);
        }

        protected void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents?.Remove(domainEvent);
        }

        protected void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        /// <summary>
        /// Used by DbContext to dispatch domain events after SaveChanges.
        /// MUST NOT be marked with [NotMapped] because attributes are not allowed on methods.
        /// </summary>
        public List<IDomainEvent> PopDomainEvents()
        {
            var events = _domainEvents ?? new List<IDomainEvent>();
            _domainEvents = new List<IDomainEvent>(); // reset list
            return events;
        }
    }
}
