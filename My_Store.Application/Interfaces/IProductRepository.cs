using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My_Store.Domain.Entities;

namespace My_Store.Application.Interfaces
{
    public interface IProductRepository:IGenericRepository<Product>
    {
        Task<Product?> GetWithImagesAsync(int id, CancellationToken ct = default);
        IQueryable<Product> QueryProducts(bool asNoTracking = true);
    }
}
