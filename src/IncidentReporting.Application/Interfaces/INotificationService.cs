using IncidentReporting.Application.DTOs;

namespace IncidentReporting.Application.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetNotificationsAsync(int userId);
        Task<NotificationDto> CreateNotificationAsync(int userId, string title, string message, string type = "Info", int? incidentId = null);
        Task MarkAsReadAsync(int notificationId, int userId);
        Task MarkAllAsReadAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}

