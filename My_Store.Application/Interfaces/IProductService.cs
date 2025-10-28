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
        Task<object> GetAllAsync(bool groupByCategory = false);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(CreateProductDto dto, int adminId);
        Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, int adminId);
        Task<IEnumerable<string>> UploadImagesAsync(IEnumerable<ProductImageUploadDto> files);
        Task<bool> DeleteAsync(int id);

    }
}
