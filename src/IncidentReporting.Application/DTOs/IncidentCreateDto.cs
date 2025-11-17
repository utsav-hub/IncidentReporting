namespace IncidentReporting.Application.DTOs
{
    public class IncidentCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? CategoryId { get; set; }
    }
}
