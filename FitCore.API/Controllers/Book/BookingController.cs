using FitCore.BLL.Interfaces.Book;
using FitCore.Shared.DTOs.Book;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FitCore.API.Controllers.Book
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController(IBookingService _bookingService) : ControllerBase
    {
        int memberId = 1;
        [HttpGet]
        public async Task<IActionResult> GetAllBookings([FromQuery] BookingParametersDto parameters)
        {
            var result = await _bookingService.GetAllBookingsAsync(parameters,memberId);
            return Ok(result);
        }
    }
}