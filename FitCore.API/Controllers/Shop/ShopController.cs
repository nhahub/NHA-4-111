using FitCore.BLL.Interfaces.IShopService;
using FitCore.DAL.Data.Models;
using FitCore.Shared.DTOs.Cart;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class ShopController(IShopService shopService) : ControllerBase
{
    private int GetUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "1");

    [HttpGet("products")]
    public async Task<IActionResult> GetProducts() => Ok(await shopService.GetAllProductsAsync());

    [HttpPost("cart")]
    public async Task<IActionResult> AddToCart([FromBody] AddCartItemDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // هذا سيكشف لك لو فيه خطأ في البيانات المرسلة
        }

        var userId = GetUserId();
        var result = await shopService.AddToCartAsync(userId, dto);
        return result ? Ok("تمت الإضافة للسلة بنجاح") : BadRequest("فشلت عملية الإضافة");
    }

    [HttpGet("cart")]
    public async Task<IActionResult> GetCart() => Ok(await shopService.GetUserCartAsync(GetUserId()));

    [HttpDelete("cart/{cartItemId}")]
    public async Task<IActionResult> RemoveFromCart(int cartItemId)
    {
        var result = await shopService.RemoveFromCartAsync(cartItemId, GetUserId());
        return result ? Ok("تم الحذف من السلة") : NotFound("العنصر غير موجود");
    }

    [HttpPatch("cart/{cartItemId}")]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromBody] int quantity)
    {
        var result = await shopService.UpdateCartItemQuantityAsync(cartItemId, quantity, GetUserId());
        return result ? Ok() : BadRequest();
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDTO dto)
    {
        var invoiceId = await shopService.CheckoutAsync(GetUserId(), dto);
        return Ok(new { InvoiceId = invoiceId, Message = "تمت عملية الشراء بنجاح" });
    }
}