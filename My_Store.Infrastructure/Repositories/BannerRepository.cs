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
    public class BannerRepository:GenericRepository<Banner>, IBannerRepository
    {
        public BannerRepository(AppDbContext context):base(context) { }

        public async Task<IReadOnlyList<Banner>> GetAllAsync(CancellationToken ct = default)
        {
            return await _dbSet
           .AsNoTracking()
           .OrderByDescending(b => b.CreatedAt)
           .ToListAsync(ct);
        }
    }
}
