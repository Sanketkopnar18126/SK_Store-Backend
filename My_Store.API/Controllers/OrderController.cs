using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using My_Store.Application.DTOs.Order;
using My_Store.Application.DTOs.Payment;
using My_Store.Application.Interfaces;

namespace My_Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;

        public OrderController(IOrderService orderService, IPaymentService paymentService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
        }
        [Authorize]
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CreateOrderRequestDto dto, CancellationToken ct)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized();

            var userPublicId = Guid.Parse(userIdClaim);

            dto.UserId = Guid.Parse(userIdClaim);
            // 1️⃣ Create Order with PaymentPending
            var orderResponse = await _orderService.CreateOrderAsync(dto, ct);

            // 2️⃣ Create Razorpay Payment Order linked to Order
            var paymentRequest = new My_Store.Application.DTOs.Payment.CreatePaymentOrderRequestDto
            {
                Amount = orderResponse.TotalAmount,
                Currency = "INR",
                OrderId = orderResponse.OrderId // Link Order → Payment
            };

            var paymentResponse = await _paymentService.CreateOrderAsync(paymentRequest, ct);

            // Return both Order + Payment info to frontend
            return Ok(new
            {
                Order = orderResponse,
                Payment = paymentResponse
            });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequestDto dto, CancellationToken ct)
        {
            var isValid = await _paymentService.VerifyPaymentAsync(dto, ct);

            if (!isValid)
                return BadRequest("Payment verification failed");

            return Ok(new { success = true });
        }
    }
}
