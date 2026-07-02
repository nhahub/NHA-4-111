using FitCore.BLL.Interfaces.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitCore.API.Controllers.Notifications
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = AppRoles.Admin + "," + AppRoles.Manager)]
    public class NotificationController (INotificationService _notificationService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery(Name = "Page_Size")] int pageSize = 20, [FromQuery(Name = "Page")] int page = 1)
        {
            var result = await _notificationService.GetAllNotifications(page, pageSize);
            return Ok(result);
        }
        [HttpPatch("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkAsReadAsync(id);
            if (!result) return NotFound("Notification not found.");

            return Ok(new { Message = "Notification marked as read." });
        }

        [HttpPatch("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync();

            return Ok(new { Message = "All notifications marked as read." });
        }
    }
}
