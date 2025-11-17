using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using IncidentReporting.Domain.DomainEvents;

namespace IncidentReporting.Domain.Common
{
    /// <summary>
    /// Base entity that supports domain events.
    /// Keep Domain layer pure (no MediatR references). Domain events are simple DTO-like objects
    /// implementing IDomainEvent. The Infrastructure layer (DbContext) will dispatch these after persistence.
    /// </summary>
    public abstract class EntityBase
    {
        // EF should ignore this property (it's a runtime-only collection).
        [NotMapped]
        private List<IDomainEvent>? _domainEvents;

        [NotMapped]
        public IReadOnlyCollection<IDomainEvent> DomainEvents => (_domainEvents ??= new List<IDomainEvent>()).AsReadOnly();

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

        // Optional helper visible to infrastructure to pop and clear events atomically.
        [NotMapped]
        public List<IDomainEvent> PopDomainEvents()
        {
            var events = _domainEvents ?? new List<IDomainEvent>();
            _domainEvents = new List<IDomainEvent>();
            return events;
        }
    }
}
