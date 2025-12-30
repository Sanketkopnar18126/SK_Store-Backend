using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My_Store.Application.DTOs.Cart;

namespace My_Store.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId, CancellationToken ct = default);
        Task<CartDto> AddItemAsync(int userId, CreateCartItemDto dto, CancellationToken ct = default);
        Task<CartDto> UpdateItemQuantityAsync(int userId, UpdateCartItemDto dto, CancellationToken ct = default);
        Task<bool> RemoveItemAsync(int userId, int productId, CancellationToken ct = default);
        //Task<bool> ClearCartAsync(int userId, CancellationToken ct = default);
        Task<CartDto> EnsureCartExistsForUserAsync(int userId, CancellationToken ct = default);
    }
}
