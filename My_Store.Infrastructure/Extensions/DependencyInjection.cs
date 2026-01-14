using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            //  Sql ==> PSQL


            //services.AddDbContext<AppDbContext>(options =>
            //    options.UseSqlServer(conn, sqlOptions =>
            //    {
            //        // Optional: enable resilient retries for transient SQL errors
            //        sqlOptions.EnableRetryOnFailure();
            //    }));

            // Register repository implementations (example):

            services.AddDbContext<AppDbContext>(options =>
                      options.UseNpgsql(conn));


            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderRepository, OrderRepository>();


            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }
    }
}
