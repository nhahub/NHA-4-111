using FitCore.BLL.Interfaces.Membership;
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
        private readonly IMembershipService _membershipService;

        public CheckoutService(FitCoreDbContext context, IMembershipService membershipService)
        {
            _context = context;
            _membershipService = membershipService;
        }

        public async Task<int?> ProcessCheckoutAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId);

            bool hasCartItems = cart != null && cart.CartItems.Any();


            var pendingBookings = await _context.Set<Booking>()
                .Include(b => b.GymService)
                .Include(b => b.Class)
                .Where(b => b.MemberUserId == userId && b.Status == BookingStatus.Booked)
                .ToListAsync();

            bool hasBookings = pendingBookings.Any();

            if (!hasCartItems && !hasBookings)
                return null;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoice = new Invoice
                {
                    UserID = userId,
                    IssueDate = DateTime.UtcNow,
                    TotalAmount = 0,
                    InvoiceStatus = InvoiceStatus.Pending,
                    Description = "Unified Checkout Invoice (Products, Services, Classes)"
                };
                await _context.Invoices.AddAsync(invoice);
                await _context.SaveChangesAsync();

                decimal totalAmount = 0;

                if (hasCartItems)
                {
                    foreach (var cartItem in cart!.CartItems)
                    {
                        var invoiceItem = new InvoiceItem
                        {
                            InvoiceID = invoice.InvoiceID,
                            ItemType = InvoiceItemType.Product,
                            ProductID = cartItem.ProductID,
                            ItemName = cartItem.Product.Name ?? "Product",
                            Quantity = cartItem.Quantity,
                            SellPrice = cartItem.Product.CurrentSellPrice,
                            LineTotal = cartItem.Product.CurrentSellPrice * cartItem.Quantity
                        };

                        totalAmount += invoiceItem.LineTotal;
                        await _context.InvoiceItems.AddAsync(invoiceItem);
                    }
                    _context.CartItems.RemoveRange(cart.CartItems);
                }

                if (hasBookings)
                {
                    foreach (var booking in pendingBookings)
                    {
                        var invoiceItem = new InvoiceItem
                        {
                            InvoiceID = invoice.InvoiceID,
                            Quantity = 1,
                        };

                        if (booking.GymServiceId.HasValue)
                        {
                            invoiceItem.ItemType = InvoiceItemType.GymService;
                            invoiceItem.ServiceID = booking.GymServiceId;
                            invoiceItem.ItemName = booking.GymService!.Name;
                            invoiceItem.SellPrice = booking.GymService.Price;
                            invoiceItem.LineTotal = booking.GymService.Price;
                        }
                        else if (booking.ClassID.HasValue)
                        {
                            invoiceItem.ItemType = InvoiceItemType.Class;
                            invoiceItem.ClassID = booking.ClassID;
                            invoiceItem.ItemName = booking.Class!.ClassName;
                            invoiceItem.SellPrice = booking.Class.Price;
                            invoiceItem.LineTotal = booking.Class.Price;
                        }

                        totalAmount += invoiceItem.LineTotal;
                        await _context.InvoiceItems.AddAsync(invoiceItem);

                        booking.Status = BookingStatus.Paid;
                        booking.IsDeleted = true;
                        booking.DeletedAt = DateTime.UtcNow;
                    }

                    _context.Set<Booking>().UpdateRange(pendingBookings);
                }

                invoice.TotalAmount = totalAmount;
                _context.Invoices.Update(invoice);
                await _context.SaveChangesAsync();                

                await transaction.CommitAsync();
                return invoice.InvoiceID;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return null;
            }
        }
    }
}