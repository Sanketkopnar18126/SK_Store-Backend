using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using My_Store.Application.DTOs.Product;
using My_Store.Application.Interfaces;
using My_Store.Domain.Entities;
using My_Store.Infrastructure.Persistence;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;

namespace My_Store.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository, IMapper mapper, Cloudinary cloudinary, ILogger<ProductService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _cloudinary = cloudinary;
            _logger = logger ;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto,int adminId)
        {
            var product = new Product(dto.Name, dto.Description, dto.Price, dto.Stock,dto.Category, adminId);

            if (dto.ImageUrls != null && dto.ImageUrls.Length > 0)
                product.SetImages(dto.ImageUrls);

            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return false;

            await _repository.DeleteAsync(product);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<object> GetAllAsync(bool groupByCategory = false)
        {
            var products = await _repository.GetAllAsync();
            var mappedProducts = _mapper.Map<IEnumerable<ProductForDisplayDto>>(products);

            if (groupByCategory)
            {
                return mappedProducts
                    .GroupBy(p => p.Category)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }

            return mappedProducts;
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, int adminId)
        {
            var product = await _repository.GetByIdAsync(id);
            if (product == null) return null;

            product.Update(dto.Name, dto.Description, dto.Price, dto.Stock,dto.Category, adminId);

            if (dto.ImageUrls != null && dto.ImageUrls.Length > 0)
                product.SetImages(dto.ImageUrls);

            await _repository.UpdateAsync(product);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ProductDto>(product);
        }


        public async Task<IEnumerable<string>> UploadImagesAsync(IEnumerable<ProductImageUploadDto> files)
        {
            var imageUrls = new List<string>();

            foreach (var file in files)
            {
                using var ms = new MemoryStream(file.Content);
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, ms),
                    Folder = "products"
                };

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

            return imageUrls;
        }
    }

  }


