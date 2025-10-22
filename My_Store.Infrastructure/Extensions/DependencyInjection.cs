using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using My_Store.Application.Interfaces;
using My_Store.Application.Mappings;
using My_Store.Infrastructure.Persistence;
using My_Store.Infrastructure.Repositories;
using My_Store.Infrastructure.Services;

namespace My_Store.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(conn, sqlOptions =>
                {
                    // Optional: enable resilient retries for transient SQL errors
                    sqlOptions.EnableRetryOnFailure();
                }));

            // Register repository implementations (example):
            services.AddScoped<IPasswordHasher<My_Store.Domain.Entities.User>, PasswordHasher<My_Store.Domain.Entities.User>>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(typeof(ProductProfile));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
