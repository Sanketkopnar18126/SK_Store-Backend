using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using My_Store.Application.DTOs.User;

namespace My_Store.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> CreateAsync(UserCreateDto dto);
        Task<UserDto?> GetByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllAsync();
        Task UpdateAsync(int id, UserUpdateDto dto);
        Task DeleteAsync(int id);
        Task AssignRoleAsync(int id, string role);
    }
}
