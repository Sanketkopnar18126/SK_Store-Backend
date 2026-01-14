using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My_Store.Application.DTOs.Order;
using My_Store.Application.Interfaces;
using My_Store.Domain.Entities;
using My_Store.Domain.Enums;

namespace My_Store.Infrastructure.Services
{
    public class OrderService:IOrderService
    {
        private readonly IUnitOfWork _uow;
        public OrderService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request, CancellationToken ct = default)
        {
            var order = new Order
            {
                UserId = request.UserId,
                Status = OrderStatus.PaymentPending
            };

            foreach (var item in request.Items)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                });
            }

            order.TotalAmount = order.Items.Sum(i => i.Price * i.Quantity);

            await _uow.Orders.AddAsync(order, ct);
            await _uow.CommitAsync(ct);

            return new CreateOrderResponseDto
            {
                OrderId = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status.ToString()
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string status, CancellationToken ct = default)
        {
            var order = await _uow.Orders.GetByIdAsync(orderId, ct);
            if (order == null) return false;

            if (Enum.TryParse<OrderStatus>(status, true, out var newStatus))
            {
                order.Status = newStatus;
                await _uow.Orders.UpdateAsync(order);
                await _uow.CommitAsync(ct);
                return true;
            }

            return false;
        }
    }
}
