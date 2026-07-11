using FitCore.BLL.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FitCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly CheckoutService _checkoutService;

        public CheckoutController(CheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost("process/{userId}")]
        public async Task<IActionResult> ProcessCheckout(int userId, [FromQuery] int? memberProfileId, [FromQuery] int? gymServiceId)
        {
            var result = await _checkoutService.ProcessCheckoutAsync(userId, memberProfileId, gymServiceId);
            if (result)
                return Ok(new { message = "Payment processed and invoice created successfully!" });

            return BadRequest(new { message = "Operation failed. The cart may be empty or the provided data is invalid." });
        }
    }
}