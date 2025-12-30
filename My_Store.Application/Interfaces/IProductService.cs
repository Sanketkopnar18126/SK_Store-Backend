using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My_Store.Application.DTOs.Product;
using My_Store.Domain.Entities;


namespace My_Store.Application.Interfaces
{
    public interface IProductService
    {
        Task<ProductDto> CreateAsync(CreateProductDto dto, int adminId, CancellationToken ct = default);
        Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<object> GetAllAsync(bool groupByCategory = false, CancellationToken ct = default);
        Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, int adminId, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<IEnumerable<string>> UploadImagesAsync(IEnumerable<ProductImageUploadDto> files, CancellationToken ct = default);
        Task<IEnumerable<ProductDto>> GetSimilarProductsAsync(int productId, int limit = 6,CancellationToken ct = default);
        Task<IDictionary<string, List<ProductDto>>> GetRecommendedProductsAsync(int limit =8,CancellationToken ct = default);

    }
}
