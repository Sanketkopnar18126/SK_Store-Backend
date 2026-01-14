using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My_Store.Application.DTOs.Order;

namespace My_Store.Application.Interfaces
{
    public interface IOrderService
    {
        Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request,CancellationToken ct);
        Task<bool> UpdateOrderStatusAsync(Guid orderId, string status, CancellationToken ct = default);
    }
}
