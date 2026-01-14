using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using My_Store.Application.DTOs.Payment;
using My_Store.Application.Interfaces;

namespace My_Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController( IPaymentService paymentService,ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] CreatePaymentOrderRequestDto dto,CancellationToken ct)
        {
            _logger.LogInformation("Create payment order requested");
            var result = await _paymentService.CreateOrderAsync(dto, ct);
            return Ok(result);
        }
    }
}
