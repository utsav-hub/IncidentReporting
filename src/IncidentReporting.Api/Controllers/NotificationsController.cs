using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Interfaces;

namespace IncidentReporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // Helper method to get current user ID from JWT claims
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }

        // --------------------------------------------------------
        // GET: api/notifications
        // --------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = GetCurrentUserId();
            var notifications = await _notificationService.GetNotificationsAsync(userId);
            return Ok(notifications);
        }

        // --------------------------------------------------------
        // GET: api/notifications/unread-count
        // --------------------------------------------------------
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = GetCurrentUserId();
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { count });
        }

        // --------------------------------------------------------
        // POST: api/notifications/{id}/mark-read
        // --------------------------------------------------------
        [HttpPost("{id:int}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetCurrentUserId();
            await _notificationService.MarkAsReadAsync(id, userId);
            return NoContent();
        }

        // --------------------------------------------------------
        // POST: api/notifications/mark-all-read
        // --------------------------------------------------------
        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            await _notificationService.MarkAllAsReadAsync(userId);
            return NoContent();
        }

        // --------------------------------------------------------
        // POST: api/notifications/seed-mock (for testing)
        // --------------------------------------------------------
        [HttpPost("seed-mock")]
        public async Task<IActionResult> SeedMockNotifications()
        {
            var userId = GetCurrentUserId();
            if (_notificationService is Infrastructure.Services.MockNotificationService mockService)
            {
                await mockService.SeedMockNotificationsAsync(userId);
                return Ok(new { message = "Mock notifications seeded" });
            }
            return BadRequest(new { message = "Mock service not available" });
        }
    }
}

