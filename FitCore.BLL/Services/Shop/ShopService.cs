using FitCore.BLL.Exceptions;
using FitCore.BLL.Interfaces;
using FitCore.BLL.Interfaces.IShopService;
using FitCore.DAL.Data.Contexts;
using FitCore.DAL.Data.Models;
using FitCore.Shared.DTOs;
using FitCore.Shared.DTOs.Cart;
using FitCore.Shared.DTOs.Products;
using FitCore.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace FitCore.BLL.Services
{
    public class ShopService(FitCoreDbContext DbContext) : IShopService
    {
        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            return await DbContext.Products
                .Where(p => !p.IsDeleted)
                .Select(p => new ProductDTO
                {
                    ProductID = p.ProductID,
                    Name = p.Name,
                    Description = p.Description,
                    CurrentSellPrice = p.CurrentSellPrice,
                    ImageUrl = p.ImageUrl
                }).ToListAsync();
        }

        public async Task<bool> AddToCartAsync(int userId, AddCartItemDTO cartItemDto)
        {
            // 1. فحص قيمة الـ userId
            if (userId <= 0) throw new BusinessRuleException("معرف المستخدم غير صالح (الرجاء تسجيل الدخول).");

            // 2. فحص وجود المنتج
            var totalAvailable = await DbContext.Set<Inventory>()
                .Where(i => i.ProductId == cartItemDto.ProductID && !i.IsDeleted)
                .SumAsync(i => i.Quantity);

            if (totalAvailable < cartItemDto.Quantity)
                throw new BusinessRuleException("الكمية المطلوبة غير متوفرة في المخزن حالياً.");

            // 3. البحث عن السلة
            var cart = await DbContext.Set<Cart>()
                .FirstOrDefaultAsync(c => c.UserID == userId && !c.IsDeleted);

            if (cart == null)
            {
                cart = new Cart { UserID = userId };
                await DbContext.Set<Cart>().AddAsync(cart);
                await DbContext.SaveChangesAsync(); 
            } 

            // 3. التحقق إذا كان المنتج موجوداً بالفعل في السلة لزيادة الكمية فقط
            var existingItem = await DbContext.Set<CartItem>()
                .FirstOrDefaultAsync(ci => ci.CartID == cart.CartID && ci.ProductID == cartItemDto.ProductID && !ci.IsDeleted);

            if (existingItem != null)
            {
                existingItem.Quantity += cartItemDto.Quantity;
                DbContext.Set<CartItem>().Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartID = cart.CartID,
                    ProductID = cartItemDto.ProductID,
                    Quantity = cartItemDto.Quantity
                };
                await DbContext.Set<CartItem>().AddAsync(cartItem);
            }

            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<CartItemDTO>> GetUserCartAsync(int userId)
        {
            return await DbContext.Set<CartItem>()
                .Include(ci => ci.Product)
                .Where(ci => ci.Cart.UserID == userId && !ci.IsDeleted)
                .Select(ci => new CartItemDTO
                {
                    CartItemID = ci.CartItemID,
                    ProductID = ci.ProductID,
                    ProductName = ci.Product.Name,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.CurrentSellPrice,
                    ImageUrl = ci.Product.ImageUrl 
                }).ToListAsync();
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId, int userId)
        {
            var item = await DbContext.Set<CartItem>()
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemID == cartItemId && ci.Cart.UserID == userId && !ci.IsDeleted);

            if (item == null) return false;

            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;

            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity, int userId)
        {
            var item = await DbContext.Set<CartItem>()
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemID == cartItemId && ci.Cart.UserID == userId && !ci.IsDeleted);

            if (item == null || quantity <= 0) return false;

            item.Quantity = quantity;
            return await DbContext.SaveChangesAsync() > 0;
        }

        public async Task<int> CheckoutAsync(int userId, CheckoutDTO checkoutDto)
        {
            // 1. جلب السلة
            var cart = await DbContext.Set<Cart>()
                .Include(c => c.CartItems).ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId && !c.IsDeleted);

            if (cart == null || !cart.CartItems.Any()) throw new BusinessRuleException("السلة فارغة");

            // 2. إنشاء الفاتورة
            var invoice = new Invoice
            {
                UserID = userId,
                IssueDate = DateTime.UtcNow,
                InvoiceStatus = InvoiceStatus.Pending, // تأكد من الـ Enum عندك
                Description = checkoutDto.Description,
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.CurrentSellPrice)
            };

            await DbContext.Set<Invoice>().AddAsync(invoice);
            await DbContext.SaveChangesAsync(); // عشان ناخد الـ InvoiceID

            // 3. تحويل عناصر السلة لفاتورة
            foreach (var item in cart.CartItems)
            {
                var invoiceItem = new InvoiceItem
                {
                    InvoiceID = invoice.InvoiceID,
                    ProductID = item.ProductID,
                    ItemName = item.Product.Name,
                    Quantity = item.Quantity,
                    SellPrice = item.Product.CurrentSellPrice,
                    LineTotal = item.Quantity * item.Product.CurrentSellPrice,
                    ItemType = InvoiceItemType.Product // حدد النوع المناسب
                };
                await DbContext.Set<InvoiceItem>().AddAsync(invoiceItem);

                // 4. عمل Soft Delete لعناصر السلة بعد التحويل
                item.IsDeleted = true;
            }

            await DbContext.SaveChangesAsync();
            return invoice.InvoiceID; // نرجع رقم الفاتورة للفرونت إيند
        }
    }
}