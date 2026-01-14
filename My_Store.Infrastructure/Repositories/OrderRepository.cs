using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using My_Store.Application.Interfaces;
using My_Store.Domain.Entities;
using My_Store.Infrastructure.Persistence;

namespace My_Store.Infrastructure.Repositories
{
    public class OrderRepository:GenericRepository<Order>,IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context) { }

        public async Task<Order?> GetOrderWithItemsAsync(Guid orderId, CancellationToken ct)
        {
            return await _dbSet
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId, ct);
        }
    }
}
