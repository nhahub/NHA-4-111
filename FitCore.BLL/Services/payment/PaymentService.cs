using FitCore.BLL.DTOs.Payment;
using FitCore.BLL.Interfaces.Membership;
using FitCore.BLL.Interfaces.Payment;
using FitCore.DAL.Data.Contexts;
using FitCore.DAL.Data.Models;
using FitCore.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;
using PaymentMethod = FitCore.Shared.Enums.PaymentMethod;


namespace FitCore.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly FitCoreDbContext _context;
        private readonly IMembershipService _membershipService;
        private readonly string _webhookSecret;

        public PaymentService(FitCoreDbContext context, IMembershipService membershipService, IConfiguration configuration)
        {
            _context = context;
            _membershipService = membershipService;
            _webhookSecret = configuration["Stripe:WebhookSecret"]
                ?? throw new InvalidOperationException("Stripe:WebhookSecret is missing from configuration.");
        }

        public async Task<CheckoutSessionResponseDto> CreateCheckoutSessionAsync(int invoiceId, string successUrl, string cancelUrl)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.InvoiceID == invoiceId);

            if (invoice == null)
                throw new InvalidOperationException("Invoice not found.");

            if (invoice.InvoiceStatus == InvoiceStatus.Completed)
                throw new InvalidOperationException("Invoice is already paid.");

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in invoice.InvoiceItems)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd", // adjust to your currency
                        UnitAmount = (long)(item.SellPrice * 100), // Stripe uses the smallest currency unit (cents)
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ItemName
                        }
                    },
                    Quantity = item.Quantity
                });
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = successUrl + "?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = cancelUrl,
                Metadata = new Dictionary<string, string>
                {
                    { "InvoiceID", invoice.InvoiceID.ToString() },
                    { "UserID", invoice.UserID.ToString() }
                }
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return new CheckoutSessionResponseDto
            {
                SessionId = session.Id,
                SessionUrl = session.Url
            };
        }

        public async Task HandleStripeWebhookAsync(string json, string stripeSignatureHeader)
        {
            // Throws StripeException if the signature is invalid - let the controller catch it and return 400.
            // This check is what prevents anyone from forging a "payment succeeded" request.
            Event stripeEvent = EventUtility.ConstructEvent(json, stripeSignatureHeader, _webhookSecret);

            if (stripeEvent.Type == "checkout.session.completed")
            {
                if (stripeEvent.Data.Object is Session session)
                {
                    await ConfirmPaymentAsync(session);
                }
            }
        }

        private async Task ConfirmPaymentAsync(Session session)
        {
            if (!session.Metadata.TryGetValue("InvoiceID", out var invoiceIdStr) ||
                !int.TryParse(invoiceIdStr, out var invoiceId))
                return;

            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceID == invoiceId);

            // Missing invoice, or already processed (Stripe can send the same webhook more than once) - skip.
            if (invoice == null || invoice.InvoiceStatus == InvoiceStatus.Completed)
                return;

            var payment = new Payment
            {
                InvoiceID = invoice.InvoiceID,
                UserId = invoice.UserID,
                AmountPaid = (session.AmountTotal ?? 0) / 100m,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = PaymentMethod.Card,
                TransactionReference = session.PaymentIntentId ?? session.Id,
                GatewayResponse = session.PaymentStatus
            };

            await _context.Payments.AddAsync(payment);

            invoice.InvoiceStatus = InvoiceStatus.Completed;
            _context.Invoices.Update(invoice);

            await _context.SaveChangesAsync();

            // Memberships are only generated once payment is actually confirmed
            await _membershipService.GenerateMembershipsFromInvoiceAsync(invoice.InvoiceID);
        }
    }
}