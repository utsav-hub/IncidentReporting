using System.ComponentModel.DataAnnotations;
using IncidentReporting.Domain.Entities;

namespace IncidentReporting.Domain.Entities;

public class IncidentHistory
{
    [Key]
    public int Id { get; set; }

    // The incident this audit entry belongs to
    public int IncidentId { get; set; }

    // Status before the change
    public IncidentStatus FromStatus { get; set; }

    // Status after the change
    public IncidentStatus ToStatus { get; set; }

    // Who changed it (in this project we use "system")
    public string? ChangedBy { get; set; }

    // Timestamp of the change
    public DateTime ChangedAt { get; set; }
}
