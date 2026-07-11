using FitCore.BLL.Exceptions;
using FitCore.BLL.Interfaces.GymService;
using FitCore.DAL.Data.Models;
using FitCore.Shared.DTOs.GymService;
using FitCore.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FitCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymServicesController(IGymServiceService _gymService) : ControllerBase
    {

        private const int HardcodedMemberUserId = 1;

        [HttpPost]
        public async Task<IActionResult> CreateGymService([FromBody] CreateGymServiceDto dto)
        {
            try
            {
                var result = await _gymService.CreateGymServiceAsync(dto);
                return CreatedAtAction(nameof(GetGymServices), new { id = result.ServiceID }, result);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGymService(int id, [FromBody] UpdateGymServiceDto dto)
        {
            try
            {
                var result = await _gymService.UpdateGymServiceAsync(id, dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGymService(int id)
        {
            try
            {
                await _gymService.DeleteGymServiceAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetGymServices(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? searchTerm = null,
            [FromQuery] ServiceCategory? category = null)
        {
            var result = await _gymService.GetGymServicesAsync(page, pageSize, searchTerm, category);
            return Ok(result);
        }

        [HttpDelete("bookings/{bookingId}/cancel")]
        public async Task<IActionResult> CancelGymServiceBooking(int bookingId)
        {
            try
            {
                await _gymService.CancelGymServiceBookingAsync(HardcodedMemberUserId, bookingId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("bookings/checkout-cleanup")]
        public async Task<IActionResult> RemoveBookingsAfterCheckout([FromBody] List<int> bookingIds)
        {
            try
            {
                await _gymService.RemoveBookingsAfterCheckoutAsync(HardcodedMemberUserId, bookingIds);
                return Ok(new { Message = "Bookings successfully processed after checkout." });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}