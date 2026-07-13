using FitCore.BLL.Exceptions;
using FitCore.BLL.Interfaces.Auth;
using FitCore.BLL.Interfaces.GymService;
using FitCore.DAL.Data.Models;
using FitCore.Shared.DTOs.GymService;
using FitCore.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymServicesController(IGymServiceService _gymService,ICurrentUserService _currentUser) : ControllerBase
    {

        [HttpPost("bookings")]
        [Authorize]
        public async Task<IActionResult> AddGymServiceToBooking([FromQuery] int gymServiceId)
        {
            int userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();
            try
            {
                var result = await _gymService.AddGymServiceToBookingAsync(userId, gymServiceId);
                return Ok(result);
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

        [HttpPost]
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> CancelGymServiceBooking(int bookingId)
        {
            int userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

            try
            {
                await _gymService.CancelGymServiceBookingAsync(userId, bookingId);
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

        //[HttpPost("bookings/checkout-cleanup")]
        //public async Task<IActionResult> RemoveBookingsAfterCheckout([FromBody] List<int> bookingIds)
        //{
        //    try
        //    {
        //        await _gymService.RemoveBookingsAfterCheckoutAsync(HardcodedMemberUserId, bookingIds);
        //        return Ok(new { Message = "Bookings successfully processed after checkout." });
        //    }
        //    catch (ValidationException ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //    catch (BusinessRuleException ex)
        //    {
        //        return BadRequest(new { Message = ex.Message });
        //    }
        //}
    }
}