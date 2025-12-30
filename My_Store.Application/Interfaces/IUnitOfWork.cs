using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace My_Store.Application.Interfaces
{
    public interface IUnitOfWork:IDisposable
    {
        IProductRepository Products { get; }
        ICartRepository Carts { get; }
        IUserRepository Users { get; }
        Task<int> CommitAsync(CancellationToken ct = default);
   
    }
}
