using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Interfaces;
using System.Collections.Concurrent;

namespace IncidentReporting.Infrastructure.Services
{
    /// <summary>
    /// Mock notification service that stores notifications in memory.
    /// In a real implementation, this would use a database or external service.
    /// </summary>
    public class MockNotificationService : INotificationService
    {
        // In-memory storage: userId -> List of notifications
        private static readonly ConcurrentDictionary<int, List<NotificationDto>> _notifications = new();
        private static int _nextId = 1;

        public Task<IEnumerable<NotificationDto>> GetNotificationsAsync(int userId)
        {
            if (_notifications.TryGetValue(userId, out var notifications))
            {
                // Return notifications ordered by most recent first
                return Task.FromResult<IEnumerable<NotificationDto>>(
                    notifications.OrderByDescending(n => n.CreatedAt).ToList()
                );
            }

            return Task.FromResult<IEnumerable<NotificationDto>>(new List<NotificationDto>());
        }

        public Task<NotificationDto> CreateNotificationAsync(int userId, string title, string message, string type = "Info", int? incidentId = null)
        {
            var notification = new NotificationDto
            {
                Id = Interlocked.Increment(ref _nextId),
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                IncidentId = incidentId
            };

            var userNotifications = _notifications.GetOrAdd(userId, _ => new List<NotificationDto>());
            lock (userNotifications)
            {
                userNotifications.Add(notification);
            }

            return Task.FromResult(notification);
        }

        public Task MarkAsReadAsync(int notificationId, int userId)
        {
            if (_notifications.TryGetValue(userId, out var notifications))
            {
                lock (notifications)
                {
                    var notification = notifications.FirstOrDefault(n => n.Id == notificationId);
                    if (notification != null)
                    {
                        notification.IsRead = true;
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task MarkAllAsReadAsync(int userId)
        {
            if (_notifications.TryGetValue(userId, out var notifications))
            {
                lock (notifications)
                {
                    foreach (var notification in notifications)
                    {
                        notification.IsRead = true;
                    }
                }
            }

            return Task.CompletedTask;
        }

        public Task<int> GetUnreadCountAsync(int userId)
        {
            if (_notifications.TryGetValue(userId, out var notifications))
            {
                var count = notifications.Count(n => !n.IsRead);
                return Task.FromResult(count);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Helper method to seed some mock notifications for testing.
        /// This can be called from the controller or during startup.
        /// </summary>
        public Task SeedMockNotificationsAsync(int userId)
        {
            var mockNotifications = new[]
            {
                new { Title = "Welcome!", Message = "Welcome to the Incident Reporting System", Type = "Success" },
                new { Title = "New Incident Created", Message = "A new incident has been reported", Type = "Info" },
                new { Title = "System Update", Message = "The system has been updated with new features", Type = "Info" }
            };

            foreach (var mock in mockNotifications)
            {
                CreateNotificationAsync(userId, mock.Title, mock.Message, mock.Type);
            }

            return Task.CompletedTask;
        }
    }
}

