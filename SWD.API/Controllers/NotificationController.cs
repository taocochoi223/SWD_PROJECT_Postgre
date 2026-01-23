using Microsoft.AspNetCore.Mvc;
using SWD.API.Dtos;
using SWD.BLL.Interfaces;

namespace SWD.API.Controllers
{
    [Route("api/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notiService;

        public NotificationController(INotificationService notiService)
        {
            _notiService = notiService;
        }

        /// <summary>
        /// Get User Notifications
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserNotificationsAsync(int userId)
        {
            var notis = await _notiService.GetUserNotificationsAsync(userId);

            var notiDtos = notis.Select(n => new NotificationDto
            {
                NotiId = n.NotiId,
                HistoryId = n.HistoryId,
                UserId = n.UserId,
                Message = n.Message,
                SentAt = n.SentAt,
                IsRead = n.IsRead,
                SensorId = n.History?.SensorId,
                SensorName = n.History?.Sensor?.Name,
                Severity = n.History?.Severity,
                TriggeredAt = n.History?.TriggeredAt
            }).ToList();

            return Ok(notiDtos);
        }

        /// <summary>
        /// Get Unread Notifications Count
        /// </summary>
        [HttpGet("user/{userId}/unread-count")]
        public async Task<IActionResult> GetUnreadCountAsync(int userId)
        {
            var notis = await _notiService.GetUserNotificationsAsync(userId);
            var unreadCount = notis.Count(n => n.IsRead == false);

            return Ok(new { unread_count = unreadCount });
        }

        /// <summary>
        /// Mark Notification as Read
        /// </summary>
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsReadAsync(long id)
        {
            await _notiService.MarkAsReadAsync(id);
            return Ok(new { message = "Notification marked as read", id = id });
        }
    }
}