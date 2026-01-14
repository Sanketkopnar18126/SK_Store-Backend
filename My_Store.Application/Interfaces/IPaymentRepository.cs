using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My_Store.Domain.Entities;

namespace My_Store.Application.Interfaces
{
    public interface IPaymentRepository:IGenericRepository<Payment>
    {
        //Task<Payment?> GetByOrderIdAsync(string orderId, CancellationToken ct);
        Task<Payment?> GetByRazorpayOrderIdAsync(string razorpayOrderId, CancellationToken ct);
        // In IPaymentRepository
        Task<Payment?> GetByAppOrderIdAsync(Guid appOrderId, CancellationToken ct = default);


    }
}
