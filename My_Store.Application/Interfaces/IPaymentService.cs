using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My_Store.Application.DTOs.Payment;

namespace My_Store.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<CreatePaymentOrderResponseDto> CreateOrderAsync(CreatePaymentOrderRequestDto request, CancellationToken ct);
        Task<bool> VerifyPaymentAsync(VerifyPaymentRequestDto request, CancellationToken ct);
    }
}
