
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using My_Store.Domain.Entities;
using My_Store.Infrastructure.Persistence;
using My_Store.Application.Interfaces;

namespace My_Store.Infrastructure.Repositories
{
    public class CartRepository:GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context) { }

        public async Task<Cart?> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);
        }

        public async Task<Cart?> GetByUserIdWithItemsAsync(int userId, CancellationToken ct = default)
        {
            return await _dbSet
                .Include(c => c.Items)
                .ThenInclude(c=>c.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId, ct);
        }
    }
}
