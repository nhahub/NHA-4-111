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
        private int CurrentUserId => 1;

        public MemberDashboardController(IMemberDashboardService service) => _service = service;

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile() => Ok(await _service.GetProfileStatsAsync(CurrentUserId));

        [HttpGet("next-class")]
        public async Task<IActionResult> GetNextClass() => Ok(await _service.GetNextClassAsync(CurrentUserId));

        [HttpPost("attendance/check-in")]
        public async Task<IActionResult> CheckIn() => Ok(await _service.CheckInAsync(CurrentUserId));

        [HttpGet("notifications")]
        public async Task<IActionResult> GetNotifications() => Ok(await _service.GetNotificationsAsync(CurrentUserId));

        [HttpGet("digital-pass")]
        public async Task<IActionResult> GetDigitalPass() => Ok(await _service.GetDigitalPassAsync(CurrentUserId));
    }
}
