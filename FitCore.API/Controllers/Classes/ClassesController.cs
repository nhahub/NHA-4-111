using FitCore.BLL.Interfaces.Classes;
using FitCore.Shared.DTOs.Classes;
using Microsoft.AspNetCore.Mvc;

namespace FitCore.API.Controllers.Classes
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassesController(IClassService classService) : ControllerBase
    {
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateClass(CreateClassDto dto)
        {
            var result = await classService.CreateClassAsync(dto);
            return CreatedAtAction(nameof(GetClassById), new { classId = result.ClassID }, result);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("{classId}")]
        public async Task<IActionResult> UpdateClass(int classId, UpdateClassDto dto)
        {
            var result = await classService.UpdateClassAsync(classId, dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClasses([FromQuery(Name = "Page_Size")] int pageSize = 20, [FromQuery(Name = "Page")] int page = 1)
        {
            var result = await classService.GetAllClassesAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{classId}")]
        public async Task<IActionResult> GetClassById(int classId)
        {
            var result = await classService.GetClassByIdAsync(classId);
            return Ok(result);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("{classId}/schedules")]
        public async Task<IActionResult> AddSchedule(int classId, ClassScheduleDto dto)
        {
            var result = await classService.AddScheduleAsync(classId, dto);
            return Ok(result);
        }

        // Members browse bookable class occurrences within a date range
        [HttpGet("browse")]
        public async Task<IActionResult> BrowseClasses(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery(Name = "Page_Size")] int pageSize = 20,
            [FromQuery(Name = "Page")] int page = 1)
        {
            var from = fromDate ?? DateTime.UtcNow.Date;
            var to = toDate ?? from.AddDays(14);

            var result = await classService.BrowseClassesAsync(from, to, page, pageSize);
            return Ok(result);
        }

        // TODO: memberUserId should come from the authenticated user's context once auth is wired in.
        [HttpPost("book")]
        public async Task<IActionResult> BookClass([FromQuery] int memberUserId, BookClassDto dto)
        {
            var result = await classService.BookClassAsync(memberUserId, dto);
            return Ok(result);
        }

        [HttpPatch("bookings/{bookingId}/cancel")]
        public async Task<IActionResult> CancelBooking([FromQuery] int memberUserId, int bookingId)
        {
            var result = await classService.CancelBookingAsync(memberUserId, bookingId);
            if (!result) return BadRequest();

            return Ok(new { Message = "Booking cancelled." });
        }

        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings([FromQuery] int memberUserId)
        {
            var result = await classService.GetMemberBookingsAsync(memberUserId);
            return Ok(result);
        }
    }
}
