namespace IncidentReporting.Application.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "Info"; // Info, Warning, Success, Error
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? IncidentId { get; set; }
    }
}

