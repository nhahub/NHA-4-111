using FitCore.BLL.Services.payment;
using FitCore.Shared.DTOs.Subscriptions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FitCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionPaymentService _subscriptionService;

        public SubscriptionController(SubscriptionPaymentService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost("create-with-invoice")]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionDto dto)
        {
            var result = await _subscriptionService.CreateSubscriptionWithInvoiceAsync(dto);
            if (result)
                return Ok(new { message = "Subscription and invoice created successfully!" });

            return BadRequest(new { message = "Failed to create subscription." });
        }

        [HttpPost("pay-invoice")]
        public async Task<IActionResult> PayInvoice([FromBody] PaymentDto dto)
        {
            var result = await _subscriptionService.PayInvoiceAsync(dto);
            if (result)
                return Ok(new { message = "Payment recorded successfully!" });

            return BadRequest(new { message = "Payment failed. Please verify the invoice number." });
        }
    }
}