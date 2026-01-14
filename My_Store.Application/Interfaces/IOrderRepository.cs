using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My_Store.Application.DTOs.Order;
using My_Store.Domain.Entities;

namespace My_Store.Application.Interfaces
{
    public interface IOrderRepository: IGenericRepository<Order>
    {
        Task<Order?> GetOrderWithItemsAsync(Guid orderId, CancellationToken ct);
    }
}
