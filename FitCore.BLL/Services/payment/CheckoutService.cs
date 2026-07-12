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

        public async Task<bool> ProcessCheckoutAsync(int userId)
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
                return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoice = new Invoice
                {
                    UserID = userId,
                    IssueDate = DateTime.UtcNow,
                    TotalAmount = 0,
                    InvoiceStatus = InvoiceStatus.Completed,
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
                            invoiceItem.SellPrice = 0;
                            invoiceItem.LineTotal = 0;
                        }

                        totalAmount += invoiceItem.LineTotal;
                        await _context.InvoiceItems.AddAsync(invoiceItem);

                        booking.Status = BookingStatus.Paid;
                        booking.IsDeleted = true;
                        booking.DeletedAt = DateTime.UtcNow;
                    }

                    _context.Set<Booking>().UpdateRange(pendingBookings);
                }

                // 6. تحديث إجمالي الفاتورة
                invoice.TotalAmount = totalAmount;
                _context.Invoices.Update(invoice);
                await _context.SaveChangesAsync();

                // 7. السحر بقى: إنشاء الاشتراكات (Generate Memberships)
                // بننادي الدالة اللي إنتي كتبتيها جوه الـ Transaction عشان لو ضربت، كل حاجة ترجع (Rollback)
               // await GenerateMembershipsFromInvoiceAsync(invoice.InvoiceID);

                // 8. تأكيد العملية
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}