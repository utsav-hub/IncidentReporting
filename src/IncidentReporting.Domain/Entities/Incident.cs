using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IncidentReporting.Domain.Common;
using IncidentReporting.Domain.DomainEvents;
using Stateless;

namespace IncidentReporting.Domain.Entities
{
    public enum IncidentStatus
    {
        Open = 0,
        InProgress = 1,
        Closed = 2
    }

    public enum IncidentTrigger
    {
        StartProgress,
        Close,
        Reopen
    }

    /// <summary>
    /// Core aggregate root representing an Incident.
    /// It uses a state machine for workflow transitions and raises domain events
    /// when important state changes occur.
    /// </summary>
    public class Incident : EntityBase
    {
        [Key]
        public int Id { get; private set; }

        [Required]
        public string Title { get; private set; } = string.Empty;

        public string? Description { get; private set; }

        public IncidentStatus Status { get; private set; } = IncidentStatus.Open;

        public string? Resolution { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; private set; }

        // EF Core concurrency token
        [Timestamp]
        public byte[]? RowVersion { get; private set; }

        [NotMapped]
        private StateMachine<IncidentStatus, IncidentTrigger>? _stateMachine;

        public Incident() { } // EF Core

        public Incident(string title, string? description)
        {
            Title = title;
            Description = description;
            Status = IncidentStatus.Open;
            CreatedAt = DateTime.UtcNow;

            ConfigureStateMachine();
        }

        private void ConfigureStateMachine()
        {
            _stateMachine = new StateMachine<IncidentStatus, IncidentTrigger>(
                () => Status,
                s => Status = s
            );

            _stateMachine.Configure(IncidentStatus.Open)
                .Permit(IncidentTrigger.StartProgress, IncidentStatus.InProgress)
                .Permit(IncidentTrigger.Close, IncidentStatus.Closed);

            _stateMachine.Configure(IncidentStatus.InProgress)
                .Permit(IncidentTrigger.Close, IncidentStatus.Closed)
                .Permit(IncidentTrigger.Reopen, IncidentStatus.Open);

            _stateMachine.Configure(IncidentStatus.Closed)
                .Permit(IncidentTrigger.Reopen, IncidentStatus.Open);
        }

        public void StartProgress()
        {
            ConfigureStateMachine();
            _stateMachine!.Fire(IncidentTrigger.StartProgress);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Close(string resolution)
        {
            ConfigureStateMachine();
            _stateMachine!.Fire(IncidentTrigger.Close);

            Resolution = resolution;
            UpdatedAt = DateTime.UtcNow;

            // 🔥 Raise a domain event for decoupled operations (emails, audit logs…)
            AddDomainEvent(new IncidentClosedEvent(Id, resolution));
        }

        public void Reopen()
        {
            ConfigureStateMachine();
            _stateMachine!.Fire(IncidentTrigger.Reopen);
            UpdatedAt = DateTime.UtcNow;

            // You could raise a ReopenedEvent here if needed
            // AddDomainEvent(new IncidentReopenedEvent(Id));
        }

        public void UpdateDetails(string? description)
        {
            if (!string.IsNullOrWhiteSpace(description))
                Description = description;

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
