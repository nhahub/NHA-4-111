using FitCore.DAL.Data.Contexts;
using FitCore.DAL.Data.Models;
using FitCore.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FitCore.BLL.Services
{
    public class CheckoutService
    {
        private readonly FitCoreDbContext _context;

        public CheckoutService(FitCoreDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ProcessCheckoutAsync(int userId, int? memberProfileId = null, int? gymServiceId = null)
        {
            var cart = await _context.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            bool hasCartItems = cart != null && cart.CartItems.Any();
            bool hasSubscription = memberProfileId.HasValue && gymServiceId.HasValue;

            if (!hasCartItems && !hasSubscription)
                return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoice = new Invoice
                {
                    UserID = userId,
                    IssueDate = DateTime.UtcNow,
                    TotalAmount = 0,
                    InvoiceStatus = InvoiceStatus.Pending,
                    Description = "Checkout Invoice"
                };

                await _context.Invoices.AddAsync(invoice);
                await _context.SaveChangesAsync();

                decimal totalAmount = 0;

                if (hasCartItems)
                {
                    foreach (var cartItem in cart.CartItems)
                    {
                        var invoiceItem = new InvoiceItem
                        {
                            InvoiceID = invoice.InvoiceID,
                            ItemType = InvoiceItemType.Product,
                            ProductID = cartItem.ProductID,
                            ItemName = "Product",
                            Quantity = cartItem.Quantity,
                            SellPrice = cartItem.Product.CurrentSellPrice,
                            LineTotal = cartItem.Product.CurrentSellPrice * cartItem.Quantity
                        };

                        totalAmount += invoiceItem.LineTotal;
                        await _context.InvoiceItems.AddAsync(invoiceItem);
                    }
                    _context.CartItem.RemoveRange(cart.CartItems);
                }

                if (hasSubscription)
                {
                    var invoiceItem = new InvoiceItem
                    {
                        InvoiceID = invoice.InvoiceID,
                        ItemType = InvoiceItemType.MembershipPlan,
                        ServiceID = gymServiceId.Value,
                        ItemName = "Gym Subscription",
                        Quantity = 1,
                        SellPrice = 0,
                        LineTotal = 0
                    };

                    totalAmount += invoiceItem.LineTotal;
                    await _context.InvoiceItems.AddAsync(invoiceItem);

                    var membership = new Membership
                    {
                        MemberProfileId = memberProfileId.Value,
                        GymServiceId = gymServiceId.Value,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(1),
                        Status = MemberShipStatus.Active,
                        IsAutoRenew = false,
                        InvoiceID = invoice.InvoiceID
                    };
                    await _context.Memberships.AddAsync(membership);
                }

                invoice.TotalAmount = totalAmount;
                _context.Invoices.Update(invoice);

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
    }
}