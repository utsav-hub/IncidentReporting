namespace IncidentReporting.Domain.Entities
{
    public class IncidentHistory
    {
        public int Id { get; private set; }
        public int IncidentId { get; private set; }
        public string Action { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public IncidentHistory(int incidentId, string action, string description)
        {
            IncidentId = incidentId;
            Action = action;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
