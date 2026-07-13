using FitCore.DAL.Data.Contexts;
using FitCore.DAL.Data.Models;
using FitCore.Shared.Enums;
using FitCore.Shared.DTOs.Subscriptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace FitCore.BLL.Services.payment
{
    public class SubscriptionPaymentService
    {
        private readonly FitCoreDbContext _context;

        public SubscriptionPaymentService(FitCoreDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateSubscriptionWithInvoiceAsync(CreateSubscriptionDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var membership = new Membership
                {
                    MemberProfileId = dto.MemberProfileId,
                    GymServiceId = dto.GymServiceId,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(dto.DurationInDays),
                    Status = MemberShipStatus.Active,
                    IsAutoRenew = false
                };
                await _context.Memberships.AddAsync(membership);

                var invoice = new Invoice
                {
                    UserID = dto.UserId,
                    IssueDate = DateTime.UtcNow,
                    TotalAmount = dto.Price,
                    InvoiceStatus = InvoiceStatus.Pending,
                    Description = $"Subscription for {dto.ServiceName}"
                };
                await _context.Invoices.AddAsync(invoice);
                await _context.SaveChangesAsync();

                var invoiceItem = new InvoiceItem
                {
                    InvoiceID = invoice.InvoiceID,

                    //ItemType = InvoiceItemType.MembershipPlan,
                    ServiceID = dto.GymServiceId,
                    ItemName = dto.ServiceName,
                    Quantity = 1,
                    SellPrice = dto.Price,
                    LineTotal = dto.Price
                };
                await _context.InvoiceItems.AddAsync(invoiceItem);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> PayInvoiceAsync(PaymentDto dto)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.InvoiceID == dto.InvoiceId);
            if (invoice == null) return false;

            var payment = new Payment
            {
                InvoiceID = dto.InvoiceId,
                UserId = dto.UserId,
                AmountPaid = dto.Amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = dto.PaymentMethod,
                TransactionReference = dto.TransactionReference
            };

            await _context.Payments.AddAsync(payment);

            if (dto.Amount >= invoice.TotalAmount)
            {
                invoice.InvoiceStatus = InvoiceStatus.Completed;
            }

            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}