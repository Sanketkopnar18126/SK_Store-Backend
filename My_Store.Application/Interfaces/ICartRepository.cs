using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My_Store.Domain.Entities;

namespace My_Store.Application.Interfaces
{
    public interface ICartRepository:IGenericRepository<Cart>
    {
        Task<Cart?> GetByUserIdAsync(int userId, CancellationToken ct = default);
        Task<Cart?> GetByUserIdWithItemsAsync(int userId, CancellationToken ct = default);
    }
}
