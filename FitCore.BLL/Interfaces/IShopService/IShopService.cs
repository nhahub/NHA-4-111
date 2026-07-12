using FitCore.Shared.DTOs.Cart;
using FitCore.Shared.DTOs.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitCore.BLL.Interfaces.IShopService
{
    public interface IShopService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<bool> AddToCartAsync(int userId, AddCartItemDTO cartItemDto);
        Task<IEnumerable<CartItemDTO>> GetUserCartAsync(int userId);
        Task<bool> RemoveFromCartAsync(int cartItemId, int userId);
        Task<bool> UpdateCartItemQuantityAsync(int cartItemId, int quantity, int userId);
        Task<int> CheckoutAsync(int userId, CheckoutDTO checkoutDto);
    }
}
