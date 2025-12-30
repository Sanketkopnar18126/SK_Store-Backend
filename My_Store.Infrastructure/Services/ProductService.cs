using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using My_Store.Application.DTOs.Product;
using My_Store.Application.Interfaces;
using My_Store.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
    
namespace My_Store.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IUnitOfWork uow,
            IMapper mapper,
            Cloudinary cloudinary,
            ILogger<ProductService> logger)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cloudinary = cloudinary ?? throw new ArgumentNullException(nameof(cloudinary));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto, int adminId, CancellationToken ct = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var product = new Product(dto.Name, dto.Description, dto.Price, dto.Stock, dto.Category, adminId);

            if (dto.ImageUrls != null && dto.ImageUrls.Length > 0)
            {
                product.SetImages(dto.ImageUrls);
            }

            await _uow.Products.AddAsync(product, ct);
            await _uow.CommitAsync(ct);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var product = await _uow.Products.GetByIdAsync(id, ct);
            if (product == null) return false;

            await _uow.Products.DeleteAsync(product);
            await _uow.CommitAsync(ct);

            return true;
        }

        public async Task<object> GetAllAsync(bool groupByCategory = false, CancellationToken ct = default)
        {
            var products = await _uow.Products.GetAsync(asNoTracking: true, ct: ct);
            var mappedProducts = _mapper.Map<IEnumerable<ProductForDisplayDto>>(products);

            if (groupByCategory)
            {
                return mappedProducts
                    .GroupBy(p => p.Category)
                    .ToDictionary(g => g.Key ?? "Uncategorized", g => g.ToList());
            }

            return mappedProducts;
        }

        public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var product = await _uow.Products.GetByIdAsync(id, ct);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, int adminId, CancellationToken ct = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var product = await _uow.Products.GetByIdAsync(id, ct);
            if (product == null) return null;

            product.Update(dto.Name, dto.Description, dto.Price, dto.Stock, dto.Category, adminId);

            if (dto.ImageUrls != null && dto.ImageUrls.Length > 0)
            {
                product.SetImages(dto.ImageUrls);
            }

            await _uow.Products.UpdateAsync(product);
            await _uow.CommitAsync(ct);

            return _mapper.Map<ProductDto>(product);
        }


        public async Task<IEnumerable<ProductDto>> GetSimilarProductsAsync( int productId,int limit = 6,CancellationToken ct = default)
        {
            var product = await _uow.Products.GetByIdAsync(productId, ct);
            if (product == null)
                return Enumerable.Empty<ProductDto>();

          
            var similarProducts = await _uow.Products.GetAsync(
                predicate: p =>
                    p.Category == product.Category &&  
                    p.Id != productId ,             
                    orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                    take: limit,
                    asNoTracking: true,
                    ct: ct
            );

            return _mapper.Map<IEnumerable<ProductDto>>(similarProducts);
        }

        public async Task<IDictionary<string, List<ProductDto>>>GetRecommendedProductsAsync(int limitPerCategory = 4,CancellationToken ct = default)
        {
            var products = await _uow.Products.GetAsync(
                predicate: p => p.Stock > 0,
                orderBy: q => q.OrderByDescending(x => x.CreatedAt),
                asNoTracking: true,
                ct: ct
            );

            var grouped = products
                .GroupBy(p => p.Category ?? "Others")
                .ToDictionary(
                    g => g.Key,
                    g => _mapper.Map<List<ProductDto>>(
                        g.Take(limitPerCategory).ToList()
                    )
                );

            return grouped;
        }


        public async Task<IEnumerable<string>> UploadImagesAsync(IEnumerable<ProductImageUploadDto> files, CancellationToken ct = default)
        {
            if (files == null) return Enumerable.Empty<string>();

            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                if (file == null) continue;

                try
                {
                    using var ms = new MemoryStream(file.Content ?? Array.Empty<byte>());
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName ?? $"upload-{Guid.NewGuid()}", ms),
                        Folder = "products"
                    };

                    // If your Cloudinary SDK version supports a CancellationToken overload, replace this call accordingly.
                    var result = await _cloudinary.UploadAsync(uploadParams);

                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        imageUrls.Add(result.SecureUrl.ToString());
                    }
                    else
                    {
                        _logger.LogError("Cloudinary upload failed for file {FileName}: {Error}", file.FileName, result.Error?.Message);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Image upload cancelled for {FileName}", file?.FileName);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error uploading file {FileName}", file?.FileName);
                }
            }

            return imageUrls;
        }
    }
}
