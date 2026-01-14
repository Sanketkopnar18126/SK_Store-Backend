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
    public class PaymentRepository:GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Payment?> GetByRazorpayOrderIdAsync(string razorpayOrderId,CancellationToken ct)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.RazorpayOrderId == razorpayOrderId, ct);
        }

        public async Task<Payment?> GetByAppOrderIdAsync(Guid appOrderId, CancellationToken ct = default)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == appOrderId, ct);
        }
    }
}
