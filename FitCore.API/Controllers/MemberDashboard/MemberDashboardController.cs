using FitCore.BLL.Interfaces.MemberDashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitCore.API.Controllers.MemberDashboard
{
    [Route("api/member/[controller]")]
    [ApiController]
    public class MemberDashboardController : ControllerBase
    {
        private readonly IMemberDashboardService _service;

        public MemberDashboardController(IMemberDashboardService service) => _service = service;

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile([FromQuery] int userId) => Ok(await _service.GetProfileStatsAsync(userId));

        [HttpGet("next-class")]
        public async Task<IActionResult> GetNextClass([FromQuery] int userId) => Ok(await _service.GetNextClassAsync(userId));

        [HttpPost("attendance/check-in")]
        public async Task<IActionResult> CheckIn([FromQuery] int userId) => Ok(await _service.CheckInAsync(userId));

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications([FromQuery] int userId) => Ok(await _service.GetNotificationsAsync(userId));

        [HttpGet("digital-pass")]
        public async Task<IActionResult> GetDigitalPass([FromQuery] int userId) => Ok(await _service.GetDigitalPassAsync(userId));
    }
}
