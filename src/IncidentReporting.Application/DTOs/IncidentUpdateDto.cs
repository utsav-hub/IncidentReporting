using IncidentReporting.Domain.Entities;

namespace IncidentReporting.Application.DTOs
{
    public class IncidentUpdateDto
    {
        public string? Description { get; set; }
        public IncidentStatus Status { get; set; }
        public string? Resolution { get; set; }
    }
}
