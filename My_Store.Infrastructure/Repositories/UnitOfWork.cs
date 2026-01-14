using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using My_Store.Application.Interfaces;
using My_Store.Infrastructure.Persistence;

namespace My_Store.Infrastructure.Repositories
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly AppDbContext _context;
        private ProductRepository? _productRepository;
        private CartRepository? _cartRepository;
        private UserRepository? _userRepository;
        private BannerRepository? _bannersRepository;
        private PaymentRepository? _paymentRepository;
        private OrderRepository? _orderRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
        public ICartRepository Carts => _cartRepository ??= new CartRepository(_context);
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);
        public IBannerRepository Banners => _bannersRepository ??= new BannerRepository(_context);
        public IPaymentRepository Payments => _paymentRepository ??= new PaymentRepository(_context);
        public IOrderRepository Orders => _orderRepository ??= new OrderRepository(_context);


        public async Task<int> CommitAsync(CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        public void Dispose()
        {
            _context.Dispose();
        }


    }
}
