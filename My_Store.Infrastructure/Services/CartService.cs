using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using My_Store.Application.DTOs.Cart;
using My_Store.Application.Interfaces;
using My_Store.Domain.Entities;

namespace My_Store.Infrastructure.Services
{
    public class CartService:ICartService
    {
        private readonly IUnitOfWork _uow;
        private readonly IProductRepository _productRepo; 
        private readonly IMapper _mapper;
        private readonly ILogger<CartService> _logger;

        public CartService(IUnitOfWork uow, IMapper mapper, ILogger<CartService> logger)
        {
            _uow = uow;
            _mapper = mapper;
            _logger = logger;
            _productRepo = uow.Products; 
        }

        public async Task<CartDto> EnsureCartExistsForUserAsync(int userId, CancellationToken ct = default)
        {
            var cart = await _uow.Products.Query().FirstOrDefaultAsync(); 
            var existing = await _uow.Products.GetAsync(predicate: null, asNoTracking: true, ct: ct); 
            throw new NotImplementedException("Ensure CartRepository is exposed via IUnitOfWork. See note below.");
        }

       
        public async Task<CartDto> GetCartAsync(int userId, CancellationToken ct = default)
        {
            var cart = await ((dynamic)_uow).Carts.GetByUserIdWithItemsAsync(userId, ct); 
            if (cart == null) 
                return new CartDto { UserId = userId, Items = new System.Collections.Generic.List<CartItemDto>(), Total = 0m };

            var dto = _mapper.Map<CartDto>(cart);
            dto.Total = cart.GetTotal();
            return dto;
        }

        public async Task<CartDto> AddItemAsync(int userId, CreateCartItemDto dto, CancellationToken ct = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Quantity <= 0) throw new ArgumentException("Quantity must be > 0", nameof(dto.Quantity));

            var product = await _productRepo.GetByIdAsync(dto.ProductId, ct);
            if (product == null) throw new InvalidOperationException("Product not found");

            var cartRepo = ((dynamic)_uow).Carts as ICartRepository;
            var cart = await cartRepo.GetByUserIdWithItemsAsync(userId, ct);
            if (cart == null)
            {
                cart = new Cart(userId);
                await cartRepo.AddAsync(cart, ct);
            }

            cart.AddOrUpdateItem(dto.ProductId, dto.Quantity, product.Price);
            await _uow.CommitAsync(ct);

            var result = _mapper.Map<CartDto>(cart);
            result.Total = cart.GetTotal();
            return result;
        }

        public async Task<CartDto> UpdateItemQuantityAsync(int userId, UpdateCartItemDto dto, CancellationToken ct = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var cartRepo = ((dynamic)_uow).Carts as ICartRepository;
            var cart = await cartRepo.GetByUserIdWithItemsAsync(userId, ct);
            if (cart == null) return new CartDto { UserId = userId, Items = new System.Collections.Generic.List<CartItemDto>(), Total = 0m };

            if (dto.Quantity <= 0)
                cart.RemoveItem(dto.ProductId);
            else
                cart.SetItemQuantity(dto.ProductId, dto.Quantity);

            await _uow.CommitAsync(ct);

            var result = _mapper.Map<CartDto>(cart);
            result.Total = cart.GetTotal();
            return result;
        }

        public async Task<bool> RemoveItemAsync(int userId, int productId, CancellationToken ct = default)
        {
            var cartRepo = ((dynamic)_uow).Carts as ICartRepository;
            var cart = await cartRepo.GetByUserIdWithItemsAsync(userId, ct);
            if (cart == null) return false;

            cart.RemoveItem(productId);
            await _uow.CommitAsync(ct);
            return true;
        }


    }
}

