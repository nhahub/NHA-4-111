using FitCore.BLL.Interfaces.Attendance;
using FitCore.Shared.DTOs.Attendance;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FitCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        // ==========================================
        // 1) Self-Service (Member App) - 5 Endpoints
        // ==========================================
        [HttpGet("me/qrcode")]
        public async Task<IActionResult> GetMyQrCode(int userId)
        {
            var result = await _attendanceService.GetMyQrCodeAsync(userId);
            return Ok(result); 
        }

        [HttpPost("me/qrcode/regenerate")]
        public async Task<IActionResult> RegenerateMyQrCode([FromQuery] int userId) => Ok(await _attendanceService.RegenerateMyQrCodeAsync(userId));

        [HttpGet("me/status-today")]
        public async Task<IActionResult> GetMyStatusToday([FromQuery] int userId) => Ok(await _attendanceService.GetMyStatusTodayAsync(userId));

        [HttpGet("me/history")]
        public async Task<IActionResult> GetMyHistory([FromQuery] int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(await _attendanceService.GetMyHistoryAsync(userId, page, pageSize));

        [HttpGet("me/stats")]
        public async Task<IActionResult> GetMyStats([FromQuery] int userId) => Ok(await _attendanceService.GetMyStatsAsync(userId));


        // ==========================================
        // 2) Reception / GymOps Terminal - 6 Endpoints
        // ==========================================
        [HttpPost("checkin/scan")]
        public async Task<IActionResult> CheckInByScan([FromBody] CheckInRequestDto request) => Ok(await _attendanceService.CheckInByScanAsync(request));

        [HttpPost("checkin/manual")]
        public async Task<IActionResult> CheckInManual([FromQuery] string searchInput) => Ok(await _attendanceService.CheckInManualAsync(searchInput));

        [HttpGet("members/search")] 
        public async Task<IActionResult> SearchMembers([FromQuery] string query) => Ok(await _attendanceService.SearchMembersAsync(query));

        [HttpGet("members/{userId}/checkin-summary")]
        public async Task<IActionResult> GetMemberCheckInSummary([FromRoute] int userId) => Ok(await _attendanceService.GetMemberCheckInSummaryAsync(userId));

        [HttpGet("recent-scans")]
        public async Task<IActionResult> GetRecentScans() => Ok(await _attendanceService.GetRecentScansAsync());

        [HttpGet("daily-logs")]
        public async Task<IActionResult> GetDailyLogs() => Ok(await _attendanceService.GetDailyLogsAsync());


        // ==========================================
        // 3) Class Check-in  - 5 Endpoints
        // ==========================================
        [HttpGet("classes")]
        public async Task<IActionResult> GetClasses() => Ok(await _attendanceService.GetAvailableClassesAsync());

        [HttpGet("classes/{id}/availability")]
        public async Task<IActionResult> GetClassAvailability([FromRoute] int id) => Ok(await _attendanceService.GetClassAvailabilityAsync(id));

        [HttpPost("checkin/class")]
        public async Task<IActionResult> CheckInClass([FromQuery] int userId, [FromQuery] int classId) => Ok(await _attendanceService.CheckInClassAsync(userId, classId));
    }
}
