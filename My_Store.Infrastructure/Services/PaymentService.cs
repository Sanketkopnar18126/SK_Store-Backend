using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using My_Store.Application.Common.Settings;
using My_Store.Application.DTOs.Payment;
using My_Store.Application.Interfaces;
using DomainPayment = My_Store.Domain.Entities.Payment;
using Razorpay.Api;

namespace My_Store.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;
        private readonly RazorpaySettings _settings;

        public PaymentService(
            IUnitOfWork uow,
            IMapper mapper,
            ILogger<PaymentService> logger,
            IOptions<RazorpaySettings> settings)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<CreatePaymentOrderResponseDto> CreateOrderAsync(CreatePaymentOrderRequestDto request,CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation(
                    "Creating payment order. Amount: {Amount}, Currency: {Currency}",
                    request.Amount,
                    request.Currency);

                // 1️⃣ Create DB payment entry
                var payment = _mapper.Map<DomainPayment>(request);
                payment.IsPaid = false;

                await _uow.Payments.AddAsync(payment, ct);
                await _uow.CommitAsync(ct);

                // 2️⃣ Create Razorpay order
                var client = new RazorpayClient(_settings.KeyId, _settings.KeySecret);

                var options = new Dictionary<string, object>
                        {
                            { "amount", (int)(request.Amount * 100) }, // paise
                            { "currency", request.Currency },
                            { "receipt", payment.Id.ToString() },
                            { "payment_capture", 1 }
                        };

                var razorpayOrder = client.Order.Create(options);

                // 3️⃣ Update DB with Razorpay OrderId
                payment.RazorpayOrderId = razorpayOrder["id"].ToString();

                await _uow.Payments.UpdateAsync(payment);
                await _uow.CommitAsync(ct);

                _logger.LogInformation("Razorpay order created successfully. RazorpayOrderId: {OrderId}",payment.RazorpayOrderId);

                // 4️⃣ Response
                var response = _mapper.Map<CreatePaymentOrderResponseDto>(payment);
                response.Key = _settings.KeyId;

                return response;
            }
            catch (Razorpay.Api.Errors.BadRequestError ex)
            {
                // 🔥 Razorpay specific error (AUTH FAILED, BAD REQUEST, etc.)
                _logger.LogError(
                    ex,
                    "Razorpay error while creating order. Message: {Message}",
                    ex.Message);

                throw new ApplicationException(
                    "Payment gateway authentication failed. Please contact support.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Payment order creation was cancelled.");
                throw;
            }
            catch (Exception ex)
            {
               
                _logger.LogError(
                    ex,
                    "Unexpected error while creating payment order.");

                throw new ApplicationException(
                    "An unexpected error occurred while creating payment order.");
            }
        }


        public async Task<bool> VerifyPaymentAsync( VerifyPaymentRequestDto request,CancellationToken ct = default)
        {
            _logger.LogInformation("Verifying payment for RazorpayOrderId: {OrderId}",request.OrderId);

            var payment = await _uow.Payments.GetByRazorpayOrderIdAsync(request.OrderId, ct);

            if (payment == null)
            {
                _logger.LogWarning("Payment verification failed. Order not found: {OrderId}",request.OrderId);
                return false;
            }
            try
            {
                var attributes = new Dictionary<string, string>
                {
                    { "razorpay_order_id", request.OrderId },
                    { "razorpay_payment_id", request.PaymentId },
                    { "razorpay_signature", request.Signature }
                };

                Utils.verifyPaymentSignature(attributes);

                payment.IsPaid = true;
                payment.RazorpayPaymentId = request.PaymentId;
                payment.RazorpaySignature = request.Signature;

                await _uow.Payments.UpdateAsync(payment);
                await _uow.CommitAsync(ct);

                _logger.LogInformation(
                    "Payment verified successfully for OrderId: {OrderId}",
                    request.OrderId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Payment verification failed for OrderId: {OrderId}",request.OrderId);
                return false;
            }
        }
    }
}
