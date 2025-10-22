using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using My_Store.Application.Interfaces;
using My_Store.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using My_Store.Application.DTOs.User;


namespace My_Store.Infrastructure.Services
{
    public class UserService: IUserService
    {

        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository repo,
            IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            ILogger<UserService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task AssignRoleAsync(int id, string role)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            user.Role = role;
            await _repo.UpdateAsync(user);
            await _repo.SaveChangesAsync();
        }

        public async Task<UserDto> CreateAsync(UserCreateDto dto)
        {
            var existingUser = await _repo.GetByUsernameAsync(dto.Username);
            if (existingUser != null) throw new Exception("Username already exists");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _repo.AddAsync(user);
            await _repo.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            await _repo.DeleteAsync(user);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task UpdateAsync(int id, UserUpdateDto dto)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            _mapper.Map(dto, user);
            await _repo.UpdateAsync(user);
            await _repo.SaveChangesAsync();
        }
    }
}
