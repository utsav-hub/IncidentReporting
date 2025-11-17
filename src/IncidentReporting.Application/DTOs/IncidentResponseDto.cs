using IncidentReporting.Domain.Entities;

namespace IncidentReporting.Application.DTOs
{
    public class IncidentResponseDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public IncidentStatus Status { get; set; }

        public string? Resolution { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
