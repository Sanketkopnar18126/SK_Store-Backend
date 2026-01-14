using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using My_Store.Application.DTOs.Cart;
using My_Store.Application.Interfaces;

namespace My_Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService service, ILogger<CartController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCart(CancellationToken ct = default)
        {
            int userId = 1; 
            var cart = await _service.GetCartAsync(userId, ct);
            return Ok(cart);
        }
        [Authorize]
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] CreateCartItemDto dto, CancellationToken ct = default)
        {
            int userId = 1;
            var updated = await _service.AddItemAsync(userId, dto, ct);
            return Ok(updated);
        }
        [Authorize]
        [HttpPut("items")]
        public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemDto dto, CancellationToken ct = default)
        {

            int userId = 1;
            var updated = await _service.UpdateItemQuantityAsync(userId, dto, ct);
            return Ok(updated);
        }
        [Authorize]
        [HttpDelete("items/{productId:int}")]
        public async Task<IActionResult> RemoveItem(int productId, CancellationToken ct = default)
        {
            int userId = 1;
            var ok = await _service.RemoveItemAsync(userId, productId, ct);
            return ok ? NoContent() : NotFound();
        }

    }
}
