using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My_Store.Domain.Entities;

namespace My_Store.Application.Interfaces
{
    public interface IBannerRepository:IGenericRepository<Banner>
    {
        Task<IReadOnlyList<Banner>> GetAllAsync(CancellationToken ct = default);
    }
}
