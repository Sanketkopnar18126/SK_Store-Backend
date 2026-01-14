using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using My_Store.Application.Common.Settings;
using My_Store.Application.Interfaces;
using Razorpay.Api;

namespace My_Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentWebhookController : ControllerBase
    {
        private readonly RazorpaySettings _settings;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<PaymentWebhookController> _logger;

        public PaymentWebhookController(
            IOptions<RazorpaySettings> settings,
            IUnitOfWork uow,
            ILogger<PaymentWebhookController> logger)
        {
            _settings = settings.Value;
            _uow = uow;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> HandleWebhook()
        {
            string body;

            try
            {
                // 1️⃣ Read raw request body
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                body = await reader.ReadToEndAsync();
                _logger.LogInformation("RAW WEBHOOK PAYLOAD: {Payload}", body);

                // 2️⃣ Read Razorpay signature header
                var signature = Request.Headers["X-Razorpay-Signature"].ToString();
                if (string.IsNullOrEmpty(signature))
                {
                    _logger.LogWarning("Webhook received without signature");
                    return BadRequest();
                }

                // 3️⃣ Verify signature
                Utils.verifyWebhookSignature(body, signature, _settings.WebhookSecret);

                // 4️⃣ Deserialize webhook payload
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(body)!;
                var paymentEntity = payload.payload.payment.entity;

                string razorpayPaymentId = paymentEntity.id;
                string status = paymentEntity.status; // authorized / captured / failed
                string? appOrderId = paymentEntity.notes?.appOrderId != null
                    ? paymentEntity.notes.appOrderId.ToString()
                    : null;

                _logger.LogInformation("Webhook received | PaymentId: {PaymentId}, AppOrderId: {AppOrderId}, Status: {Status}",
                    razorpayPaymentId,
                    appOrderId,
                    status
                );

                // 5️⃣ Find payment using appOrderId
                if (string.IsNullOrEmpty(appOrderId))
                {
                    _logger.LogWarning(
                        "Webhook does not contain appOrderId in notes. PaymentId: {PaymentId}",
                        razorpayPaymentId
                    );
                    return Ok(); // prevent Razorpay retry
                }

                var payment = await _uow.Payments.GetByAppOrderIdAsync(Guid.Parse(appOrderId), CancellationToken.None);

                if (payment == null)
                {
                    _logger.LogWarning(
                        "Payment record not found for AppOrderId: {AppOrderId}, PaymentId: {PaymentId}",
                        appOrderId,
                        razorpayPaymentId
                    );
                    return Ok(); // Always return 200 to Razorpay
                }


                // 6️⃣ Update payment info
                payment.RazorpayPaymentId = razorpayPaymentId;
                payment.IsPaid = status == "captured"; // only true if captured
                payment.Status = status;
                payment.RazorpaySignature = signature;

                await _uow.Payments.UpdateAsync(payment);

                // 7️⃣ Update associated app order
                var order = await _uow.Orders.GetByIdAsync(payment.OrderId, CancellationToken.None);
                if (order != null)
                {
                    order.Status = payment.IsPaid
                        ? Domain.Enums.OrderStatus.Paid
                        : (status == "failed"
                            ? Domain.Enums.OrderStatus.PaymentFailed
                            : order.Status); // keep existing status if authorized

                    await _uow.Orders.UpdateAsync(order);
                }

                // 8️⃣ Save all changes
                await _uow.CommitAsync();

                _logger.LogInformation(
                    "Payment processed successfully | PaymentId: {PaymentId}, AppOrderId: {AppOrderId}",
                    razorpayPaymentId,
                    appOrderId
                );

                return Ok(); // Razorpay requires 200
            }
            catch (Razorpay.Api.Errors.SignatureVerificationError ex)
            {
                _logger.LogError(ex, "Invalid Razorpay webhook signature");
                return BadRequest();
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                _logger.LogError(ex, "Invalid webhook JSON");
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled webhook error");
                return StatusCode(500);
            }
        }
    }
}
